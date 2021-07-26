using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface ISearchCriteriaValidator
    {
        void Validate(ApprenticeIdentitySearchCriteriaMessage message);
    }
}