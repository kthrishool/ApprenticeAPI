using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.HttpClients.USI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    //TODO: implement unit testing
    [ExcludeFromCodeCoverage]
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
        public async Task VerifyAsync(int apprenticeId, string usi)
        {
            //if (usi.IsNullOrEmpty()) return;                

            Profile profile = repository.Get<Profile>(apprenticeId);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());

            //get the active apprenticeUsi record.
            ApprenticeUSI apprenticeUSI = profile.USIs.FirstOrDefault(x => x.ActiveFlag == true && x.USI == usi);
            if (apprenticeUSI == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice USI", usi);

            //The available end points for USI verification accepts lists of USI requests - so need to convert into a list even though we verify single USI
            List<VerifyUsiMessage> messages = new List<VerifyUsiMessage>();
            VerifyUsiMessage message = new VerifyUsiMessage 
            { 
                RecordId = apprenticeUSI.Id,
                FirstName = profile.FirstName,
                FamilyName = profile.Surname,
                DateOfBirth = profile.BirthDate,
                USI = usi,
            };
            messages.Add(message);
            try
            {
                //Get the verify result - get the first one as we know we pass only one USI in the request.
                VerifyUsiModel model = usiClient.VerifyUsi(messages).Result.First();

                apprenticeUSI.DateOfBirthMatchedFlag = model.DateOfBirthMatched;
                apprenticeUSI.FirstNameMatchedFlag = model.FirstNameMatched;
                apprenticeUSI.SurnameMatchedFlag = model.FamilyNameMatched;
                apprenticeUSI.USIStatus = model.USIStatus;
                apprenticeUSI.USIVerifyFlag = model.FirstNameMatched.HasValue && model.DateOfBirthMatched.HasValue && model.FamilyNameMatched.HasValue
                    && model.FirstNameMatched.Value && model.DateOfBirthMatched.Value && model.FamilyNameMatched.Value;

                await repository.SaveAsync();
            }
            catch
            {
                //hardly get excetion from the external api. In case if we get it, silently continue. Log into logging database may be?
                return;
            }            
        }
    }
}