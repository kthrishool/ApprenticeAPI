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

        public ApprenticeProfileController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IProfileCreator profileCreator,
            IProfileUpdater profileUpdater            
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.profileCreator = profileCreator;            
            this.profileUpdater = profileUpdater;
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
            PagedList<Profile> profiles = await pagingHelper.ToPagedListAsync(repository.Retrieve<Profile>(), paging);
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
        /// Updates an existing AAIP claim application.
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
    }
}