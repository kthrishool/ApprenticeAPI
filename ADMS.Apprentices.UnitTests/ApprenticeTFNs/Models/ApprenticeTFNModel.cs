using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Models
{
    #region WhenCreatingApprenticeTFNModel
    [TestClass]
    public class WhenCreatingTFNModel
    {
        protected ApprenticeTFNModel model;
        protected ApprenticeTFN t;
        protected DateTime now = DateTime.Now;

        [TestMethod]
        public void NewEmptyApprenticeTFNModel()
        {
            model = new ApprenticeTFNModel();

            model.Id.Should().Be(0);
            model.ApprenticeId.Should().Be(0);
            model.TaxFileNumber.Should().Be(null);
            model.Status.Should().Be(TFNStatus.TBVE);
            model.StatusReason.Should().Be(null);
        }

        [TestMethod]
        public void NewApprenticeTFNModel()
        {
            t = new();
            t.Id = 1;
            t.ApprenticeId = 1;
            t.TaxFileNumber = "TaxFileNumber";
            t.StatusCode = TFNStatus.MTCH;
            t.StatusReasonCode = "StatusReason";

            model = new ApprenticeTFNModel(t);

            model.Id.Should().Be(t.Id);
            model.ApprenticeId.Should().Be(t.ApprenticeId);
            model.TaxFileNumber.Should().Be(t.TaxFileNumber);
            model.Status.Should().Be(t.StatusCode);
            model.StatusReason.Should().Be(t.StatusReasonCode);
        }


    }

    #endregion

}