using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class PhoneValidator : IPhoneValidator
    {
        private static readonly string[] startingCode = {"02", "03", "04", "07", "08", "13", "18"};


        private readonly IExceptionFactory exceptionFactory;


        public PhoneValidator(
            IExceptionFactory exceptionFactory
        )
        {
            this.exceptionFactory = exceptionFactory;
        }

        public void ValidatePhonewithType(Phone phone)
        {
            phone.PhoneTypeCode = PhoneType.LANDLINE.ToString();
            phone.PhoneNumber = new string(phone.PhoneNumber.ToCharArray().Where(char.IsDigit).ToArray());
            if ((phone.PhoneNumber.Length == 11) && phone.PhoneNumber.Substring(0, 2) == "61")
                phone.PhoneNumber = phone.PhoneNumber.Replace("61", "0");

            if (!(startingCode.Contains(phone.PhoneNumber.Substring(0, 2)) && phone.PhoneNumber.Length == 10))
            {
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber);
            }
            if (phone.PhoneNumber.Substring(0, 2) == "04")
                phone.PhoneTypeCode = PhoneType.MOBILE.ToString();
        }

        public string ValidatePhone(string phoneNumber, ValidationExceptionType exception)
        {
            if (phoneNumber.Sanitise() == null)
                return null;
            phoneNumber = new string(phoneNumber.Sanitise().ToCharArray().Where(char.IsDigit).ToArray());
            if ((phoneNumber.Length == 11) && phoneNumber.Substring(0, 2) == "61")
                phoneNumber = phoneNumber.Replace("61", "0");

            if (!(startingCode.Contains(phoneNumber.Substring(0, 2)) && phoneNumber.Length == 10))
                throw exceptionFactory.CreateValidationException(exception);

            return phoneNumber;
        }
    }
}