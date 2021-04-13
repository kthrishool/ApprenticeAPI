using System;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.httpClients.ReferenceDataApi
{
    #region WhenInstantiatingAListCodeResponseV1

    [TestClass]
    public class WhenInstantiatingAListCodeResponseV1 : GivenWhenThen
    {
        private ListCodeResponseV1 model;


        protected override void Given()
        {
            model = new ListCodeResponseV1
            {
                ShortDescription = "test",
                StartDate = Convert.ToDateTime("01-01-2021"),
                Code = "01",
                Description = "test Desc",
                EndDate = Convert.ToDateTime("01-01-2021")
            };
        }


        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.ShortDescription.Should().Be("test");
            model.StartDate.Should().Be(Convert.ToDateTime("01-01-2021"));
            model.Code.Should().Be("01");
            model.Description.Should().Be("test Desc");
            model.EndDate.Should().BeCloseTo(Convert.ToDateTime("01-01-2021"));
        }
    }

    #endregion
}