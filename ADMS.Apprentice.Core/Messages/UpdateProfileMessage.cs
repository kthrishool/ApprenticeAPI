
namespace ADMS.Apprentice.Core.Messages
{
    public record UpdateProfileMessage
    (
        BasicDetailsMessage BasicDetails,
        ContactDetailsMessage ContactDetails,
        SchoolDetailsMessage SchoolDetails,
        OtherDetailsMessage OtherDetails,
        QualificationDetailsMessage QualificationDetails
    );
}