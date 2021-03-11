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
    [Route("api/v1/apprentice-profile")]
    [Route("api/apprentice-profile")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeProfileController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IProfileCreator profileCreator;

        public ApprenticeProfileController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IProfileCreator profileCreator
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.profileCreator = profileCreator;
        }

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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileModel>> Get(int id)
        {
            Profile profile = await repository.GetAsync<Profile>(id);
            return Ok(new ProfileModel(profile));
        }


        [HttpPost]
        public async Task<ActionResult<ProfileModel>> Create([FromBody] ProfileMessage message)
        {
            Profile profile = await profileCreator.CreateAsync(message);
            await repository.SaveAsync();
            return Created($"/{profile.Id}", new ProfileModel(profile));
        }
    }
}