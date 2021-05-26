using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    public class ProfileGuardianModel
    {
        public int ApprenticeId { get; set; }

        public int Id { get; set; }

        public string Surname { get; init; }
        public string FirstName { get; init; }
        public string EmailAddress { get; init; }

        public string HomePhoneNumber { get; init; }

        public string Mobile { get; init; }

        public string WorkPhoneNumber { get; init; }

        public ProfileAddressModel Address { get; init; }

        public ProfileGuardianModel(Guardian guardian)
        {
            ApprenticeId = guardian.ApprenticeId;
            Id = guardian.Id;
            Surname = guardian.Surname;
            FirstName = guardian.FirstName;
            EmailAddress = guardian.EmailAddress;
            HomePhoneNumber = guardian.HomePhoneNumber;
            Mobile = guardian.Mobile;
            WorkPhoneNumber = guardian.WorkPhoneNumber;
            Address = new ProfileAddressModel()
            {
                StreetAddress1 = guardian.StreetAddress1,
                StreetAddress2 = guardian.StreetAddress2,
                StreetAddress3 = guardian.StreetAddress3,
                Locality = guardian.Locality,
                StateCode = guardian.StateCode,
                Postcode = guardian.Postcode,
                SingleLineAddress = guardian.SingleLineAddress
            };
        }
    }
}