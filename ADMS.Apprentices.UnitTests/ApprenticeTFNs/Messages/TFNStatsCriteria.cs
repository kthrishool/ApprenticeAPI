using ADMS.Apprentices.Core.Messages.TFN;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Messages
{
    #region WhenCreatingApprenticeTFNV1
    [TestClass]
    public class WhenCreatingTFNStatsCriteria
    {
        private TFNStatsCriteria message;

        [TestMethod]
        public void NewApprenticeTFN()
        {
            message = new TFNStatsCriteria
            {
                Keyword = "Keyword",
                StatusCode = "StatusCode"
            };

            message.Keyword.Should().Be("Keyword");
            message.StatusCode.Should().Be("StatusCode");
        }


    }

    #endregion

}