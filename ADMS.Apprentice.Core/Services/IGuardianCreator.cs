﻿using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IGuardianCreator
    {
        Task<Guardian> CreateAsync(ProfileGuardianMessage message);
    }
}