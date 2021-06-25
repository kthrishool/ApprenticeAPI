using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.UnitTests.Helpers
{
    public interface IPropertyValidator
    {
        IList<ValidationResult> ValidateModel(object model);
    }
}