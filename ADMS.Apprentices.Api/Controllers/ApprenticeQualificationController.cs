﻿using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice qualification endpoints of a given apprentice.
    /// </summary>
    [ApiController]
    //[ApiDescription(Summary = "Apprentice qualification endpoints of a given apprentice", Description = "")]
    [Route("api/v1/apprentices/{apprenticeId}/qualifications")]
    [Route("api/apprentices/{apprenticeId}/qualifications")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeQualificationController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IQualificationCreator qualificationCreator;
        private readonly IQualificationUpdater qualificationUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeQualificationController(
            IRepository repository,
            IQualificationCreator qualificationCreator,
            IQualificationUpdater qualificationUpdater
        )
        {
            this.repository = repository;
            this.qualificationCreator = qualificationCreator;
            this.qualificationUpdater = qualificationUpdater;
        }

        /// <summary>
        /// List all qualifications for an apprentice.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet]
        public async Task<ActionResult<ProfileQualificationModel[]>> List(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            ProfileQualificationModel[] models = profile.Qualifications.Map(a => new ProfileQualificationModel(a)).ToArray();
            return Ok(models);
        }

        /// <summary>
        /// Gets all information of a given qualification id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the qualification</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Get(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Qualification qualification = profile.Qualifications.Get(q => q.Id, id);
            return Ok(new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Adds a new qualification for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the qualification to be created</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPost]
        public async Task<ActionResult<ProfileQualificationModel>> Create(int apprenticeId, [FromBody] ProfileQualificationMessage message)
        {
            Qualification qualification = await qualificationCreator.CreateAsync(apprenticeId, message);
            await repository.SaveAsync();
            return Created($"/{qualification.Id}", new ProfileQualificationModel(qualification));
        }
        
        /// <summary>
        /// Updates an existing qualification claim application.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the qualification to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Update(int apprenticeId, int id, [FromBody] ProfileQualificationMessage message)
        {
            var qualification = await qualificationUpdater.Update(apprenticeId, id, message);
            await repository.SaveAsync();
            return Ok(new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Removes a qualification from an apprentice profile
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the qualification to be removed</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            Qualification qualification = profile.Qualifications.Get(q => q.Id, id);
            profile.Qualifications.Remove(qualification);
            await repository.SaveAsync();
            return new NoContentResult();
        }
    }
}