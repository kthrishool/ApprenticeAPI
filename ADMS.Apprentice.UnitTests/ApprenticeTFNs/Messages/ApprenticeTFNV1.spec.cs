using System;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using Adms.Shared.Database;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Messages
{
    #region WhenCreatingApprenticeTFNV1
    [TestClass]
    public class WhenCreatingApprenticeTFNV1 
    {        
        private ApprenticeTFNV1 tfnDetail;

        [TestMethod]
        public void NewApprenticeTFN()
        {
            tfnDetail= new ApprenticeTFNV1();

            tfnDetail.ApprenticeId = 1;
            tfnDetail.TaxFileNumber = 1;

            tfnDetail.ApprenticeId.Should().Be(1);
            tfnDetail.TaxFileNumber.Should().Be(1);
        }


    }

    #endregion
}