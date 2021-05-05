using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Exceptions
{
    public enum ValidationExceptionType
    {
        [ExceptionDetails("AP-VAL-0001", "Apprentice must be at least 12 years old")]
        InvalidApprenticeAge,

        [ExceptionDetails("AP-VAL-0002", "Invalid apprentice profile type ")]
        InvalidApprenticeprofileType,

        [ExceptionDetails("AP-VAL-0003", "ApprenticeId is not valid")]
        InvalidApprenticeId,

        [ExceptionDetails("AP-VAL-0004", "TFN already recorded for this apprentice.")]
        TFNAlreadyExists,

        [ExceptionDetails("AP-VAL-0005", "TFN is not valid")]
        InvalidTFN,

        [ExceptionDetails("AP-VAL-0006", "Phone number is mandatory")]
        NullPhoneNumber,

        [ExceptionDetails("AP-VAL-0007", "Invalid apprentice phone number")]
        InvalidPhoneNumber,

        [ExceptionDetails("AP-VAL-0008", "Invalid email address")]
        InvalidEmailAddress,

        [ExceptionDetails("AP-VAL-0008", "Invalid postcode")]
        InvalidPostcode,

        [ExceptionDetails("AP-VAL-0009", "Invalid address details")]
        AddressRecordNotFound,

        [ExceptionDetails("AP-VAL-0010", "State selected is invalid for the postcode selected")]
        PostCodeStateCodeMismatch,

        [ExceptionDetails("AP-VAL-0011", "Suburb selected is invalid for the postcode")]
        PostCodeLocalityMismatch,

        [ExceptionDetails("AP-VAL-0012", "Invalid state code")]
        InvalidStateCode,

        [ExceptionDetails("AP-VAL-0013", "Street address line exceeds 80 characters")]
        StreetAddressExceedsMaxLength,

        [ExceptionDetails("AP-VAL-0014", "Suburb name exceeds 40 characters")]
        SuburbExceedsMaxLength,

        [ExceptionDetails("AP-VAL-0015", "Street address line cannot be null")]
        StreetAddressLine1CannotBeNull,

        [ExceptionDetails("AP-VAL-0016", "Unexpected error while processing your request")]
        ServerError,

        [ExceptionDetails("AP-VAL-0017", "Invalid country code")]
        InvalidCountryCode,

        [ExceptionDetails("AP-VAL-0018", "Postcode selected is invalid for the state selected")]
        PostCodeMismatch,

        [ExceptionDetails("AP-VAL-0019", "Invalid language code")]
        InvalidLanguageCode,

        [ExceptionDetails("AP-VAL-0020", "Invalid highest school level code")]
        InvalidHighestSchoolLevelCode,

        [ExceptionDetails("AP-VAL-0021", "Invalid month code")]
        InvalidMonthCode,

        [ExceptionDetails("AP-VAL-0022", "Invalid left school year")]
        InvalidLeftSchoolYear,

        [ExceptionDetails("AP-VAL-0023", "Missing mobile phone details for preferred contact")]
        MobilePreferredContactIsInvalid,

        [ExceptionDetails("AP-VAL-0024", "Missing email address for preferred contact")]
        EmailPreferredContactisInvalid,

        [ExceptionDetails("AP-VAL-0025", "Missing phone details for preferred contact")]
        PhonePreferredContactisInvalid,

        [ExceptionDetails("AP-VAL-0026", "Missing address details for preferred contact")]
        MailPreferredContactisInvalid,

        [ExceptionDetails("AP-VAL-0027", "Invalid preferred contact type code")]
        InvalidPreferredContactCode,

        [ExceptionDetails("AP-VAL-0028", "Invalid qualification details")]
        InvalidQualification,

        [ExceptionDetails("AP-VAL-0030", "Invalid qualification level")]
        InvalidQualificationLevel,

        [ExceptionDetails("AP-VAL-0031", "Invalid qualification ANZSCO")]
        InvalidQualificationANZSCO,

        [ExceptionDetails("AP-VAL-0032", "Birth date is required")]
        InvalidDOB,

        [ExceptionDetails("AP-VAL-0033", "Invalid left school details")]
        InvalidLeftSchoolDetails,

        [ExceptionDetails("AP-VAL-0034", "Duplicate qualification identified")]
        DuplicateQualification,
    }
}