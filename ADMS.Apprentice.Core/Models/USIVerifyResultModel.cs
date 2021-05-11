using ADMS.Apprentice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Models
{
    //TODO: implement unit testing
    [ExcludeFromCodeCoverage]
    public class USIVerifyResultModel
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
        public bool? UsiVefify { get; set; }
        public USIVerifyResultModel(ApprenticeUSI apprenticeUSI)
        {
            this.ApprenticeId = apprenticeUSI.ApprenticeId;
            this.USI = apprenticeUSI.USI;
            this.USIStatus = apprenticeUSI.USIStatus;
            this.FirstNameMatched = apprenticeUSI.FirstNameMatchedFlag;
            this.SurnameMatched = apprenticeUSI.SurnameMatchedFlag;
            this.DateOfBirthMatched = apprenticeUSI.DateOfBirthMatchedFlag;
            this.UsiVefify = apprenticeUSI.USIVerifyFlag;
        }
    }    
}
