﻿using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using Adms.Shared.Exceptions;
using System;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class PhoneValidator : IPhoneValidator
    {
        private static readonly string[] startingCode = {"02", "03", "04", "07", "08", "13", "18"};




        public PhoneValidator(
        )
        {
        }

        public void ValidatePhonewithType(IValidatorExceptionBuilder exceptionBuilder, Phone phone)
        {
            phone.PhoneTypeCode = PhoneType.LANDLINE.ToString();

            phone.PhoneNumber = phone.PhoneNumber.Sanitise();

            if(phone.PhoneNumber.IsNullOrEmpty()) {
                exceptionBuilder.Add(ValidationExceptionType.InvalidPhoneNumber);
                return;
            }

            phone.PhoneNumber = new string(phone.PhoneNumber.ToCharArray().Where(char.IsDigit).ToArray());

            if(phone.PhoneNumber.Substring(0,2) == "61") 
                phone.PhoneNumber = "0" + phone.PhoneNumber.Substring(2, phone.PhoneNumber.Length - 2);

            if(phone.PhoneNumber.Length < 10) {
                exceptionBuilder.Add(ValidationExceptionType.InvalidPhoneNumber);
                return;
            }

            if (!(startingCode.Contains(phone.PhoneNumber.Substring(0, 2)) && phone.PhoneNumber.Length == 10))
            {
                exceptionBuilder.Add(ValidationExceptionType.InvalidPhoneNumber);
            }

            if(phone.PhoneNumber.Length != 10) 
                exceptionBuilder.Add(ValidationExceptionType.InvalidPhoneNumber);

            if (phone.PhoneNumber.Substring(0, 2) == "04")
                phone.PhoneTypeCode = PhoneType.MOBILE.ToString();
        }

        public string ValidatePhone(IValidatorExceptionBuilder exceptionBuilder, string phoneNumber, ValidationExceptionType exception)
        {
            if (phoneNumber.Sanitise() == null) {
                return null;
            }
            phoneNumber = new string(phoneNumber.Sanitise().ToCharArray().Where(char.IsDigit).ToArray());

            if(phoneNumber.Substring(0,2) == "61") 
                phoneNumber = "0" + phoneNumber.Substring(2, phoneNumber.Length - 2);

            if(phoneNumber.Length < 10) {
                exceptionBuilder.Add(ValidationExceptionType.InvalidPhoneNumber);
                return phoneNumber;
            }

            if (!(startingCode.Contains(phoneNumber.Substring(0, 2)) && phoneNumber.Length == 10))
                exceptionBuilder.Add(exception);

            if(phoneNumber.Length != 10)
                exceptionBuilder.Add(exception);

            return phoneNumber;
        }
    }
}