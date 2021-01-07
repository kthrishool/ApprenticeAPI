using System;
using System.Runtime.Serialization;

namespace ADMS.Services.Apprentice.Contract
{
    /// <summary>
    /// Related ADW code
    /// </summary>
    [DataContract]
    public class RelatedCodeResponseV1
    {
        /// <summary>
        /// Related ADW code
        /// </summary>
        [DataMember]
        public string RelatedCode { get; set; }

        /// <summary>
        /// Whether the code was retrieved with a dominant search.
        /// </summary>
        [DataMember]
        public bool Dominant { get; set; }

        /// <summary>
        /// Dominant code
        /// </summary>
        [DataMember]
        public string DominantCode { get; set; }

        /// <summary>
        /// Dominant long description
        /// </summary>
        [DataMember]
        public string DominantDescription { get; set; }

        /// <summary>
        /// Dominant short description
        /// </summary>
        [DataMember]
        public string DominantShortDescription { get; set; }

        /// <summary>
        /// Subordinate code
        /// </summary>
        [DataMember]
        public string SubordinateCode { get; set; }

        /// <summary>
        /// Subordinate long description
        /// </summary>
        [DataMember]
        public string SubordinateDescription { get; set; }

        /// <summary>
        /// Subordinate short description
        /// </summary>
        [DataMember]
        public string SubordinateShortDescription { get; set; }

        /// <summary>
        /// Currency Start date time
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Currency end date time
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [DataMember]
        public int Position { get; set; }
    }
}
