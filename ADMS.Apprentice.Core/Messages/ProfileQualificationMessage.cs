using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileQualificationMessage
    {
        [MaxLength(10, ErrorMessage = "Qualification code cannot exceed 10 characters in length")]
        public string QualificationCode { get; set; }

        [MaxLength(50, ErrorMessage = "Qualification description cannot exceed 100 characters in length")]
        public string QualificationDescription { get; set; }
        
        public string StartMonth { get; set; }        
        public string StartYear { get; set; }        
        public string EndMonth { get; set; }        
        public string EndYear { get; set; }        
    }
}