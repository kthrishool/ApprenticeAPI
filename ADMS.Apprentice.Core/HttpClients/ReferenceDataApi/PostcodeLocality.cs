namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    public class PostcodeLocality
    {
        public string Postcode { get; set; }

        /// <summary>
        /// Locality code
        /// </summary>

        public string LocalityCode { get; set; }

        /// <summary>
        /// Short description
        /// </summary>

        public string ShortDescription { get; set; }

        /// <summary>
        /// Long description
        /// </summary>

        public string LongDescription { get; set; }

        /// <summary>
        /// State code
        /// </summary>

        public string StateCode { get; set; }
    }
}