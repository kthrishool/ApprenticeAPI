using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using Adms.Shared.Database;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Entities
{
    #region ApprenticeTFN
    [TestClass]
    public class WhenCreatingApprenticeTFN 
    {        
        private ApprenticeTFN tfnDetail;

        [TestMethod]
        public void NewApprenticeTFN()
        {
            tfnDetail= new ApprenticeTFN();
            var dt = DateTime.Now;
            var g = new Guid();
            var b = new byte[]{ 1 };

            tfnDetail.ApprenticeId = 1;
            tfnDetail.TaxFileNumber = "1";
            tfnDetail.StatusCode = TFNStatus.TBVE;
            tfnDetail.StatusDate = dt;
            tfnDetail.StatusReasonCode = "NEW";
            tfnDetail.MessageQueueCorrelationId = g;
            tfnDetail.CreatedOn = dt;
            tfnDetail.CreatedBy = "1";
            tfnDetail.UpdatedOn = dt;
            tfnDetail.UpdatedBy = "1";
            tfnDetail.Version = b;
            tfnDetail.AuditEventId = 1;

            tfnDetail.ApprenticeId.Should().Be(1);
            tfnDetail.TaxFileNumber.Should().Be("1");
            tfnDetail.StatusCode.Should().Be(TFNStatus.TBVE);
            tfnDetail.StatusDate.Should().Be(dt);
            tfnDetail.StatusReasonCode.Should().Be("NEW");
            tfnDetail.MessageQueueCorrelationId.Should().Be(g);
            tfnDetail.CreatedOn.Should().Be(dt);
            tfnDetail.CreatedBy.Should().Be("1");
            tfnDetail.UpdatedOn.Should().Be(dt);
            tfnDetail.UpdatedBy.Should().Be("1");
            tfnDetail.Version.Should().NotBeEmpty();
            tfnDetail.AuditEventId.Should().Be(1);
        }


    }

    #endregion
}