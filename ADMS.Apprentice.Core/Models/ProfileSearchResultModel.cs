using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Generic apprentice search results model
    /// </summary>
    public record ProfileSearchResultModel
    (
        int ApprenticeId,        
        string ProfileTypeCode,        
        string FirstName,        
        string Surname,
        string OtherNames,
        DateTime BirthDate,
        string EmailAddress,
        string USI,
        string PhoneNumber,
        string ResidentialAddress,
        string PostalAddress,
        int ScoreValue
    );
}