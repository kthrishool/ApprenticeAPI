using System;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IUSIValidator
    {
        ValidationExceptionBuilder Validate(Profile profile);
    }
}