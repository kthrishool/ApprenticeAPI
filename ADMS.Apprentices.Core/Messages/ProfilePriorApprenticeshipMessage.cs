namespace ADMS.Apprentices.Core.Messages
{
    public record ProfilePriorApprenticeshipMessage : ProfilePriorQualificationMessage
    {
        public string StateCode { get; set; }


        public string CountryCode { get; set; }
    }
}