using System;

namespace ADMS.Services.Apprentice.Model
{
    /// <summary>
    /// Related ADW code
    /// </summary>
    public class RelatedCode
    {
        /// <summary>
        /// Related ADW code
        /// </summary>
        public string RelatedCodeValue { get; set; }

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

        private string _subordinateDescription;
        /// <summary>
        /// Subordinate long description
        /// </summary>
        public string SubordinateDescription {
            get { return _subordinateDescription?.TrimEnd(); }
            set { _subordinateDescription = value; } 
        }

        private string _subordinateShortDescription;
         /// <summary>
        /// Subordinate short description
        /// </summary>
       public string SubordinateShortDescription {
            get { return _subordinateShortDescription?.TrimEnd(); }
            set { _subordinateShortDescription = value; }
        }

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
