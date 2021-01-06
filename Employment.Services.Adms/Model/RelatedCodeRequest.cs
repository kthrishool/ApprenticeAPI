using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employment.Services.Adms.Model
{
    /// <summary>
    /// Returns a list that contains all the related codes that meet the specified criteria.
    /// </summary>
    public class RelatedCodeRequest
    {
        /// <summary>
        /// The type of related code.
        /// </summary>
        public string RelatedCodeType { get; set; }
        /// <summary>
        /// The code to search for.
        /// </summary>
        public string SearchCode { get; set; }
        /// <summary>
        /// Whether it is a dominant search or not
        /// </summary>
        public bool DominantSearch { get; set; }
        /// <summary>
        /// Whether to use current codes only
        /// </summary>
        public bool CurrentCodesOnly { get; set; }
        /// <summary>
        /// Whether to do an exact lookup
        /// </summary>
        public bool ExactLookup { get; set; }
        /// <summary>
        /// The maximum number of rows to return (0 is all)
        /// </summary>
        public int MaxRows { get; set; }
        /// <summary>
        /// Unknown
        /// </summary>
        public int RowPosition { get; set; }
        /// <summary>
        /// Allows the current DtaeTime to be faked
        /// </summary>
        public DateTime? CurrentDate { get; set; }

        /// <summary>
        /// Whether to check current dates based on GEN logic (inclusive end date).
        /// </summary>
        public bool? EndDateInclusive { get; set; }
    }
}
