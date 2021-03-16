using System.Linq;
using ADMS.Apprentice.Core.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public static class PhoneValidator
    {
        private static readonly string[] StartingCode = {"02", "03", "04", "07", "08", "13", "18"};

        public static bool ValidatePhone(string phoneNumber, ValidationExceptionType errorMessage)
        {
            phoneNumber = new string(phoneNumber.ToCharArray().Where(char.IsDigit).ToArray());
            if ((phoneNumber.Length == 11) && phoneNumber.Substring(0, 2) == "61")
                phoneNumber = phoneNumber.Replace("61", "0");

            if (!(StartingCode.Contains(phoneNumber.Substring(0, 2)) && phoneNumber.Length == 10))
            {
                errorMessage = ValidationExceptionType.InvalidPhoneNumber;
                return false;
            }
            return true;
        }
    }
}