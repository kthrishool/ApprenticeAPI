using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.HttpClients.USI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services
{
    public class USIVerifyDisabled : IUSIVerify
    {
        /// <summary>
        /// mock verification if the usi integration is disabled
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>ApprenticeUSI</returns>
        public ApprenticeUSI Verify(Profile profile)
        {
            return null;
        }
    }
}