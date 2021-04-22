using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Messages.TFN;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                StartMonth = "JAN",
                StartYear = 2000,
                EndMonth = "DEC",
                EndYear = 2004,
            };

            qualificationMessage.QualificationCode.Should().Be("QCODE");
            qualificationMessage.QualificationDescription.Should().Be("QDESCRIPTION");
            qualificationMessage.QualificationLevel.Should().Be("QLEVEL");
            qualificationMessage.QualificationANZSCOCode.Should().Be("QANZS");
            qualificationMessage.StartMonth.Should().Be("JAN");
            qualificationMessage.StartYear.Should().Be(2000);
            qualificationMessage.EndMonth.Should().Be("DEC");
            qualificationMessage.EndYear.Should().Be(2004);
        }
    }

    #endregion
}