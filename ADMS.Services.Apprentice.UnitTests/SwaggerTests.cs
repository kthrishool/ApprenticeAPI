using ADMS.Services.Apprentice.WebApi;
using Employment.Services.Infrastructure.UnitTesting;
using Microsoft.OpenApi.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Employment.Services.Adms.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SwaggerTests
    {
        private const string AcceptHeader = "Accept";
        private const string Header = "application/json";
        private static string CurrentSwagger = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testcontext)
        {
            const string uri = "swagger/docs/v1";
            using (var server = TestHelper.CreateTestServer<Startup>())
            {
                var client = TestHelper.CreateTestClient(server);
                client.DefaultRequestHeaders.Add(AcceptHeader, Header);

                var task = client.GetAsync(uri);
                var response = task.Result.Content.ReadAsStringAsync();
                CurrentSwagger = response.Result;
            }
        }

        private void checkExists(string url, string paramNames)
        {
            var reader = new Microsoft.OpenApi.Readers.OpenApiStringReader(new OpenApiReaderSettings());
            var document = reader.Read(CurrentSwagger, out OpenApiDiagnostic diagnostic);
            //var returned = JsonConvert.DeserializeObject<WrappedHttpResponseBody<IEnumerable<DiscoveryResponseV1>>>(CurrentSwagger);
            bool found = false;
            foreach (var a in document.Paths)
            {
                string parameternames = a.Value.Operations.Aggregate(string.Empty, (current, p) => current + (p.Key + ","));
                parameternames = parameternames.Trim(',');
                if (a.Key == url && parameternames == paramNames)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "Could not find method "+url+" "+paramNames);
        }
        [TestMethod] public void Swagger_Exists__api_v1_BuildInformation_Get(){ checkExists("/api/v1/BuildInformation", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_CurrentUserClaims_Get(){ checkExists("/api/v1/CurrentUserClaims", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_Discovery_Get(){ checkExists("/api/v1/Discovery", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_HealthCheck_Get(){ checkExists("/api/v1/HealthCheck", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_HealthCheck_Diagnostics_Get(){ checkExists("/api/v1/HealthCheck/Diagnostics", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_Ping_Get(){ checkExists("/api/v1/Ping", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v2_Ping_Get(){ checkExists("/api/v2/Ping", "Get"); }
        [TestMethod] public void Swagger_Exists__api_v1_RelatedCodes_Get(){ checkExists("/api/v1/RelatedCodes", "Get"); }






        /// <summary>
        /// The console output of this test will generate the ones above.
        /// </summary>
        [TestMethod]
        public void GetSwagger_OK()
        {
            const string uri = "swagger/docs/v1";
            using (var server = TestHelper.CreateTestServer<Startup>())
            {
                var client = TestHelper.CreateTestClient(server);
                client.DefaultRequestHeaders.Add(AcceptHeader, Header);

                var task = client.GetAsync(uri);
                var response = task.Result.Content.ReadAsStringAsync().Result;

                //var returned = JsonConvert.DeserializeObject<WrappedHttpResponseBody<IEnumerable<DiscoveryResponseV1>>>(response.Result);
                var reader = new Microsoft.OpenApi.Readers.OpenApiStringReader(new OpenApiReaderSettings());
                var document = reader.Read(response, out OpenApiDiagnostic diagnostic);
                Assert.IsTrue(document != null, "document != null");
                Assert.IsTrue(document.Info.Version == "v1", "document.Info.Version=='v1'; actual:" + document.Info.Version);
                //Assert.IsTrue(document.Info.Title == "Employment Infrastructure API", "document.Info.Title == 'Employment Infrastructure API'; actual:" + document.Info.Title);
                Assert.IsTrue(document.Paths.Count > 0, "document.Paths.Count>0; actual:" + document.Paths.Count);


                //Debug.WriteLine("Version, Method, URL, Parameters, ReturnValues");
                foreach (var a in document.Paths)
                {
                    //Debug.WriteLine(""+a.Url);
                    string parameternames = a.Value.Operations.Aggregate(string.Empty, (current, p) => current + (p.Key + ","));

                   // Debug.WriteLine("Params - Version: {0}, Method: {1}, Url: {2}, Parameters Aggregate: {3}", a.Version, a.Method, a.Url, parameternames.TrimEnd(','));

                    Console.WriteLine("[TestMethod] public void Swagger_Exists_{2}_{3}(){{ checkExists(\"{0}\", \"{1}\"); }}", a.Key, parameternames.TrimEnd(','), a.Key.Replace("/","_").Replace(" ","__").Replace("(", "Ɨ").Replace(")", "Ɨ").Replace("{", "Ɨ").Replace("}", "Ɨ"), parameternames.Replace(" ","__").TrimEnd(',').Replace(",", "ƶ").Replace("(", "Ɨ").Replace(")", "Ɨ"));

                    //foreach (var b in a.Params)
                    //{
                    //    Debug.WriteLine("  Outer list - Name: {0}, Type: {1}", b.Name, b.Type);
                    //}

                    //foreach (var e in a.ReturnValues)
                    //{
                    //    foreach (var f in e.Params)
                    //    {
                    //        string returnname = f.Name;
                    //        string returntype = f.Type;

                    //        //(string.Empty,
                    //        //(current, r) => current + (r. + ","));
                    //        //string returnvalues = d.ReturnValues.Aggregate(string.Empty,(current, r) => current + (r.Params + ","));
                    //        Debug.WriteLine("Outer list  - Name: {0}, Type: {1}", returnname, returntype);

                    //        foreach (var g in f.Params)
                    //        {
                    //            string returnname2 = g.Name;
                    //            string returntype2 = g.Type;

                    //            Debug.WriteLine("Inner list  - Name: {0}, Type: {1}", returnname2, returntype2);
                    //        }
                    //    }
                    //}
                    //Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", d.Version, d.Method, d.Url, parameternames.TrimEnd(','), returnvalues);
                }
            }

            Assert.IsTrue(true); // mark test passed
        }

    }
}
