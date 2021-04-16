using System;
using System.ComponentModel;
using System.Linq;
using ADMS.Apprentice.Core.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public static class PhoneValidator
    {
        private static readonly string[] startingCode = {"02", "03", "04", "07", "08", "13", "18"};

        public static bool ValidatePhone(ref string phoneNumber, ValidationExceptionType errorMessage)
        {
            if (!Enum.IsDefined(typeof(ValidationExceptionType), errorMessage))
                throw new InvalidEnumArgumentException(nameof(errorMessage), (int) errorMessage, typeof(ValidationExceptionType));
            phoneNumber = new string(phoneNumber.ToCharArray().Where(char.IsDigit).ToArray());
            if ((phoneNumber.Length == 11) && phoneNumber.Substring(0, 2) == "61")
                phoneNumber = phoneNumber.Replace("61", "0");

            if (!(startingCode.Contains(phoneNumber.Substring(0, 2)) && phoneNumber.Length == 10))
            {
                errorMessage = ValidationExceptionType.InvalidPhoneNumber;
                return false;
            }
            return true;
        }
    }
}