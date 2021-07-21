using System;
using System.Collections.Generic;
using System.Reflection;
using ADMS.Apprentices.Api.Configuration;
using ADMS.Apprentices.Api.Controllers;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Database;
using Adms.Shared;
using Adms.Shared.Authorisation;
using Adms.Shared.Helpers;
using Adms.Shared.Paging;
using Au.Gov.Infrastructure.Authorisation;
using Au.Gov.Infrastructure.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ADMS.Apprentices.IntegrationTests
{
    /// <summary>
    /// This is a sample test fixture which will be created once for the whole run of WeatherTest.
    /// Creating a TestServer is high cost so it is better to run a bunch of tests using a test fixture.
    /// </summary>
    public class ApprenticesFixture : BaseInternalFixture
    {

        private const string ROLE_Dept_Admin = "DAPD";
        private const string ROLE_Dept_ITAdmin = "DPIA";
        private const string ROLE_Dept_ServiceDelivery = "DPSD";
        private const string ROLE_Dept_ViewOnly = "DPVW";

        private const string ROLE_NP_Admin = "NPAD";
        private const string ROLE_NP_ClaimAdmin = "NPCA";
        private const string ROLE_NP_Management = "NPMG";
        private const string ROLE_NP_View = "NPVW";

        private const string ROLE_STA_View = "STVW";
        private const string ROLE_STA_Admin = "STAD";

        // SA = Services Australia i.e. Centrelink
        private const string ROLE_SA_View = "CNVW";
        
        public override List<Type> Controllers()
        {
            // these are the controllers we are going to test.
            return new List<Type>() {typeof(ApprenticeProfileController)};
        }

        public override void Register(IServiceCollection services)
        {
            // register mock services here.
            Assembly core = typeof(OurDatabaseSettings).Assembly;
            Assembly database = typeof(Repository).Assembly;
            Assembly web = typeof(DependencyInjectionConfiguration).Assembly;
            Assembly shared = typeof(PagingHelper).Assembly;

            IocRegistrationHelper.SetupAutoRegistrations(services, core);
            IocRegistrationHelper.SetupAutoRegistrations(services, database);
            IocRegistrationHelper.SetupAutoRegistrations(services, web);
            IocRegistrationHelper.SetupAutoRegistrations(services, shared);

            var mockRepo = new Mock<IRepository>();
            mockRepo.SetupAllProperties();
            services.AddSingleton<IRepository>(mockRepo.Object);

            var mockSettings = new Mock<ISharedSettings>();
            mockSettings.SetupAllProperties();
            services.AddSingleton<ISharedSettings>(mockSettings.Object);

            var mockRefDataClient = new Mock<IReferenceDataClient>();
            mockRefDataClient.SetupAllProperties();
            services.AddSingleton<IReferenceDataClient>(mockRefDataClient.Object);

            var mockUsiVerify = new Mock<IUSIVerify>();
            mockUsiVerify.SetupAllProperties();
            services.AddSingleton<IUSIVerify>(mockUsiVerify.Object);

            var mockApprenticeRepository = new Mock<IApprenticeRepository>();
            mockApprenticeRepository.SetupAllProperties();
            services.AddSingleton<IApprenticeRepository>(mockApprenticeRepository.Object);
        }

        public override void Use(IApplicationBuilder applicationBuilder)
        {
            // any additional IApplicationBuilder items go here.
        }

        /// <summary>
        /// Return the authorisation model for ADMS.Apprentices.Api
        /// </summary>
        public override Dictionary<string, List<ActivityClaimValue>> Activities()
        {
            // set up the activities your API controller needs here
            var model = new AuthorisationModel
            {
                Activities = new Dictionary<string, List<ActivityClaimValue>>(StringComparer.OrdinalIgnoreCase)
            };

            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_View,
                        ROLE_Dept_Admin, ROLE_Dept_ITAdmin, ROLE_Dept_ServiceDelivery, ROLE_Dept_ViewOnly
                            ,ROLE_NP_Admin, ROLE_NP_ClaimAdmin, ROLE_NP_Management, ROLE_NP_View
                            ,ROLE_STA_View
                            ,ROLE_SA_View
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_Management,
                        ROLE_Dept_Admin, ROLE_Dept_ITAdmin, ROLE_Dept_ServiceDelivery
                            ,ROLE_NP_Admin, ROLE_NP_ClaimAdmin, ROLE_NP_Management
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_Activiate,
                        ROLE_Dept_Admin, ROLE_Dept_ITAdmin, ROLE_Dept_ServiceDelivery
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_Merge,
                        ROLE_Dept_Admin, ROLE_Dept_ITAdmin, ROLE_Dept_ServiceDelivery
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_TSL_Management,
                        ROLE_Dept_Admin
                            ,ROLE_NP_ClaimAdmin, ROLE_NP_Management
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_TSL_View,
                        ROLE_Dept_Admin, ROLE_Dept_ITAdmin, ROLE_Dept_ServiceDelivery, ROLE_Dept_ViewOnly
                            ,ROLE_NP_Admin, ROLE_NP_ClaimAdmin, ROLE_NP_Management, ROLE_NP_View
            );
            AddAuthorisation(model, AuthorisationConfiguration.AUTH_Apprentice_Gateway,
                            ROLE_NP_Admin, ROLE_NP_ClaimAdmin, ROLE_NP_Management
            );

            AddAuthorisation(model, AuthorisationConfiguration.AUTH_ITAdmin,
                        ROLE_Dept_ITAdmin
            );
            return model.Activities;
        }
        
        private static void AddAuthorisation(AuthorisationModel model, string policy, params string[] roles)
        {
            var claimRoles = new List<ActivityClaimValue>();
            foreach(var role in roles) {
                claimRoles.Add(new ActivityClaimValue {
                    ClaimValue = role,
                    ClaimSchema = "http://dese.gov.au/adms/claims/baserole",
                });
            }
            model.Activities.Add(policy, claimRoles);
        }

        ///// <summary>
        ///// This is the mock version of our repository. We'll use the real controller with this fake repository.
        ///// </summary>
        //public class MockWeatherRepository:IWeatherRepository
        //{
        //    public Task<TrainingContractsModel> GetForecastAsync(int id)
        //    {
        //        if (id > 5)
        //            return Task.FromResult<TrainingContractsModel>(null);
        //        return Task.FromResult(new TrainingContractsModel() {Date = DateTime.Now, Summary = "TestForecast", TemperatureC = 50});
        //    }

        //    public Task<IEnumerable<TrainingContractsModel>> GetForecastAsync()
        //    {
        //        return Task.FromResult(new List<TrainingContractsModel>() {new TrainingContractsModel() {Date = DateTime.Now, Summary = "TestForecast", TemperatureC = 50}}.AsEnumerable());
        //    }

        //    public Task<IEnumerable<WeatherWarningModel>> GetWarningsAsync()
        //    {
        //        return Task.FromResult(new List<WeatherWarningModel>() {new WeatherWarningModel() {Date = DateTime.Now, Warning = "TestWarning"}}.AsEnumerable());
        //    }
        //}
    }
}