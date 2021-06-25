using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentices.Core.Entities
{
    public class Phone : PhoneBase, IAuditableIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }

        public int ApprenticeId { get; set; }


        public virtual Profile Profile { get; set; }
    }
}