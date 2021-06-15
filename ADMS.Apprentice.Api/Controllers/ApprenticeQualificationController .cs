using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Services;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Api.Controllers
{
    /// <summary>
    /// Apprentice qualification endpoints of a given apprentice.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/qualifications")]
    [Route("api/apprentices/{apprenticeId}/qualifications")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeQualificationController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IQualificationCreator qualificationCreator;
        private readonly IQualificationUpdater qualificationUpdater;
        private readonly IExceptionFactory exceptionFactory;

        /// <summary>Constructor</summary>
        public ApprenticeQualificationController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IQualificationCreator qualificationCreator,
            IQualificationUpdater qualificationUpdater,
            IExceptionFactory exceptionFactory
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.qualificationCreator = qualificationCreator;
            this.qualificationUpdater = qualificationUpdater;
            this.exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// List all qualifications of an apprentice.
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="paging">Paging information</param>
        [HttpGet]
        [SupportsPaging(null)]
        public async Task<ActionResult<PagedList<ProfileQualificationModel>>> List(int apprenticeId, PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("id", true);
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);

            IQueryable<Qualification> query = profile.Qualifications.AsQueryable();            
            PagedList<Qualification> qualifications = pagingHelper.ToPagedList(query, paging);
            IEnumerable<ProfileQualificationModel> models = qualifications.Results.Map(a => new ProfileQualificationModel(a));
            return Ok(new PagedList<ProfileQualificationModel>(qualifications, models));
        }

        /// <summary>
        /// Gets all information of a given qualification id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the qualification</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Get(int apprenticeId, int id)
        {            
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);            
            Qualification qualification = profile.Qualifications.Single(x => x.Id == id);
            return Ok(new ProfileQualificationModel(qualification));
        }


        /// <summary>
        /// Adds a new qualification for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the qualification to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileQualificationModel>> Create(int apprenticeId, [FromBody] ProfileQualificationMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            message.profile = profile;
            Qualification qualification = await qualificationCreator.CreateAsync(message);
            profile.Qualifications.Add(qualification);            

            await repository.SaveAsync();
            return Created($"/{qualification.Id}", new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Updates an existing qualification claim application.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the qualification to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Update(int apprenticeId, int id, [FromBody] ProfileQualificationMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Qualification qualification = profile.Qualifications.SingleOrDefault(x => x.Id == id);
            if (qualification == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Qualification ", id.ToString());

            await qualificationUpdater.Update(qualification, message);
            
            await repository.SaveAsync();
            return Ok(new ProfileQualificationModel(qualification));
        }
    }
}