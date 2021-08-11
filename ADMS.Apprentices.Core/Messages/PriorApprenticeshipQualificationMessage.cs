namespace ADMS.Apprentices.Core.Messages
{
    public record PriorApprenticeshipQualificationMessage : PriorQualificationMessage
    {
        public string StateCode { get; set; }


        public string CountryCode { get; set; }
    }
}