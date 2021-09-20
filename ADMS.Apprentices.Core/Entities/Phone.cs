using ADMS.Services.Infrastructure.Model.Interface;
using Au.Gov.Infrastructure.EntityFramework.Entities;

namespace ADMS.Apprentices.Core.Entities
{
    public class Phone : PhoneBase, IAuditedIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }

        public int ApprenticeId { get; set; }


        public virtual Profile Profile { get; set; }
    }
}