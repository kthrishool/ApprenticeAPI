using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ADMS.Apprentice.Api.Controllers.Tfn;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile
    [TestClass]
    public class WhenCreatingTfgnDetailUsingTheApi: GivenWhenThen<TfnDetailController>
    {
        private TfnDetail profile;
        private ActionResult<TfnDetailModel> result;
        private TfnCreateMessage message;       
        protected override void Given()
        {
            message = new TfnCreateMessage
            {
                ApprenticeId =1,
                TaxFileNumber = "123456789"
            };

            profile = new TfnDetail
            {
                Id = 1,
                ApprenticeId = 1,
                TFN = "123456789"
            };

            Container
               .GetMock<ITfnDetailCreator>()
               .Setup(r => r.CreateTfnDetailAsync(message))
               .Returns(Task.FromResult(profile));

        }

        protected override async void When()
        {             
            result = await ClassUnderTest.Create(message);
        }

        [TestMethod]
        public void ShouldReturnResult()
        {
            result.Should().NotBeNull();
        }
    }

    #endregion
}