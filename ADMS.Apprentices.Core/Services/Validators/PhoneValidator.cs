using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Helpers;
using Adms.Shared.Exceptions;
using System;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class PhoneValidator : IPhoneValidator
    {
        private static readonly string[] startingCode = {"02", "03", "04", "07", "08", "13", "18"};

        public PhoneValidator(
        )
        {
        }

        public void ValidatePhonewithType(ValidationExceptionBuilder exceptionBuilder, Phone phone)
        {         
            phone.PhoneNumber = phone.PhoneNumber.Sanitise();

            if(phone.PhoneNumber.IsNullOrEmpty()) {
                exceptionBuilder.AddException(ValidationExceptionType.NullPhoneNumber);
                return;
            }

            if (!phone.PhoneTypeCode.IsNullOrEmpty() && !(Enum.IsDefined(typeof(PhoneType), phone.PhoneTypeCode)))
            {
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneTypeCode);
                return;
            }
            
            phone.PhoneNumber = new string(phone.PhoneNumber.ToCharArray().Where(char.IsDigit).ToArray());

            if(phone.PhoneNumber.Substring(0,2) == "61") 
                phone.PhoneNumber = "0" + phone.PhoneNumber.Substring(2, phone.PhoneNumber.Length - 2);

            if(phone.PhoneNumber.Length < 10) {
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);
                return;
            }

            if (!(startingCode.Contains(phone.PhoneNumber.Substring(0, 2)) && phone.PhoneNumber.Length == 10))
            {
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);
            }

            if(phone.PhoneNumber.Length != 10) 
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);

            if (phone.PhoneNumber.Substring(0, 2) == "04")
            {
                if (phone.PhoneTypeCode.IsNullOrEmpty() || phone.PhoneTypeCode == PhoneType.MOBILE.ToString())
                    phone.PhoneTypeCode = PhoneType.MOBILE.ToString();
                else
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);
            }
            else
            {
                if (phone.PhoneTypeCode.IsNullOrEmpty() || phone.PhoneTypeCode == PhoneType.LANDLINE.ToString())
                    phone.PhoneTypeCode = PhoneType.LANDLINE.ToString();
                else
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);
            }
        }

        public string ValidatePhone(ValidationExceptionBuilder exceptionBuilder, string phoneNumber, ValidationExceptionType exception)
        {
            if (phoneNumber.Sanitise() == null) {
                return null;
            }
            phoneNumber = new string(phoneNumber.Sanitise().ToCharArray().Where(char.IsDigit).ToArray());

            if(phoneNumber.Substring(0,2) == "61") 
                phoneNumber = "0" + phoneNumber.Substring(2, phoneNumber.Length - 2);

            if(phoneNumber.Length < 10) {
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPhoneNumber);
                return phoneNumber;
            }

            if (!(startingCode.Contains(phoneNumber.Substring(0, 2)) && phoneNumber.Length == 10))
                exceptionBuilder.AddException(exception);

            if(phoneNumber.Length != 10)
                exceptionBuilder.AddException(exception);

            return phoneNumber;
        }
    }
}