using System;
using ADMS.Apprentices.Core.Messages;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Messages
{
    #region WhenCreatingProfileQualificationMessage

    [TestClass]
    public class WhenCreatingProfileQualificationMessage
    {
        private ProfilePriorQualificationMessage qualificationMessage;

        [TestMethod]
        public void NewQualificationMessage()
        {
            qualificationMessage = new ProfilePriorQualificationMessage
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