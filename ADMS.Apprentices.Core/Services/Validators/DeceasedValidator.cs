using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using ADMS.Apprentices.Core.Entities;
using System;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class DeceasedValidator : IDeceasedValidator
    {
        public void Validate(Profile profile)
        {
            bool deceased = profile.DeceasedFlag;
            bool hasDeceasedDate = profile.DeceasedDate.HasValue;
            if (hasDeceasedDate && profile.DeceasedDate.Value < profile.BirthDate)
                throw AdmsValidationException.Create(ValidationExceptionType.DeceasedDateDOBMismatch);
            if (hasDeceasedDate && profile.DeceasedDate.Value > System.DateTime.Now.Date)
                throw AdmsValidationException.Create(ValidationExceptionType.DeceasedDateCurrentDateMismatch);
            if (deceased && !hasDeceasedDate)
                throw AdmsValidationException.Create(ValidationExceptionType.DeceasedDateRequired);
            if (!deceased && hasDeceasedDate)
                throw AdmsValidationException.Create(ValidationExceptionType.DeceasedFlagDeceasedDateMismatch);
        }
    }
}