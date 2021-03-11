using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileListModel

    [TestClass]
    public class WhenInstantiatingAProfileListModel : GivenWhenThen
    {
        private ProfileListModel model;
        private Profile profile;

        protected override void Given()
        {
            profile = new Profile
            {
                Id = 123,
                FirstName = "Bob",
                Surname = "Alex",
                OtherNames = "Charlie",
                BirthDate = DateTime.Today.AddYears(-25),
                //GenderCode = "X",
                //ProfileTypeCode = "Apprentice",
                CreatedOn = DateTime.Now.AddMinutes(-3),            
                CreatedBy = "User1",
                UpdatedOn = DateTime.Now,
                UpdatedBy = "User2",
            };
        }

        protected override void When()
        {
            model = new ProfileListModel(profile);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.Id.Should().Be(123);
            model.FirstName.Should().Be("Bob");
            model.Surname.Should().Be("Alex");
            model.OtherNames.Should().Be("Charlie");
            model.BirthDate.Should().BeCloseTo(DateTime.Today.AddYears(-25));
            model.CreatedOn.Should().BeCloseTo(DateTime.Now.AddMinutes(-3));            
            model.UpdatedOn.Should().BeCloseTo(DateTime.Now);
            model.CreatedBy.Should().Be("User1");
            model.UpdatedBy.Should().Be("User2");
        }
    }
    #endregion
}
