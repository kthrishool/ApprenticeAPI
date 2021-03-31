using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    /// <summary>
    /// A partial address including Locality Postcode details
    /// </summary>
    /// <remarks>Most data is source from the G-NAF dataset. Up to date descriptions of the G-NAF dataset can be found here: https://www.psma.com.au/products/g-naf </remarks>
    public record PartialAddressModel(
        string Locality, 
        string State,
        string Postcode,
        decimal Latitude,
        decimal Longitude,
        Boundary[] Boundaries        
    );
}