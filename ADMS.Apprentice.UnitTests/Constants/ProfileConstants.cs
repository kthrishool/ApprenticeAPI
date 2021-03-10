﻿using System;
using System.Linq;

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
        public static string Emailaddressmax256 =  "nljflkjdljflksdjflksdjflkjsdfkljsdkljflksdjfldjsfkljsdnljflkjdljflksdjflksdjflkjsdfklfjklsdjfklldsjfhsdkhfkhfjkhsdkfhsdjkhfkhjkfhsdjkh@jklfsdhkhfksdhfjkshdfjkhsdkjfhjksdhfjkhsdfkjhkdsjhfjksdhfkjhkjhfkjsdhfkjsdhfjkhsdjkhfjksdhfjk.hjkfshdkfhskdjhfkjsdhfjkshfsdhjkfhsdjkfhkjsdfhjksdhfjkhsdjkfhksjdhfjsdhfhfsdfhkshkbddvcjhrjkw";
        public static DateTime Createdon = DateTime.Now.AddMinutes(-3);
        public static DateTime Updatedon = DateTime.Now;
        public static string Createdby = "User1";
        public static string Updatedby = "User2";



      
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


             
    }
}