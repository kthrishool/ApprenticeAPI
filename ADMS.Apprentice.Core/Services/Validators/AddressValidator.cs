﻿using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using System.Collections.Generic;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class AddressValidator : IAddressValidator
    {
        private readonly IReferenceDataClient referenceDataClient;
        
        private readonly IValidatorExceptionBuilderFactory exceptionBuilderFactory;

        public AddressValidator(
            IValidatorExceptionBuilderFactory exceptionBuilderFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.referenceDataClient = referenceDataClient;
            this.exceptionBuilderFactory = exceptionBuilderFactory;

        }

        public async Task<IValidatorExceptionBuilder> ValidateAsync(IAddressAttributes address)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            if (address == null) {
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);
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


        private IValidatorExceptionBuilder ValidateDefaultCodes(IAddressAttributes address)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            // If the single line address is empty we need to check if other details are valid.
            // if the postcode is there then validate it first
            if (string.IsNullOrWhiteSpace(address.Postcode) ||
                string.IsNullOrWhiteSpace(address.Locality) ||
                string.IsNullOrWhiteSpace(address.StateCode))
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);
            if (address.Postcode.Length != 4 || address.Postcode.All(char.IsDigit) == false)
                exceptionBuilder.Add(ValidationExceptionType.InvalidPostcode);
            if (address.StateCode.Length > 10)
                exceptionBuilder.Add(ValidationExceptionType.InvalidStateCode);

            if (address.Locality.Length > 40)
                exceptionBuilder.Add(ValidationExceptionType.SuburbExceedsMaxLength);
            if (address.StreetAddress2?.Length > 80)
                exceptionBuilder.Add(ValidationExceptionType.StreetAddressExceedsMaxLength);
            if (address.StreetAddress3?.Length > 80)
                exceptionBuilder.Add(ValidationExceptionType.StreetAddressExceedsMaxLength);

            if (string.IsNullOrWhiteSpace(address.StreetAddress1)) {
                exceptionBuilder.Add(ValidationExceptionType.StreetAddressLine1CannotBeNull);
                return exceptionBuilder;
            }
            if (address.StreetAddress1.Length > 80)
                exceptionBuilder.Add(ValidationExceptionType.StreetAddressExceedsMaxLength);
            return exceptionBuilder;
        }

        private async Task<IValidatorExceptionBuilder> ValidateSingleLineAddressAsync(IAddressAttributes address)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            //Verify the address using iGas
            //If it is a valid single line address, iGas will return a record with geo location details         

            DetailAddressModel detailAddress = await referenceDataClient.GetDetailAddressByFormattedAddress(address.SingleLineAddress);
            //no chance to have detail address to be null, but in case of reasons..
            if (detailAddress == null) {
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }

            if (detailAddress.StreetAddressLine1.IsNullOrEmpty() || detailAddress.Locality.IsNullOrEmpty() || detailAddress.State.IsNullOrEmpty() || detailAddress.Postcode.IsNullOrEmpty()) {
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);
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

        private async Task<IValidatorExceptionBuilder> ValidatePartialAddressAsync(IAddressAttributes address)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            //Verify the partial address using iGas. Partial address = Locality + State + postcode
            //If it is a valid, populate geo location details

            string formattedLocality = $"{address.Locality} {address.StateCode} {address.Postcode}".ToUpper();

            PartialAddressModel partialAddress = await referenceDataClient.GetAddressByFormattedLocality(formattedLocality);

            if (partialAddress == null) {
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);
                return exceptionBuilder;
            }

            //if iGas couldnt resolve the partial address
            if (string.IsNullOrEmpty(partialAddress.Locality) || string.IsNullOrEmpty(partialAddress.State) || string.IsNullOrEmpty(partialAddress.Postcode))
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFound);

            if (partialAddress.Locality != address.Locality.ToUpper())
                exceptionBuilder.Add(ValidationExceptionType.PostCodeLocalityMismatch);

            if (partialAddress.State != address.StateCode.ToUpper())
                exceptionBuilder.Add(ValidationExceptionType.PostCodeStateCodeMismatch);

            if (partialAddress.Postcode != address.Postcode)
                exceptionBuilder.Add(ValidationExceptionType.PostCodeMismatch);
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

            address.Locality = partialAddress.Locality;
            address.StateCode = partialAddress.State;
            address.Postcode = partialAddress.Postcode;
            address.Latitude = partialAddress.Latitude;
            address.Longitude = partialAddress.Longitude;
            address.Confidence = 4;
            return exceptionBuilder;
        }
    }
}