using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.UnitTests.Heplers
{
    public interface IPropertyValidator
    {
        IList<ValidationResult> ValidateModel(object model);
    }
}