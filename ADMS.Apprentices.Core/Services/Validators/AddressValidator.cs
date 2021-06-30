using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using System.Collections.Generic;
using System;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class AddressValidator : IAddressValidator
    {
        private readonly IReferenceDataClient referenceDataClient;
        
        private readonly IExceptionFactory exceptionFactory;

        public AddressValidator(
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.referenceDataClient = referenceDataClient;
            this.exceptionFactory = exceptionFactory;

        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(IAddressAttributes address)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            if (address == null) {
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }
            if (!string.IsNullOrEmpty(address.SingleLineAddress))
                exceptionBuilder.AddExceptions(await ValidateSingleLineAddressAsync(address));
            else
            {
                //Do the default code validations
                exceptionBuilder.AddExceptions(ValidateDefaultCodes(address));
                if (exceptionBuilder.HasExceptions())
                    return exceptionBuilder;
                //At this point we know we have a valid Locality, State and postcode. So use iGas to get the partical geolocation details
                exceptionBuilder.AddExceptions(await ValidatePartialAddressAsync(address));
            }
            return exceptionBuilder;
        }


        private ValidationExceptionBuilder ValidateDefaultCodes(IAddressAttributes address)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            // If the single line address is empty we need to check if other details are valid.
            // if the postcode is there then validate it first
            if (string.IsNullOrWhiteSpace(address.Postcode) ||
                string.IsNullOrWhiteSpace(address.Locality) ||
                string.IsNullOrWhiteSpace(address.StateCode))
            {
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }                
            if (address.Postcode.Length != 4 || address.Postcode.All(char.IsDigit) == false)
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPostcode);
            if (address.StateCode.Length > 10)
                exceptionBuilder.AddException(ValidationExceptionType.InvalidStateCode);

            if (address.Locality.Length > 40)
                exceptionBuilder.AddException(ValidationExceptionType.SuburbExceedsMaxLength);
            if (address.StreetAddress2?.Length > 80)
                exceptionBuilder.AddException(ValidationExceptionType.StreetAddressExceedsMaxLength);
            if (address.StreetAddress3?.Length > 80)
                exceptionBuilder.AddException(ValidationExceptionType.StreetAddressExceedsMaxLength);

            if (string.IsNullOrWhiteSpace(address.StreetAddress1)) {
                exceptionBuilder.AddException(ValidationExceptionType.StreetAddressLine1CannotBeNull);
                return exceptionBuilder;
            }
            if (address.StreetAddress1.Length > 80)
                exceptionBuilder.AddException(ValidationExceptionType.StreetAddressExceedsMaxLength);
            return exceptionBuilder;
        }

        private async Task<ValidationExceptionBuilder> ValidateSingleLineAddressAsync(IAddressAttributes address)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            //Verify the address using iGas
            //If it is a valid single line address, iGas will return a record with geo location details         

            DetailAddressModel detailAddress = await referenceDataClient.GetDetailAddressByFormattedAddress(address.SingleLineAddress);
            //no chance to have detail address to be null, but in case of reasons..
            if (detailAddress == null) {
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }

            if (detailAddress.StreetAddressLine1.IsNullOrEmpty() || detailAddress.Locality.IsNullOrEmpty() || detailAddress.State.IsNullOrEmpty() || detailAddress.Postcode.IsNullOrEmpty()) {
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }

            //populate geo location + postcode suburb details to profile address from detailsAddress component
            address.SingleLineAddress = detailAddress.FormattedAddress.Sanitise();
            address.StreetAddress1 = detailAddress.StreetAddressLine1.Sanitise();
            address.StreetAddress2 = detailAddress.StreetAddressLine2.Sanitise();
            address.StreetAddress3 = detailAddress.StreetAddressLine2.Sanitise();
            address.Locality = detailAddress.Locality;
            address.StateCode = detailAddress.State;
            address.Postcode = detailAddress.Postcode;
            address.GeocodeType = detailAddress.GeocodeType;
            address.Latitude = detailAddress.Latitude;
            address.Longitude = detailAddress.Longitude;
            address.Confidence = (short) detailAddress.Confidence;
            return exceptionBuilder;
        }

        private async Task<ValidationExceptionBuilder> ValidatePartialAddressAsync(IAddressAttributes address)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            //Verify the partial address using iGas. Partial address = Locality + State + postcode
            //If it is a valid, populate geo location details

            string formattedLocality = $"{address.Locality} {address.StateCode} {address.Postcode}".ToUpper();

            PartialAddressModel partialAddress = await referenceDataClient.GetAddressByFormattedLocality(formattedLocality);

            if (partialAddress == null) {
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }

            //if iGas couldnt resolve the partial address
            if (string.IsNullOrEmpty(partialAddress.Locality) || string.IsNullOrEmpty(partialAddress.State) || string.IsNullOrEmpty(partialAddress.Postcode))
            { 
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }
        
            //Looking for contains rather than exact match on Locality in case of spelling erros or terminologies like Civic, Civic square, Toowoomba city, Toowoomba DC etc
            if (!partialAddress.Locality.Contains(address.Locality.ToUpper()) && !address.Locality.ToUpper().Contains(partialAddress.Locality))
                exceptionBuilder.AddException(ValidationExceptionType.PostCodeLocalityMismatch);

            if (partialAddress.State != address.StateCode.ToUpper())
                exceptionBuilder.AddException(ValidationExceptionType.PostCodeStateCodeMismatch);

            if (partialAddress.Postcode != address.Postcode)
                exceptionBuilder.AddException(ValidationExceptionType.PostCodeMismatch);
            //update address with geo location details
            //partial address doenst provide confidence info, but according to below info assigning confidence level of 4
            //The confidence level from GNAF for the address and Geocode.Should be used in conjunction with Geo Code Type
            //- 1 = The address is not contained in any of three major contributors / sources of addresses(GNAF)
            //0 = The address is contained in only one of the three major contributors / sources of addresses(GNAF)
            //1 = The address is contained in two of the three major contributors / sources of addresses(GNAF)
            //2 = The address is contained in all three of the three major contributors / sources of addresses(GNAF)
            //3 = A Match was not found on Street Number.The Geocode returned will only be accurate to the Street(iGas)
            //4 = A Match was not found on Street.The Geocode returned will only be accurate to the Locality(iGas)
            //5 = Not found(iGas)

            if(exceptionBuilder.HasExceptions())
                return exceptionBuilder;

            address.Locality = address.Locality.ToUpper();
            address.StateCode = partialAddress.State;
            address.Postcode = partialAddress.Postcode;
            address.Latitude = partialAddress.Latitude;
            address.Longitude = partialAddress.Longitude;
            address.Confidence = 4;
            return exceptionBuilder;
        }
    }
}