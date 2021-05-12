using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;

namespace ADMS.Apprentice.UnitTests.Constants
{
    public static class ProfileConstants
    {
        private static Random random = new Random();
        public static int Id = 1234;
        public static string Firstname = "Alex";
        public static string Secondname = "Charlie";
        public static string Surname = "Bob";
        public static string PreferredName = "Bob";
        public static DateTime Birthdate = DateTime.Now.AddYears(-25);
        public static string Emailaddress = "test@test.com";
        public static string Emailaddressmax256 = RandomString(247) + "@test.com";

        public static List<PhoneNumberMessage> PhoneNumbers = new List<PhoneNumberMessage>()
        {
            new PhoneNumberMessage() {PhoneNumber = "0212345678"},
            new PhoneNumberMessage() {PhoneNumber = "+61 2 1234 1111", PreferredPhoneFlag = true}
        };

        public static List<string> InvalidPhoneNumbers = new List<string>() {"0212345678", "+61 2 1234 1111"};
        public static string Profiletype = "APPR";
        public static string IndigenousStatusCode = "@";
        public static string SelfAssessedDisabilityCode = "Y";
        public static string CitizenshipCode = "A";
        public static bool? InterpretorRequiredFlag = true;
        public static DateTime Createdon = DateTime.Now.AddMinutes(-3);
        public static DateTime Updatedon = DateTime.Now;
        public static string Createdby = "User1";
        public static string Updatedby = "User2";
        public static ProfileAddressMessage ResidentialAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street"};
        public static readonly ProfileAddressMessage PostalAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = null};
        public static ProfileAddressMessage ResidentialSingleLineAddress = new ProfileAddressMessage() {SingleLineAddress = "14 Mort Street ACT BRADDON 2601"};
        public static string GenderCode = "M";
        public static string CountryOfBirthCode = "1101";
        public static string LanguageCode = "1200";
        public static string HighestSchoolLevelCode = "DONTKNOW";
        public static string LeftSchoolMonthCode = "JAN";
        public static int? LeftSchoolYear = 2000;
        public static bool DeceasedFlag = false;
        public static ProfileType ProfileTypeCode = ProfileType.APPR;
        public static PreferredContactType PreferredContactType = PreferredContactType.MOBILE;
        public static Qualification Qualification = new Qualification() {QualificationCode = "QCode", QualificationDescription = "QDescription", QualificationLevel = "1101", QualificationANZSCOCode = "1101", StartDate = new DateTime(2000, 10, 1), EndDate = new DateTime(2002, 10, 1)};

        public static ProfileQualificationMessage QualificationMessage = new ProfileQualificationMessage()
            {QualificationCode = "QCode", QualificationDescription = "QDescription", StartMonth = "JAN", StartYear = 2000, EndMonth = "DEC", EndYear = 2004};

        public static bool ActiveFlag = true;

        public static string VisaNumber = "12345678901";

        // public static byte[] Version = Convert.ToBase64String("1.1") ;
        public static string USI = "thisisTestUsi";

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}