using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Exceptions
{
    public enum ValidationExceptionType
    {
        [ExceptionDetails("AP-VAL-0001", "Apprentice must be at least 12 years old")]
        InvalidApprenticeAge,

        [ExceptionDetails("AP-VAL-0002", "Invalid Apprentice profile type ")]
        InvalidApprenticeprofileType,

        [ExceptionDetails("AP-VAL-0003", "ApprenticeId is not valid")]
        InvalidApprenticeId,

        [ExceptionDetails("AP-VAL-0004", "TFN already recorded for this apprentice.")]
        TFNAlreadyExists,

        [ExceptionDetails("AP-VAL-0005", "TFN is not valid")]
        InvalidTFN,

        [ExceptionDetails("AP-VAL-0006", "Phone number is mandatory")]
        NullPhoneNumber,

        [ExceptionDetails("AP-VAL-0007", "Invalid Apprentice phone number")]
        InvalidPhoneNumber,

        [ExceptionDetails("AP-VAL-0008", "Invalid Email address")]
        InvalidEmailAddress,

        [ExceptionDetails("AP-VAL-0008", "Invalid Postcode")]
        InvalidPostcode,

        [ExceptionDetails("AP-VAL-0009", "Invalid Address details")]
        AddressRecordNotFound,

        [ExceptionDetails("AP-VAL-0010", "State selected is Invalid for the postcode selected")]
        PostCodeStateCodeMismatch,

        [ExceptionDetails("AP-VAL-0011", "Suburb selected is Invalid for the postcode")]
        PostCodeLocalityMismatch,

        [ExceptionDetails("AP-VAL-0012", "Invalid State Code")]
        InvalidStateCode,

        [ExceptionDetails("AP-VAL-0013", "Street Address Line Exceeds 80 Characters")]
        StreetAddressExceedsMaxLength,

        [ExceptionDetails("AP-VAL-0014", "Suburb name Exceeds 40 Characters")]
        SuburbExceedsMaxLength,

        [ExceptionDetails("AP-VAL-0015", "Street Address Line cannot be null")]
        StreetAddressLine1CannotBeNull,

        [ExceptionDetails("AP-VAL-0016", "Unexpected error while processing your request")]
        ServerError,

        [ExceptionDetails("AP-VAL-0017", "Invalid Country code")]
        InvalidCountryCode,

        [ExceptionDetails("AP-VAL-0018", "Postcode selected is Invalid for the state selected")]
        PostCodeMismatch

    }
}