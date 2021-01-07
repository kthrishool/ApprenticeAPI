using Employment.Services.Adms.AspNetCore.WebApi;
using Employment.Services.Infrastructure.Contract;
using Employment.Services.Infrastructure.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Employment.Services.Adms.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DiscoveryTests
    {
        private const string AcceptHeader = "Accept";
        private const string Header = "application/json";
        private static string CurrentDiscovery = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testcontext)
        {
            const string uri = "api/v1/Discovery";
            using (var server = TestHelper.CreateTestServer<Startup>())
            {
                var client = TestHelper.CreateTestClient(server);
                client.DefaultRequestHeaders.Add(AcceptHeader, Header);

                var task = client.GetAsync(uri);
                var response = task.Result.Content.ReadAsStringAsync();
                CurrentDiscovery = response.Result;
            }
        }

        private void checkExists(int version, string method, string url, string paramNames)
        {
            var returned = JsonConvert.DeserializeObject<WrappedHttpResponseBody<IEnumerable<DiscoveryResponseV1>>>(CurrentDiscovery);
            var versionstring = "" + version;
            bool found = false;
            foreach (DiscoveryResponseV1 a in returned.Data)
            {
                string parameternames = a.Params.Aggregate(string.Empty, (current, p) => current + (p.Name + ","));
                parameternames = parameternames.Trim(',');
                if (a.Version == versionstring && a.Method == method && a.Url==url&& parameternames == paramNames)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "Could not find method V"+version+" "+method+" "+url+" "+paramNames);
        }

        [TestMethod] public void Discovery_Exists_V2_GET_api_Ping_() { checkExists(2, "GET", "api/Ping", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_CurrentUserClaims_() { checkExists(1, "GET", "api/CurrentUserClaims", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_Discovery_() { checkExists(1, "GET", "api/Discovery", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_BuildInformation_() { checkExists(1, "GET", "api/BuildInformation", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_HealthCheck_() { checkExists(1, "GET", "api/HealthCheck", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_HealthCheck_Diagnostics_() { checkExists(1, "GET", "api/HealthCheck/Diagnostics", ""); }
        [TestMethod] public void Discovery_Exists_V1_GET_api_RelatedCodes_RelatedCodeTypeƶSearchCodeƶDominantSearchƶCurrentCodesOnlyƶExactLookupƶMaxRowsƶRowPositionƶCurrentDateƶEndDateInclusive() { checkExists(1, "GET", "api/RelatedCodes", "RelatedCodeType,SearchCode,DominantSearch,CurrentCodesOnly,ExactLookup,MaxRows,RowPosition,CurrentDate,EndDateInclusive"); }



        /// <summary>
        /// The console output of this test will generate the ones above.
        /// </summary>
        [TestMethod]
        public void GetDiscovery_OK()
        {
            const string uri = "api/v1/Discovery";
            using (var server = TestHelper.CreateTestServer<Startup>())
            {
                var client = TestHelper.CreateTestClient(server);
                client.DefaultRequestHeaders.Add(AcceptHeader, Header);

                var task = client.GetAsync(uri);
                var response = task.Result.Content.ReadAsStringAsync();

                var returned = JsonConvert.DeserializeObject<WrappedHttpResponseBody<IEnumerable<DiscoveryResponseV1>>>(response.Result);

                Debug.WriteLine("Version, Method, URL, Parameters, ReturnValues");
                foreach (DiscoveryResponseV1 a in returned.Data)
                {
                    //Debug.WriteLine(""+a.Url);
                    string parameternames = a.Params.Aggregate(string.Empty, (current, p) => current + (p.Name + ","));

                   // Debug.WriteLine("Params - Version: {0}, Method: {1}, Url: {2}, Parameters Aggregate: {3}", a.Version, a.Method, a.Url, parameternames.TrimEnd(','));

                    Console.WriteLine("[TestMethod] public void Discovery_Exists_V{0}_{1}_{4}_{5}(){{ checkExists({0}, \"{1}\", \"{2}\", \"{3}\"); }}", a.Version, a.Method, a.Url, parameternames.TrimEnd(','), a.Url.Replace("/","_").Replace("{", "Ɨ").Replace("}", "Ɨ"), parameternames.TrimEnd(',').Replace(",", "ƶ"));

                    foreach (var b in a.Params)
                    {
                        Debug.WriteLine("  Outer list - Name: {0}, Type: {1}", b.Name, b.Type);
                    }

                    foreach (var e in a.ReturnValues)
                    {
                        foreach (var f in e.Params)
                        {
                            string returnname = f.Name;
                            string returntype = f.Type;

                            //(string.Empty,
                            //(current, r) => current + (r. + ","));
                            //string returnvalues = d.ReturnValues.Aggregate(string.Empty,(current, r) => current + (r.Params + ","));
                            Debug.WriteLine("Outer list  - Name: {0}, Type: {1}", returnname, returntype);

                            foreach (var g in f.Params)
                            {
                                string returnname2 = g.Name;
                                string returntype2 = g.Type;

                                Debug.WriteLine("Inner list  - Name: {0}, Type: {1}", returnname2, returntype2);
                            }
                        }
                    }
                    //Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", d.Version, d.Method, d.Url, parameternames.TrimEnd(','), returnvalues);
                }
            }

            Assert.IsTrue(true); // mark test passed
        }

    }
}
