using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Entities
{
    public interface IAddressAttributes
    {
         string StreetAddress1 { get; set; }
         string StreetAddress2 { get; set; }

         string StreetAddress3 { get; set; }
         string Locality { get; set; }

         string StateCode { get; set; }
         string Postcode { get; set; }
         string SingleLineAddress { get; set; }

          string GeocodeType { get; set; }

          decimal? Latitude { get; set; }
          decimal? Longitude { get; set; }
          short? Confidence { get; set; }
    }
}
