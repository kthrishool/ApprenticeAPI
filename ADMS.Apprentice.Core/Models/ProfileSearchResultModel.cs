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
        int ApprenticeId ,
        //string CustomerReferenceNumber,
        string ProfileTypeCode,
        //string TitleCode,
        string FirstName,
        string OtherNames,
        string Surname,
        string GenderCode,
        DateTime BirthDate,
        int ScoreValue
        //int? PreviousApprenticeId
    );
}