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
                StartMonth = "JAN",
                StartYear = "2000",
                EndMonth = "DEC",
                EndYear = "2004",
            };

            qualificationMessage.QualificationCode.Should().Be("QCODE");
            qualificationMessage.QualificationDescription.Should().Be("QDESCRIPTION");
            qualificationMessage.StartMonth.Should().Be("JAN");
            qualificationMessage.StartYear.Should().Be("2000");
            qualificationMessage.EndMonth.Should().Be("DEC");
            qualificationMessage.EndYear.Should().Be("2004");
        }
    }

    #endregion
}