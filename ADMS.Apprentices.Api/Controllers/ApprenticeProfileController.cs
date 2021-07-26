using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.HttpClients;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v1/apprentices")]
    [Route("api/apprentices")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeProfileController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IProfileCreator profileCreator;
        private readonly IProfileUpdater profileUpdater;
        private readonly IProfileRetreiver profileRetreiver;

        /// <summary>Constructor</summary>
        public ApprenticeProfileController(
            IRepository repository,
            IPagingHelper pagingHelper,
            IProfileCreator profileCreator,
            IProfileUpdater profileUpdater,
            IProfileRetreiver profileRetreiver
        )
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.profileCreator = profileCreator;
            this.profileUpdater = profileUpdater;
            this.profileRetreiver = profileRetreiver;
        }

        /// <summary>
        /// List all apprentice profile and returns generic summary information for each apprentice.
        /// </summary>
        /// <param name="paging">Paging information</param>
        [HttpGet]
        [SupportsPaging(null)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        public async Task<ActionResult<PagedList<ProfileListModel>>> List([FromQuery] PagingInfo paging )
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("id", true);
            PagedList<Profile> profiles = await pagingHelper.ToPagedListAsync(repository.Retrieve<Profile>().Where(x => x.ActiveFlag == true), paging);
            IEnumerable<ProfileListModel> models = profiles.Results.Map(a => new ProfileListModel(a));
            return Ok(new PagedList<ProfileListModel>(profiles, models));
        }


        /// <summary>
        /// Get all apprentice profile based on the provided search params.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="paging">Paging information</param>
        [HttpPost("search")]
        [SupportsPaging(null)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        public ActionResult<PagedList<ProfileSearchResultModel>> Search([FromBody] ProfileSearchMessage message, [FromQuery] PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("ScoreValue", true);
            PagedInMemoryList<ProfileSearchResultModel> profiles = pagingHelper.ToPagedInMemoryList(profileRetreiver.Search(message), paging);
            return Ok(profiles);
        }

        /// <summary>
        /// Gets all information of a given apprentice id.
        /// </summary>
        /// <param name="id">Id of the apprentice</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileModel>> Get(int id)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            return Ok(new ProfileModel(profile));
        }


        /// <summary>
        /// Creates a new apprentice profile
        /// </summary>
        /// <param name="message">Details of the apprentice profile to be created</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPost]
        public async Task<ActionResult<ProfileModel>> Create([FromBody] ProfileMessage message)
        {
            Profile profile = await profileCreator.CreateAsync(message);
            await repository.SaveAsync();
            return Created($"/{profile.Id}", new ProfileModel(profile));
        }

        /// <summary>
        /// Updates an existing apprentice.
        /// </summary>
        /// <param name="id">ID of the application to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileModel>> Update(int id, [FromBody] UpdateProfileMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            await profileUpdater.Update(profile, message);
            await repository.SaveAsync();
            return Ok(new ProfileModel(profile));
        }

        /// <summary>
        /// Updates an existing apprentice deceased flag to true.
        /// </summary>
        /// <param name="id">ID of the apprentice</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut("{id}/deceased")]
        public async Task<ActionResult<ProfileModel>> Deceased(int id)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            profileUpdater.UpdateDeceasedFlag(profile, true);
            await repository.SaveAsync();
            return Ok(new ProfileModel(profile));
        }


        /*******************************************************
         *The current assumption that we dont allow network providers to change the deceased flag to false once they set to true.
         *So creating separate endpoint in case if the department user need to update it to false.
         *Feel free to change to another name / uri :)
         *******************************************************/
        /// <summary>
        /// Updates an existing apprentice deceased flag to false and other special department updates.
        /// </summary>
        /// <param name="id">ID of the apprentice</param>
        /// <param name="message"></param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Activiate)]
        [HttpPut("{id}/admin-update")]
        public async Task<ActionResult<ProfileModel>> AdminUpdate(int id, [FromBody] AdminUpdateMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            profileUpdater.Update(profile, message);
            await repository.SaveAsync();
            return Ok(new ProfileModel(profile));
        }

        /// <summary>
        /// Updates  apprentice CRN from Service Australia .
        /// </summary>
        /// <param name="id">ID of the apprentice</param>
        /// <param name="message"></param>
        /// TODO Figure out an appropriate policy for this function
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpPut("{id}/service-australia-apprentice-update")]
        public async Task<ActionResult<ProfileModel>> ServiceAustraliaApprenticeUpdate(int id, [FromBody] ServiceAustraliaUpdateMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            profileUpdater.UpdateCRN(profile, message);
            await repository.SaveAsync();
            return Ok(new ProfileListModel(profile));
        }
    }
}