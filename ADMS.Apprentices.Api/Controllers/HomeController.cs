using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Adms.Shared;
using ADMS.Apprentices.Core;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Provides information about the API
    /// </summary>
    [Route("")]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly ISharedSettings sharedSettings;
        private readonly IOptions<OurTestingSettings> ourTestingSettings;
        private readonly IOptions<OurEnvironmentSettings> ourEnvironmentSettings;

        /// <summary>
        /// </summary>
        /// <param name="sharedSettings"></param>
        /// <param name="ourTestingSettings"></param>
        /// <param name="ourEnvironmentSettings"></param>
        public HomeController(ISharedSettings sharedSettings, IOptions<OurTestingSettings> ourTestingSettings, IOptions<OurEnvironmentSettings> ourEnvironmentSettings)
        {
            this.sharedSettings = sharedSettings;
            this.ourTestingSettings = ourTestingSettings;
            this.ourEnvironmentSettings = ourEnvironmentSettings;
        }

        /// <summary>
        /// API Home
        /// </summary>
        /// <remarks>
        /// Lists available endpoints, documentation URLs, and build details
        /// </remarks>
        [HttpGet]
        public ActionResult Summary()
        {
            string root = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            string swaggerUrl = $"{root}/{ourEnvironmentSettings.Value.SwaggerPath}";            
            Dictionary<string, object> summary = new()
            {
                { "apprentices_url", $"{root}{Request.Path}/apprentices" },
                { "swagger_url", $"{swaggerUrl}" },
                { "file_version", typeof(HomeController).Assembly.GetName().Version.ToString() },
                { "product_version", FileVersionInfo.GetVersionInfo(typeof(HomeController).Assembly.Location).ProductVersion },
                { "sortable_list_row_limit", sharedSettings.SortableListRowLimit },
                { "spreadsheet_export_row_limit", sharedSettings.SpreadsheetExportRowLimit },
                { "testing_tools_enabled", ourTestingSettings.Value.EnableTestingTools },
                { "claim_mappings", sharedSettings.Authorisation.Mappings },
            };            
            return Ok(summary);
        }
    }
}