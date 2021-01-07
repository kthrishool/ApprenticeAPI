using Employment.Services.ReferenceData.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Employment.Infrastructure.Security.Contract;
using Employment.Services.Infrastructure.UnitTesting;

namespace Employment.Services.ReferenceData.UnitTests.Controller
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AutocompletePartialAddressV1ControllerTests: ControllerUnitTestBase<Startup>
    {

        /// <summary>
        /// Create a dummy list of AuthorisationClaimMappings. The mock authoriser will use these
        /// to validate security on the API calls.
        /// </summary>
        /// <returns>A list of AuthorisationClaimMappings.</returns>
        public override List<AuthorisationClaimMapping> GetAuthorisationClaimMappings()
        {
            // create authoriser mappings that are likely to be hit by the current tests
            var mappings = new List<AuthorisationClaimMapping>();
            mappings.Add(new AuthorisationClaimMapping() {ActivityName = "AAA-YourActivityName",ClaimSchema = "http://deewr.gov.au/es/2011/03/claims/baserole", ClaimValue = "DAD"});
            return mappings;
        }

        private AutocompletePartialAddressV1Controller CreateAutocompletePartialAddressV1Controller()
        {
            return new AutocompletePartialAddressV1Controller();
        }

        [TestMethod]
        public async Task AutocompletePartialAddress_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var autocompletePartialAddressV1Controller = this.CreateAutocompletePartialAddressV1Controller();
            string partialAddress = null;
            int maximumRows = 0;
            string context = null;
            int? fuzzyFactor = null;

            // Act
            var result = await autocompletePartialAddressV1Controller.AutocompletePartialAddress(
                partialAddress,
                maximumRows,
                context,
                fuzzyFactor);

            // Assert
            Assert.Fail();
        }
    }
}
