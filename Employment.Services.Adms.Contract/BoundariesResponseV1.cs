using Employment.Services.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Employment.Services.ReferenceData.Contract
{
    /// <summary>
    /// Collection of boundary data
    /// </summary>
    [DataContract]
    public class BoundariesResponseV1 : ContractBase
    {
        /// <remarks />
        public BoundariesResponseV1():base(1)
        {

        }

        /// <summary>
        /// A collection of matched boundaries based on the requested set of boundary properties requested. 
        /// </summary>
        [DataMember]
        public List<BoundaryV1> Boundaries { get; set; }
    }
}
