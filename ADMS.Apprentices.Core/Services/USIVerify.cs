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
    public class USIVerify : IUSIVerify
    {
        private readonly IRepository repository;
        private readonly IUSIClient usiClient;
        private readonly IExceptionFactory exceptionFactory;

        public USIVerify(IRepository repository, IUSIClient usiClient, IExceptionFactory exceptionFactory)
        {
            this.repository = repository;
            this.usiClient = usiClient;
            this.exceptionFactory = exceptionFactory;
        }
        /// <summary>
        /// Verify the active USI of given apprentice
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>ApprenticeUSI</returns>
        public ApprenticeUSI Verify(Profile profile)
        {
            //get the active apprenticeUsi record.
            ApprenticeUSI apprenticeUSI = profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault();
            if (apprenticeUSI == null) return null;                

            //The available end points for USI verification accepts lists of USI requests - so need to convert into a list even though we verify single USI
            List<VerifyUsiMessage> messages = new List<VerifyUsiMessage>();
            VerifyUsiMessage message = new VerifyUsiMessage 
            { 
                RecordId = apprenticeUSI.Id,
                FirstName = profile.FirstName,
                FamilyName = profile.Surname,
                DateOfBirth = profile.BirthDate,
                USI = apprenticeUSI.USI,
            };
            messages.Add(message);
            try
            {
                //Get the verify result - get the first one as we know we pass only one USI in the request.
                VerifyUsiModel model =  usiClient.VerifyUsi(messages).Result.First();

                apprenticeUSI.DateOfBirthMatchedFlag = model.DateOfBirthMatched;
                apprenticeUSI.FirstNameMatchedFlag = model.FirstNameMatched;
                apprenticeUSI.SurnameMatchedFlag = model.FamilyNameMatched;
                apprenticeUSI.USIStatus = model.USIStatus;
                apprenticeUSI.USIVerifyFlag = model.FirstNameMatched.HasValue && model.DateOfBirthMatched.HasValue && model.FamilyNameMatched.HasValue
                    && model.FirstNameMatched.Value && model.DateOfBirthMatched.Value && model.FamilyNameMatched.Value && model.USIStatus == "Valid";
                
                return apprenticeUSI;
            }
            catch
            {
                //hardly get excetion from the external api. In case if we get it, silently continue. Log into logging database may be?
                return apprenticeUSI;
            }            
        }
    }
}