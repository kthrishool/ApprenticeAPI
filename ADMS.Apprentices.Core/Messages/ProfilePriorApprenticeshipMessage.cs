using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record ProfilePriorApprenticeshipMessage : ProfileQualificationMessage
    {

    public string StateCode { get; set; }


    public string CountryCode { get; set; }


    }
}