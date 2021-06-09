﻿using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Messages.TFN;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ADMS.Apprentice.UnitTests.Profiles.Messages
{
    #region WhenCreatingProfileQualificationMessage
    [TestClass]
    public class WhenCreatingProfileQualificationMessage
    { 
        private ProfileQualificationMessage qualificationMessage;

        [TestMethod]
        public void NewQualificationMessage()
        {
            qualificationMessage = new ProfileQualificationMessage
            {
                QualificationCode = "QCODE",
                QualificationDescription = "QDESCRIPTION",
                QualificationLevel = "QLEVEL",
                QualificationANZSCOCode = "QANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1)
            };

            qualificationMessage.QualificationCode.Should().Be("QCODE");
            qualificationMessage.QualificationDescription.Should().Be("QDESCRIPTION");
            qualificationMessage.QualificationLevel.Should().Be("QLEVEL");
            qualificationMessage.QualificationANZSCOCode.Should().Be("QANZS");
            qualificationMessage.StartDate.Should().Be(new DateTime(2010, 1, 1));
            qualificationMessage.EndDate.Should().Be(new DateTime(2020, 1, 1));
        }
    }

    #endregion
}