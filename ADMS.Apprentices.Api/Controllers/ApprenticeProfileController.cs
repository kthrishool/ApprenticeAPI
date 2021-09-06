using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Api.Configuration;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice endpoints.
    /// </summary>
    [ApiController]
    [Route("v1/apprentices")]
    [Route("apprentices")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeProfileController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IApprenticeRepository apprenticeRepository;
        private readonly IPagingHelper pagingHelper;
        private readonly IProfileCreator profileCreator;
        private readonly IProfileUpdater profileUpdater;
        private readonly IProfileRetriever profileRetriever;
        private readonly ISearchCriteriaValidator searchCriteriaValidator;

        /// <summary>Constructor</summary>
        public ApprenticeProfileController(
            IRepository repository,
            IApprenticeRepository apprenticeRepository,
            IPagingHelper pagingHelper,
            IProfileCreator profileCreator,
            IProfileUpdater profileUpdater,
            IProfileRetriever profileRetriever,
            ISearchCriteriaValidator searchCriteriaValidator
        )
        {
            this.repository = repository;
            this.apprenticeRepository = apprenticeRepository;
            this.pagingHelper = pagingHelper;
            this.profileCreator = profileCreator;
            this.profileUpdater = profileUpdater;
            this.profileRetriever = profileRetriever;
            this.searchCriteriaValidator = searchCriteriaValidator;
        }

        /// <summary>
        /// List generic summary information of all apprentices or apprentices match the optional search parameters
        /// </summary>
        /// <param name="paging">Paging information</param>
        /// <param name="message">search options</param>
        [HttpGet]
        [SupportsPaging(null)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        public async Task<ActionResult<PagedList<ProfileListModel>>> List([FromQuery] PagingInfo paging, [FromQuery] ProfileSearchMessage message)
        {
            paging ??= new PagingInfo();
            PagedList<ProfileListModel> pagedList = await profileRetriever.RetreiveList(paging, message);
            return Ok(pagedList);
        }


        /// <summary>
        /// Get all apprentice profile based on the provided search params.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="paging">Paging information</param>
        [HttpPost("search")]
        [SupportsPaging(null)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [Obsolete] //search is now combined witht list all end point.
        public async Task<ActionResult<PagedList<ProfileSearchResultModel>>> Search([FromBody] ProfileSearchMessage message, [FromQuery] PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("ScoreValue", true);
            PagedInMemoryList<ProfileSearchResultModel> profiles = pagingHelper.ToPagedInMemoryList(await profileRetriever.Search(message), paging);
            return Ok(profiles);
        }

        /// <summary>
        /// Gets apprentice profiles that match the supplied apprentice identity information.
        /// Used by the training contract to search for existing apprentice profiles that might
        /// match the apprentice identity information entered against the training contract
        /// </summary>
        /// <param name="message">Apprentice identity information</param>
        [HttpGet("identity-matches")]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        public async Task<ActionResult<ApprenticeIdentitySearchResultModel[]>> SearchByIdentity([FromQuery] ApprenticeIdentitySearchCriteriaMessage message)
        {
            searchCriteriaValidator.Validate(message);
            return Ok(await apprenticeRepository.GetMatchesByIdentityAsync(message));
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
        /// <param name="deceasedDate"></param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut("{id}/deceased")]
        public async Task<ActionResult<ProfileModel>> Deceased(int id, [FromQuery][Required] DateTime? deceasedDate)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            profileUpdater.UpdateDeceasedFlag(profile, true, deceasedDate);
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