using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IQualificationValidator
    {
        Task<List<Qualification>> ValidateAsync(List<Qualification> qualifications);
        Task<Qualification> ValidateAsync(Qualification qualifications);

        void CheckForDuplicates(List<Qualification> qualifications);
    }
}