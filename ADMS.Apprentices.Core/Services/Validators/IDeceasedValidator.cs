using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IDeceasedValidator
    {
        void Validate(Profile profile);
    }
}