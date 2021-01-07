using ADMS.Services.Apprentice.Contract;
using ADMS.Services.Apprentice.WebApi;
using Employment.Infrastructure.Security.Contract;
using Employment.Services.Infrastructure.Core.Logging;
using Employment.Services.Infrastructure.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace ADMS.Services.Apprentice.UnitTests.Controller
{
    /// <summary> Unit tests for the RelatedCodesV1Controller class. </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RelatedCodesV1ControllerTests : ControllerUnitTestBase<Startup>
    {
        public RelatedCodesV1ControllerTests():base(true){}

        /// <summary>
        /// Create a dummy list of AuthorisationClaimMappings. The mock authoriser will use these
        /// to validate security on the API calls.
        /// </summary>
        /// <returns>A list of AuthorisationClaimMappings.</returns>
        public override List<AuthorisationClaimMapping> GetAuthorisationClaimMappings()
        {
            var mappings = new List<AuthorisationClaimMapping>();
            //mappings.Add(new AuthorisationClaimMapping() {ActivityName = "JCA-AssessmentCreateUpdate",ClaimSchema = "http://deewr.gov.au/es/2011/03/claims/baserole", ClaimValue = "DAD"});
            return mappings;
        }

        /// <summary> Call RelatedCodes GetRelatedCodes as a DEPT user. </summary>
        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_UserDept()
        {
            base.SetupPrincipal("test", "DEPT", "USRS", "UserType_DEWR", "DAD", new[] {"ADA"});

            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet< List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=ORGF&SearchCode=AAAB", 1, out response);

            Assert.IsTrue(results.Code == 200);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "CP20").Count() == 1);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "NHMZ").Count() == 1);
        }



        /// <summary> Call RelatedCodes GetRelatedCodes as an anonymous user. </summary>
        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_UserAnon()
        {
            base.SetupEmptyPrincipal();

            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet<List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=ORGF&SearchCode=AAAB", 1, out response);

            Assert.IsTrue(results.Code == 200);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "CP20").Count() == 1);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "NHMZ").Count() == 1);
        }


        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_NonExistentCode()
        {
            base.SetupPrincipal("test", "DEPT", "USRS", "UserType_DEWR", "DAD", new[] { "ADA" });

            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet<List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=PANTS&SearchCode=PANTS", 1, out response);

            Assert.IsTrue(results.Code == 200);
            Assert.IsTrue(results.Data.Count==0);
        }


        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_MissingCodeType()
        {
            base.SetupPrincipal("test", "DEPT", "USRS", "UserType_DEWR", "DAD", new[] { "ADA" });

            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet<List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=", 1, out response);

            Assert.IsTrue(results.Code == 400);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(content.IndexOf("One or more validation errors occurred.", StringComparison.Ordinal)>=0, "Unexpected response: "+ content);
        }

        /// <summary> Test that additional debug entries when LogLevel==LogLevel.Debug. </summary>
        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_LogDebug()
        {
            base.SetupPrincipal("test", "DEPT", "USRS", "UserType_DEWR", "DAD", new[] { "ADA" });
            base.LogLevel = LogLevel.Debug;
            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet<List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=ORGF&SearchCode=AAAB", 1, out response);

            Assert.IsTrue(results.Code == 200);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "CP20").Count() == 1);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "NHMZ").Count() == 1);
            Assert.IsTrue(base.LogItems.Count>0);
            Assert.IsTrue(base.LogContainsMessage("Request for RelatedCodeType: ORGF"));
        }
        /// <summary> Test that debug entries are not logged when LogLevel==LogLevel.Error. </summary>
        [TestMethod]
        public void RelatedCodesV1Controller_GetRelatedCodes_LogError()
        {
            base.SetupPrincipal("test", "DEPT", "USRS", "UserType_DEWR", "DAD", new[] { "ADA" });
            base.LogLevel = LogLevel.Error;
            string uri = "/api/RelatedCodes";
            HttpResponseMessage response;
            var results = base.ExecuteGet<List<RelatedCodeResponseV1>>(uri + "?RelatedCodeType=ORGF&SearchCode=AAAB", 1, out response);

            Assert.IsTrue(results.Code == 200);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "CP20").Count() == 1);
            Assert.IsTrue(results.Data.Where(r => r.SubordinateCode == "NHMZ").Count() == 1);
            Assert.IsFalse(base.LogContainsMessage("Request for CodeType: STT"));//in error mode this should not be logged
        }


    }
}
