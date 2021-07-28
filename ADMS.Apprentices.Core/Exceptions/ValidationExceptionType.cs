using Adms.Shared.Exceptions;

namespace ADMS.Apprentices.Core.Exceptions
{
    public enum ValidationExceptionType
    {
        [ExceptionDetails("AP-VAL-0001", "Apprentice must be at least 12 years old")]
        InvalidApprenticeAge,

        [ExceptionDetails("AP-VAL-0002", "Invalid apprentice profile type")]
        InvalidApprenticeprofileType,

        [ExceptionDetails("AP-VAL-0003", "ApprenticeId is not valid")]
        InvalidApprenticeId,

        [ExceptionDetails("AP-VAL-0004", "TFN already recorded for this apprentice")]
        TFNAlreadyExists,

        [ExceptionDetails("AP-VAL-0005", "TFN is not valid")]
        InvalidTFN,

        [ExceptionDetails("AP-VAL-0006", "Phone number cannot be null")]
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

        [ExceptionDetails("AP-VAL-0021", "Invalid indigenous status code")]
        InvalidIndegenousStatusCode,

        [ExceptionDetails("AP-VAL-0022", "Invalid citizenship code")]
        InvalidCitizenshipCode,

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

        [ExceptionDetails("AP-VAL-0028", "Missing qualification code")]
        MissingQualificationCode,

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

        [ExceptionDetails("AP-VAL-0035", "Invalid USI code ")]
        InvalidUSI,

        [ExceptionDetails("AP-VAL-0036", "CRN exceeds 10 characters")]
        InvalidCRN,

        [ExceptionDetails("AP-VAL-0037", "Invalid address details for guardian")]
        AddressRecordNotFoundForGuardian,

        [ExceptionDetails("AP-VAL-0038", "Invalid guardian phone number")]
        InvalidGuardianNumber,

        [ExceptionDetails("AP-VAL-0039", "Minimum 8 numbers if searching only by phonenumber")]
        InvalidPhonenumberSearch,

        [ExceptionDetails("AP-VAL-0040", "Minimum 4 characters if searching only by email address")]
        InvalidEmailSearch,

        [ExceptionDetails("AP-VAL-0041", "A phone or email address must be provided")]
        MandatoryContact,

        [ExceptionDetails("AP-VAL-0042", "A guardian already exists for this apprentice")]
        GuardianExists,

        [ExceptionDetails("AP-VAL-0043", "Invalid phone type code")]
        InvalidPhoneTypeCode,

        [ExceptionDetails("AP-VAL-0044", "The apprenticeship referenced by the qualification does not exist")]
        QualificationApprenticeshipDoesNotExist,

        [ExceptionDetails("AP-VAL-0045", "The apprenticeship referenced by the qualification has a different qualification code")]
        QualificationApprenticeshipQualificationCodeDoesNotMatch,

        [ExceptionDetails("AP-VAL-0046", "The apprenticeship referenced by the qualification is not yet complete")]
        QualificationApprenticeshipIsNotComplete,

        [ExceptionDetails("AP-VAL-0047", "Date completed cannot be before Date commenced")]
        DateMismatch,

        [ExceptionDetails("AP-VAL-0048", "Date commenced and Date completed cannot be before apprentice DOB +12 years")]
        DOBDateMismatch,

        [ExceptionDetails("AP-VAL-0049", "Date commenced and Date completed cannot be after today's date")]
        InvalidDate,

        [ExceptionDetails("AP-VAL-0050", "Apprenticeship qualification should have a start date and end date")]
        InvalidApprenticeshipQualification,

        [ExceptionDetails("AP-VAL-0051", "ApprenticeshipId does not belong to the apprentice")]
        InvalidApprenticeshipIDForQualification,

        [ExceptionDetails("AP-VAL-0052", "Nothing to search. Please add your search criteria")]
        InvalidSearch,

        [ExceptionDetails("AP-VAL-0053", "Cannot search by state if searching only by address")]
        InvalidAddressSearch,

        [ExceptionDetails("AP-VAL-0054", "Insufficient apprentice identity information to perform a search. You must provide at least date of birth and first name or surname, USI, phone number or email address.")]
        InsufficientApprenticeIdentitySearchCriteria,

        [ExceptionDetails("AP-VAL-0055", "Insufficient apprentice identity information to perform a search. When searching by date of birth you must also provide surname and / or first name.")]
        BirthDateMustBeCombinedWithFirstNameOrSurname,

        [ExceptionDetails("AP-VAL-0055", "Insufficient apprentice identity information to perform a search. When searching by surname or first name you must also provide date of birth.")]
        FirstNameOrSurnameMustBeCombinedWithBirthDate,
    }
}