using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IValidatorExceptionBuilderFactory
    {
        IValidatorExceptionBuilder CreateExceptionBuilder();
    }
}