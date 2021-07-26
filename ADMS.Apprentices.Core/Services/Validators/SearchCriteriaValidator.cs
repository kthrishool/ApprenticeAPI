using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class SearchCriteriaValidator : ISearchCriteriaValidator
    {
        public void Validate(ApprenticeIdentitySearchCriteriaMessage message)
        {
            message ??= new ApprenticeIdentitySearchCriteriaMessage();
            bool hasDob = message.BirthDate != null;
            bool hasAPrimaryField =
                !message.USI.IsNullOrWhitespace() ||
                !message.PhoneNumber.IsNullOrWhitespace() ||
                !message.EmailAddress.IsNullOrWhitespace();
            bool hasAName = !message.FirstName.IsNullOrWhitespace() || !message.Surname.IsNullOrWhitespace();
            if (!hasAPrimaryField && !hasAName && !hasDob)
                throw AdmsValidationException.Create(ValidationExceptionType.MissingApprenticeIdentitySearchCriteria);
            if (!hasAPrimaryField && !hasDob)
                throw AdmsValidationException.Create(ValidationExceptionType.InsufficientApprenticeIdentitySearchCriteria);
            if (hasDob && !hasAPrimaryField && message.Surname.IsNullOrWhitespace())
                throw AdmsValidationException.Create(ValidationExceptionType.BirthDateMustBeCombinedWithOtherApprenticeIdentitySearchCriteria);
        }
    }
}