using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.HttpClients.USI
{
    public class VerifyUsiModel
    {
        public int? RecordId { get; set; }

        public string USI { get; set; }

        /// <summary>
        /// Valid, Invalid, or Deactivated
        /// </summary>
        public string USIStatus { get; set; }

        public bool? FirstNameMatched { get; set; }

        public bool? FamilyNameMatched { get; set; }

        public bool? SingleNameMatched { get; set; }

        public bool? DateOfBirthMatched { get; set; }
    }
}
