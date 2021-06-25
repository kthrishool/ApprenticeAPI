using System;
using ADMS.Apprentices.Core.Messages.TFN;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Messages
{
    #region WhenCreatingTFNStatsV1
    [TestClass]
    public class WhenCreatingApprenticeTFNStatsV1
    {
        private TFNStatsV1 message;
        protected DateTime now = DateTime.Now;

        [TestMethod]
        public void NewTFNStatsV1NullDates()
        {
            message = new TFNStatsV1(
                ApprenticeId:1,
                ApprenticeName:"ApprenticeName",
                now,
                now,
                null,
                "TfnVerificationStatus",
                "NumberOfDaysSinceTheMismatch"
                );

            message.ApprenticeId.Should().Be(1);
            message.ApprenticeName.Should().Be("ApprenticeName");
            message.DateOfBirth.Should().Be(now);
            message.TfnStatusDateTime.Should().Be(now);
            message.TfnCreatedDateTime.Should().Be(null);
            message.TfnVerificationStatus.Should().Be("TfnVerificationStatus");
            message.NumberOfDaysSinceTheMismatch.Should().Be("NumberOfDaysSinceTheMismatch");
        }

        [TestMethod]
        public void NewTFNStatsV1Dates()
        {
            message = new TFNStatsV1(
                1,
                "ApprenticeName",
                now,
                now,
                now,
                "TfnVerificationStatus",
                "NumberOfDaysSinceTheMismatch"
                );

            message.ApprenticeId.Should().Be(1);
            message.ApprenticeName.Should().Be("ApprenticeName");
            message.DateOfBirth.Should().Be(now);
            message.TfnStatusDateTime.Should().Be(now);
            message.TfnCreatedDateTime.Should().Be(now);
            message.TfnVerificationStatus.Should().Be("TfnVerificationStatus");
            message.NumberOfDaysSinceTheMismatch.Should().Be("NumberOfDaysSinceTheMismatch");
        }

        [TestMethod]
        public void SetTFNStatsV1()
        {
            message = new TFNStatsV1(
               1,
               "",
               now,
               now,
               null,
               "",
               ""
               );

            var newMessage = message with {
                ApprenticeId =  1,
                ApprenticeName  = "ApprenticeName",
                DateOfBirth = now,
                TfnStatusDateTime = now,
                TfnCreatedDateTime = now,
                TfnVerificationStatus = "TfnVerificationStatus",
                NumberOfDaysSinceTheMismatch = "NumberOfDaysSinceTheMismatch"
            };

            newMessage.ApprenticeId.Should().Be(1);
            newMessage.ApprenticeName.Should().Be("ApprenticeName");
            newMessage.DateOfBirth.Should().Be(now);
            newMessage.TfnStatusDateTime.Should().Be(now);
            newMessage.TfnCreatedDateTime.Should().Be(now);
            newMessage.TfnVerificationStatus.Should().Be("TfnVerificationStatus");
            newMessage.NumberOfDaysSinceTheMismatch.Should().Be("NumberOfDaysSinceTheMismatch");
        }
    }

    #endregion
}