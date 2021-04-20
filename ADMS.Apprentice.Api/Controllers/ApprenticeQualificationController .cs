using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using System;
using System.Linq;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Services.Validators;
using Microsoft.EntityFrameworkCore;

namespace ADMS.Apprentice.Api.Controllers
{
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/qualificatios")]
    [Route("api/apprentices/{apprenticeId}/qualifications")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeQualificationController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IQualificationValidator qualificationValidator;        
        //private readonly IProfileUpdater profileUpdater;

        public ApprenticeQualificationController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IQualificationValidator qualificationValidator
            //IProfileUpdater profileUpdater            
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.qualificationValidator = qualificationValidator;            
            //this.profileUpdater = profileUpdater;
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
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
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
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            Qualification qualification = profile.Qualifications.SingleOrDefault(x => x.Id == id);
            return Ok(new ProfileQualificationModel(qualification));
        }


        /// <summary>
        /// Creates a new qualification for an apprentice
        /// </summary>
        /// <param name="message">Details of the qualification to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileQualificationModel>> Create(int apprenticeId, [FromBody] ProfileQualificationMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            Qualification qualification = new Qualification
            {
                QualificationCode = message.QualificationCode.SanitiseUpper(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                StartMonth = message.StartMonth.SanitiseUpper(),
                StartYear = message.StartYear.Sanitise(),
                EndMonth = message.EndMonth.SanitiseUpper(),
                EndYear = message.EndYear.Sanitise(),
            };
            
            await qualificationValidator.ValidateAsync(new List<Qualification> { qualification });
            profile.Qualifications.Add(qualification);
            await repository.SaveAsync();
            return Created($"/{profile.Qualifications.LastOrDefault().Id}", new ProfileQualificationModel(qualification));
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
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            Qualification qualification = profile.Qualifications.SingleOrDefault(x => x.Id == id);
            if (qualification != null)
            {
                qualification.QualificationCode = message.QualificationCode;
                qualification.QualificationDescription = message.QualificationDescription;
                qualification.StartMonth = message.StartMonth.SanitiseUpper();
                qualification.StartYear = message.StartYear.Sanitise();
                qualification.EndMonth = message.EndMonth.SanitiseUpper();
                qualification.EndYear = message.EndYear.Sanitise();
            }
            await qualificationValidator.ValidateAsync(new List<Qualification> { qualification });
            await repository.SaveAsync();
            return Ok(new ProfileQualificationModel(profile.Qualifications.SingleOrDefault(x => x.Id == id)));
        }
    }
}