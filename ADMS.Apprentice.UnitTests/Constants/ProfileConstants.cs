using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Messages;

namespace ADMS.Apprentice.UnitTests.Constants
{
    public static class ProfileConstants
    {
        private static Random random = new Random();
        public static string Firstname = "Alex";
        public static string Secondname = "Charlie";
        public static string Surname = "Bob";
        public static DateTime Birthdate = DateTime.Now.AddYears(-25);
        public static string Emailaddress = "test@test.com";
        public static string Emailaddressmax256 = RandomString(247) + "@test.com";
        public static List<string> PhoneNumbers = new List<string>() {"0212345678", "+61 2 1234 1111"};
        public static List<string> InvalidPhoneNumbers = new List<string>() {"0212345678", "+61 2 1234 1111"};
        public static string Profiletype = "APPR";
        public static DateTime Createdon = DateTime.Now.AddMinutes(-3);
        public static DateTime Updatedon = DateTime.Now;
        public static string Createdby = "User1";
        public static string Updatedby = "User2";
        public static ProfileAddressMessage ResidentialAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = ""};
        public static readonly ProfileAddressMessage PostalAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = ""};
        public static string GenderCode = "M";

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}