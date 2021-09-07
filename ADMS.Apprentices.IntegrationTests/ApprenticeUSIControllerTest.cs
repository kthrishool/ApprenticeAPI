using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ADMS.Apprentices.Api.Controllers;
using Adms.Shared.Authorisation;
using Au.Gov.Infrastructure.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.IntegrationTests
{
    /// <summary> Run controller tests with the internal infrastructure. </summary>
    [TestClass]
    public class ApprenticeUSIControllerTest : IDisposable
    {
        public Type ControllerType { get; }
        public string Url { get; }
        public BaseInternalFixture Fixture;

        public ApprenticeUSIControllerTest()
        {
            Fixture = new ApprenticesFixture();
            ControllerType = typeof(ApprenticeUSIController);
            Url = "v1/apprentices";
        }

        /// <summary> A valid departmental user with DAD access. </summary>
        public readonly List<KeyValuePair<string, string>> UserValid = new List<KeyValuePair<string, string>>()
        {
            new("http://dese.gov.au/adms/claims/org", "DEPT"),
            new("Name", "userid1"),
            new("http://dese.gov.au/adms/claims/baserole", "DAPD")
        };

        /// <summary> A user who has no valid roles. </summary>
        public readonly List<KeyValuePair<string, string>> UserInvalid = new List<KeyValuePair<string, string>>()
        {
            new("http://dese.gov.au/adms/claims/org", "0000"),
            new("Name", "userid2"),
            new("http://dese.gov.au/adms/claims/baserole", "0000")
        };

        //TODO: This breaks on an entity framework error
        ///// <summary>
        ///// Send in a valid DAD user JWT should return the correct results.
        ///// </summary>
        //[TestMethod]
        //public async Task Get_ValidUser_ReturnsAllItems()
        //{
        //    var req = new HttpRequestMessage(HttpMethod.Get, Url);
        //    req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValid(UserValid));
        //    var rep = await Fixture.Client.SendAsync(req);
        //    var content = await rep.Content.ReadAsStringAsync();
        //    Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 200, "Expected 200 Actual " + rep.StatusCode);
        //    var items = Fixture.Deserialize<PagedList<ProfileListModel>>(content);
        //    Assert.IsTrue(items?.Results != null);
        //    Assert.IsTrue(items.Results.Length == 1);
        //}


        /// <summary>
        /// Send in a valid JWT but with invalid roles.
        /// </summary>
        [TestMethod]
        public async Task Get_InvalidRoles_ReturnsForbidden()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValid(UserInvalid));
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 403, "Expected 403 Actual " + rep.StatusCode);
        }

        /// <summary>
        /// Send in a DAD JWT but with the wrong audience.
        /// </summary>
        [TestMethod]
        public async Task Get_InvalidAudienceJwt_ReturnsUnauthorised()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtInvalidAudience(UserValid));
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 401, "Expected 401 Actual " + rep.StatusCode);
        }

        /// <summary>
        /// Send in a request with no auth header.
        /// </summary>
        [TestMethod]
        public async Task Get_NoAuthHeader_ReturnsUnauthorised()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 401, "Expected 401 Actual " + rep.StatusCode);
        }

        /// <summary>
        /// Send in a request with no auth header.
        /// </summary>
        [TestMethod]
        public async Task Get_NonsenseAuthHeader_ReturnsUnauthorised()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            req.Headers.Add("Authorization", "Bearer " + "flibble");
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 401, "Expected 401 Actual " + rep.StatusCode);
        }

        /// <summary>
        /// Send in a DAD JWT but with the wrong issuer.
        /// </summary>
        [TestMethod]
        public async Task Get_InvalidIssuerJwt_ReturnsUnauthorised()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtInvalidIssuer(UserValid));
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 401, "Expected 401 Actual " + rep.StatusCode);
        }

        /// <summary>
        /// Send in a valid DAD user JWT which has expired.
        /// </summary>
        [TestMethod]
        public async Task Get_ExpiredJwt_ReturnsUnauthorised()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, Url);
            req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValidExpired(UserValid));
            var rep = await Fixture.Client.SendAsync(req);
            var content = await rep.Content.ReadAsStringAsync();
            Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 401, "Expected 401 Actual " + rep.StatusCode);
        }


        ///// <summary> OpenApi - Get the json output and validate that our controller is present. </summary>
        //[TestMethod]
        //public async Task GetOpenApiJson_ReturnsController()
        //{
        //    // get the openapi.json (swagger was renamed to openapi)
        //    var req = new HttpRequestMessage(HttpMethod.Get, "openapi.json");
        //    req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValid(UserValid)); //jwt is only mandatory for openapi output on External apis
        //    var rep = await Fixture.Client.SendAsync(req);
        //    var content = await rep.Content.ReadAsStringAsync();
        //    Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 200, "Expected 200 Actual " + rep.StatusCode);

        //    // parse the swagger into an openapi document and check our controller is in there
        //    var reader = new OpenApiStringReader(new OpenApiReaderSettings());
        //    var document = reader.Read(content, out _);
        //    Assert.IsTrue(document != null, "document != null");
        //    Assert.IsTrue(document.Paths.Count > 0, "document.Paths.Count>0; actual:" + document.Paths.Count);
        //    Assert.IsTrue(document.Paths.ContainsKey("/" + Url), "Path " + Url + " not found in openapi output.");
        //    foreach (var methodInfo in ControllerType.GetMethods(BindingFlags.Public))
        //    {
        //        if (methodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Get), "Path " + Url + " did not contain operation Get in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Post), "Path " + Url + " did not contain operation Post in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Put), "Path " + Url + " did not contain operation Put in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Delete), "Path " + Url + " did not contain operation Delete in openapi output.");
        //    }
        //}

        ///// <summary> OpenApi - Get the yaml output and validate that our controller is present. </summary>
        //[TestMethod]
        //public async Task GetOpenApiYaml_ReturnsController()
        //{
        //    // get the openapi.json (swagger was renamed to openapi)
        //    var req = new HttpRequestMessage(HttpMethod.Get, "openapi.yaml");
        //    req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValid(UserValid)); //jwt is only mandatory for openapi output on External apis
        //    var rep = await Fixture.Client.SendAsync(req);
        //    var content = await rep.Content.ReadAsStringAsync();
        //    Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 200, "Expected 200 Actual " + rep.StatusCode);

        //    // parse the swagger into an openapi document and check our controller is in there
        //    var reader = new OpenApiStringReader(new OpenApiReaderSettings());
        //    var document = reader.Read(content, out _);
        //    Assert.IsTrue(document != null, "document != null");
        //    Assert.IsTrue(document.Paths.Count > 0, "document.Paths.Count>0; actual:" + document.Paths.Count);
        //    Assert.IsTrue(document.Paths.ContainsKey("/" + Url), "Path " + Url + " not found in openapi output.");
        //    foreach (var methodInfo in ControllerType.GetMethods(BindingFlags.Public))
        //    {
        //        if (methodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Get), "Path " + Url + " did not contain operation Get in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Post), "Path " + Url + " did not contain operation Post in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Put), "Path " + Url + " did not contain operation Put in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() != null)
        //            Assert.IsTrue(document.Paths["/" + Url].Operations.ContainsKey(OperationType.Delete), "Path " + Url + " did not contain operation Delete in openapi output.");
        //    }
        //}

        ///// <summary> OpenApi - Get the refit proxy output and validate that our controller is present. </summary>
        //[TestMethod]
        //public async Task GetOpenApiRefit_ReturnsController()
        //{
        //    // get the openapi.json (swagger was renamed to openapi)
        //    var req = new HttpRequestMessage(HttpMethod.Get, "openapi.refit");
        //    req.Headers.Add("Authorization", "Bearer " + Fixture.GetJwtValid(UserValid)); //jwt is only mandatory for openapi output on External apis
        //    var rep = await Fixture.Client.SendAsync(req);
        //    var content = await rep.Content.ReadAsStringAsync();
        //    Assert.IsTrue(Convert.ToInt32(rep.StatusCode) == 200, "Expected 200 Actual " + rep.StatusCode);

        //    // note the refit proxy is C# so we can't use Microsoft.OpenApi - but let's check if our paths are in there
        //    Assert.IsTrue(content != null, "content != null");
        //    foreach (var methodInfo in ControllerType.GetMethods(BindingFlags.Public))
        //    {
        //        if (methodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
        //            Assert.IsTrue(content.Contains("[Refit.Get(\"/" + Url + "\")]"), "Path " + Url + " not found in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
        //            Assert.IsTrue(content.Contains("[Refit.Post(\"/" + Url + "\")]"), "Path " + Url + " not found in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
        //            Assert.IsTrue(content.Contains("[Refit.Put(\"/" + Url + "\")]"), "Path " + Url + " not found in openapi output.");
        //        else if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() != null)
        //            Assert.IsTrue(content.Contains("[Refit.Delete(\"/" + Url + "\")]"), "Path " + Url + " not found in openapi output.");
        //    }
        //}

        public void Dispose()
        {
            Fixture.Dispose();
        }
    }
}