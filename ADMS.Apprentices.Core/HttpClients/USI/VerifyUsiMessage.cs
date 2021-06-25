using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.HttpClients.USI
{
    public class VerifyUsiMessage
    {
        public int RecordId { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string USI { get; set; }
    }
}
