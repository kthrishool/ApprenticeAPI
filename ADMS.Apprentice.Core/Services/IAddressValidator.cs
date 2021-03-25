using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IAddressValidator
    {
        List<Address> Validate(Profile message);
    }
}