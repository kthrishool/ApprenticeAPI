using ADMS.Apprentices.Core.Messages.TFN;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Messages
{
    #region WhenCreatingApprenticeTFNV1
    [TestClass]
    public class WhenCreatingApprenticeTFNV1
    {
        private ApprenticeTFNV1 tfnDetail;

        [TestMethod]
        public void NewApprenticeTFN()
        {
            tfnDetail = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = 1
            };

            tfnDetail.ApprenticeId.Should().Be(1);
            tfnDetail.TaxFileNumber.Should().Be(1);
        }


    }

    #endregion
}