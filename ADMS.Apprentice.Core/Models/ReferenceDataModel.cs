using System;

namespace ADMS.Apprentice.Core.Models
{
    public class ReferenceDataModel
    {
        /// <summary>
        /// Adw code
        /// </summary>

        public string Code { get; set; }

        /// <summary>
        /// Long description
        /// </summary>

        public string Description { get; set; }

        /// <summary>
        /// Short description
        /// </summary>

        public string ShortDescription { get; set; }

        /// <summary>
        /// Currency Start date time
        /// </summary>

        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Currency end date time
        /// </summary>

        public DateTime? EndDate { get; set; }
    }
}