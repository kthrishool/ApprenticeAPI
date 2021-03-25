using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    /// <summary>
    /// A possible match from an Autocomplete Address Search
    /// </summary>
    /// <remarks>Most data is source from the G-NAF dataset. Up to date descriptions of the G-NAF dataset can be found here: https://www.psma.com.au/products/g-naf </remarks>
    public record AutocompleteAddressModel(
    
        /// <summary>
        /// The address identifier of the address
        /// </summary>
        /// <remarks>The address identifier is used within the iGas dataset as a short term primary key. Ongoing 
        /// maintenance, for example, updating a source dataset - can cause identifiers for the same physical address
        /// to change. As such the identifier should only be used for lookups occuring in relation to the same activity</remarks>
        
        int Id,

        /// <summary>
        /// The G-NAF Confidence rating
        /// </summary>
        
        int Confidence,

        /// <summary>
        /// A single line representation of the address formatted according to the format specifier 
        /// </summary>
        
        string SingleLineAddress,

        /// <summary>
        /// Any street related address informaiton including level, unit, and street if relevant
        /// </summary>
        
        string StreetAddress,

        /// <summary>
        /// A named geographical area defining a community or area of interest, which may be rural or urban in character.
        /// Usually known as a Suburb in an urban area. The localities used in G-NAF are the gazetted localities as provided by the respective jurisdictions
        /// </summary>
        
        string Locality,

        /// <summary>
        /// The two / three character abbriviated name of the state or territory
        /// </summary>
        
        string State,

        /// <summary>
        /// The postal Postcode as defined in AS:4590
        /// </summary>
        
        string Postcode
    );
}
