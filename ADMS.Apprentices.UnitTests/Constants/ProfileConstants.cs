using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;

namespace ADMS.Apprentices.UnitTests.Constants
{
    public static class ProfileConstants
    {
        private static Random random = new();
        public static int Id = 1234;
        public static string Firstname = "Alex";
        public static string Secondname = "Charlie";
        public static string Surname = "Bob";
        public static string PreferredName = "Bob";
        public static DateTime Birthdate = DateTime.Now.AddYears(-25);
        public static string Emailaddress = "test@test.com";
        public static string Emailaddressmax256 = RandomString(247) + "@test.com";
        public static string Phone1 = "1234567890";
        public static string Phone2 = "+123456 789";

        public static List<PhoneNumberMessage> PhoneNumbers => new()
        {
            new PhoneNumberMessage() {PhoneNumber = "0212345678"},
            new PhoneNumberMessage() {PhoneNumber = "+61 2 1234 1111", PreferredPhoneFlag = true}
        };

        public static List<UpdatePhoneNumberMessage> UpdatedPhoneNumbers => new()
        {
            new UpdatePhoneNumberMessage() {PhoneNumber = "0212345678", Id = 0},
            new UpdatePhoneNumberMessage() {PhoneNumber = "0212345678", Id = 1},
            new UpdatePhoneNumberMessage() {PhoneNumber = "+61 2 1234 1111", PreferredPhoneFlag = true}
        };

        public static List<string> InvalidPhoneNumbers => new() {"0212345678", "+61 2 1234 1111"};
        public static string Profiletype = "APPR";
        public static string IndigenousStatusCode = "@";
        public static string SelfAssessedDisabilityCode = "Y";
        public static string CitizenshipCode = "A";
        public static bool? InterpretorRequiredFlag = true;
        public static DateTime Createdon = DateTime.Now.AddMinutes(-3);
        public static DateTime Updatedon = DateTime.Now;
        public static string Createdby = "User1";
        public static string Updatedby = "User2";
        public static ProfileAddressMessage ResidentialAddress => new() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street"};
        public static ProfileAddressMessage PostalAddress => new() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = null};
        public static ProfileAddressMessage ResidentialSingleLineAddress => new() {SingleLineAddress = "14 Mort Street ACT BRADDON 2601"};
        public static string GenderCode = "M";
        public static string CountryOfBirthCode = "1101";
        public static string LanguageCode = "1200";
        public static string HighestSchoolLevelCode = "DONTKNOW";
        public static string LeftSchoolMonthCode = "JAN";
        public static int? LeftSchoolYear = 2000;
        public static bool DeceasedFlag = false;
        public static ProfileType ProfileTypeCode = ProfileType.APPR;
        public static PreferredContactType PreferredContactType = PreferredContactType.MOBILE;
        public static PriorQualification Qualification => new() {QualificationCode = "QCode", QualificationDescription = "QDescription", QualificationLevel = "1101", QualificationANZSCOCode = "1101", NotOnTrainingGovAu = true, StartDate = new DateTime(2000, 10, 1), EndDate = new DateTime(2002, 10, 1)};
        public static PriorApprenticeshipQualification PriorApprenticeshipQualification => new() {EmployerName = "employer-name", QualificationCode = "QCode", QualificationDescription = "QDescription", QualificationLevel = "1101", QualificationANZSCOCode = "1101", NotOnTrainingGovAu = true, StartDate = new DateTime(2000, 10, 1), ApprenticeshipReference = "apprenticeship-reference"};
        public static DateTime LeftSchoolDate = DateTime.Now.AddYears(-5);

        public static PriorQualificationMessage QualificationMessage => new()
            {QualificationCode = "QCode", QualificationDescription = "QDescription", StartDate = new DateTime(2010, 1, 1), EndDate = new DateTime(2020, 1, 1)};

        public static bool ActiveFlag = true;

        public static string VisaNumber = "12345678901";

        public static string USI = "thisisTestUsi";
        public static string CustomerReferenceNumber = "1234567890";

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}