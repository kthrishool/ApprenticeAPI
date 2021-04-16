using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    /// <summary>
    /// Address with detailed fields and boundary data
    /// </summary>
    /// <remarks>Most data is source from the G-NAF dataset. Up to date descriptions of the G-NAF dataset can be found here: https://www.psma.com.au/products/g-naf </remarks>
    public record DetailAddressModel(
        int AddressId,
        string BuildingName,
        string FormattedAddress,
        string StreetAddress,
        string StreetAddressLine1,
        string StreetAddressLine2,
        string StreetAddressLine3,
        string Locality,
        string State,
        string Postcode,
        string GeocodeType,
        decimal Latitude,
        decimal Longitude,
        string AddressSource,
        string AddressSourceId,
        int Confidence,
        int SubAddressCount,
        Boundary[] Boundaries,
        int Index
    );    
}
