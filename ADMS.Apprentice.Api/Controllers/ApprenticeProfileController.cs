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

namespace ADMS.Apprentice.Api.Controllers
{
    /// <summary>
    /// Apprentice endpoints.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices")]
    [Route("api/apprentices")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeProfileController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IProfileCreator profileCreator;
        private readonly IProfileUpdater profileUpdater;
        private readonly IProfileRetreiver profileRetreiver;
        private readonly IUSIVerify usiVerify;

        /// <summary>Constructor</summary>
        public ApprenticeProfileController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IProfileCreator profileCreator,
            IProfileUpdater profileUpdater,
            IProfileRetreiver profileRetreiver,
            IUSIVerify usiVerify
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.profileCreator = profileCreator;
            this.profileUpdater = profileUpdater;
            this.profileRetreiver = profileRetreiver;
            this.usiVerify = usiVerify;
        }

        /// <summary>
        /// List all apprentice profile and returns generic summary information for each apprentice.
        /// </summary>
        /// <param name="paging">Paging information</param>
        [HttpGet]
        [SupportsPaging(null)]
        public async Task<ActionResult<PagedList<ProfileListModel>>> List(PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("id", true);
            //   PagedList<Profile> profiles = await pagingHelper.ToPagedListAsync(repository.Retrieve<Profile>(), paging);
            PagedList<Profile> profiles = await pagingHelper.ToPagedListAsync(profileRetreiver.RetreiveList(), paging);
            IEnumerable<ProfileListModel> models = profiles.Results.Map(a => new ProfileListModel(a));
            return Ok(new PagedList<ProfileListModel>(profiles, models));
        }

        /// <summary>
        /// Gets all information of a given apprentice id.
        /// </summary>
        /// <param name="id">Id of the apprentice</param>
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