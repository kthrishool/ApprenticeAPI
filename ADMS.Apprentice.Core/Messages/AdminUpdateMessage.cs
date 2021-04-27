
namespace ADMS.Apprentice.Core.Messages
{
    //the apprentice info can only be updated by dept. user or admin user
    public record AdminUpdateMessage
    {
        public bool DeceasedFlag { get; set; }
    };
}