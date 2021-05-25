using System;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class USIValidator : IUSIValidator
    {
        private readonly IExceptionFactory exceptionFactory;


        public USIValidator(IExceptionFactory exceptionFactory)
        {
            this.exceptionFactory = exceptionFactory;
        }

        static readonly char[] validChars =
        {
            '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z'
        };


        static bool VerifyKey(string key)
        {
            if (key.Length != 10)
                return false;
            char checkDigit = GenerateCheckCharacter(key.ToUpper().Substring(0, 9));
            return key[9] == checkDigit;
        }

        // Implementation of Luhn Mod N algorithm for check digit.
        static char GenerateCheckCharacter(string input)
        {
            int factor = 2;
            int sum = 0;
            int n = validChars.Length;
            // Starting from the right and working leftwards is easier since
            // the initial "factor" will always be "2"
            for (int i = input.Length - 1; i >= 0; i--)
            {
                int codePoint = Array.IndexOf(validChars, input[i]);
                int addend = factor * codePoint;
                // Alternate the "factor" that each "codePoint" is multiplied by
                factor = (factor == 2) ? 1 : 2;
                // Sum the digits of the "addend" as expressed in base "n"
                addend = (addend / n) + (addend % n);
                sum += addend;
            }
            // Calculate the number that must be added to the "sum"
            // to make it divisible by "n"
            int remainder = sum % n;
            int checkCodePoint = (n - remainder) % n;
            return validChars[checkCodePoint];
        }

        public Boolean Validate(Profile profile)
        {
            if (profile.USIs.Any())
            {
                if (profile.USIs.Single(x => x.ActiveFlag == true).USI.Sanitise() == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidUSI);

                // code to be implemented fro additional validation
                if (VerifyKey(profile.USIs.Single(x => x.ActiveFlag == true).USI.Sanitise()))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidUSI);
            }
            return true;
        }
    }
}