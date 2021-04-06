using System;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    public class LocalityPostCodeModel
    {
        /// <summary>
        /// Related ADW code
        /// </summary>
        public string RelatedCode { get; set; }

        /// <summary>
        /// Whether the code was retrieved with a dominant search.
        /// </summary>
        public bool Dominant { get; set; }

        /// <summary>
        /// Dominant code
        /// </summary>
        public string DominantCode { get; set; }

        /// <summary>
        /// Dominant long description
        /// </summary>
        public string DominantDescription { get; set; }

        /// <summary>
        /// Dominant short description
        /// </summary>
        public string DominantShortDescription { get; set; }

        /// <summary>
        /// Subordinate code
        /// </summary>
        public string SubordinateCode { get; set; }

        /// <summary>
        /// Subordinate long description
        /// </summary>
        public string SubordinateDescription { get; set; }

        /// <summary>
        /// Subordinate short description
        /// </summary>
        public string SubordinateShortDescription { get; set; }

        /// <summary>
        /// Currency Start date time
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Currency end date time
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public int Position { get; set; }
    }
}