using System.Diagnostics.CodeAnalysis;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public class ProfileUSIModel
    {
        public int ApprenticeId { get; set; }

        public string USI { get; set; }

        /// <summary>
        /// Valid, Invalid, or Deactivated
        /// </summary>
        public string USIStatus { get; set; }

        public bool? FirstNameMatched { get; set; }

        public bool? SurnameMatched { get; set; }

        public bool? DateOfBirthMatched { get; set; }
        public bool? UsiVerify { get; set; }

        public ProfileUSIModel(ApprenticeUSI apprenticeUSI)
        {
            this.ApprenticeId = apprenticeUSI.ApprenticeId;
            this.USI = apprenticeUSI.USI;
            this.USIStatus = apprenticeUSI.USIStatus;
            this.FirstNameMatched = apprenticeUSI.FirstNameMatchedFlag;
            this.SurnameMatched = apprenticeUSI.SurnameMatchedFlag;
            this.DateOfBirthMatched = apprenticeUSI.DateOfBirthMatchedFlag;
            this.UsiVerify = apprenticeUSI.USIVerifyFlag;
        }
    }
}