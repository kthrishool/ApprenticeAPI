using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ADMS.Apprentices.Api.Controllers;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.UnitTests.Constants;
using ADMS.Apprentices.UnitTests.Helpers;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile

    [TestClass]
    public class WhenCreatingAApprenticeProfileUsingApi : GivenWhenThen<ApprenticeProfileController>, IPropertyValidator
    {
        private Profile profile;
        private ActionResult<ProfileModel> profileResult;
        private ProfileMessage message;

        private Dictionary<string, Tuple<int, string, bool>> profileFieldDefinition;

        private Dictionary<string, Tuple<int, string, bool>> addressFieldDefinition;

        //message
        private ProfileMessage CreateNewProfileMessage(string surName,
            String firstName,
            DateTime dob,
            String email = null,
            string profileType = null,
            string[] phoneNumbers = null,
            string indigenousStatusCode = null,
            string selfAssessedDisabilityCode = null,
            string citizenshipCode = null,
            string gender = null,
            string countryofBirth = null,
            string Language = null,
            string USI = null)
        {
            return new ProfileMessage
            {
                Surname = surName,
                FirstName = firstName,
                BirthDate = dob,
                EmailAddress = email,
                ProfileType = profileType,
                PhoneNumbers = phoneNumbers?.Select(c => new PhoneNumberMessage() {PhoneNumber = c}).ToList(),
                IndigenousStatusCode = indigenousStatusCode,
                SelfAssessedDisabilityCode = selfAssessedDisabilityCode,
                CitizenshipCode = citizenshipCode,
                GenderCode = gender,
                CountryOfBirthCode = countryofBirth,
                LanguageCode = Language,
                USI = USI
            };
        }

        private ProfileMessage GetValidMessage()
        {
            return new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileType = ProfileConstants.Profiletype,
                PhoneNumbers = ProfileConstants.PhoneNumbers.ToList(),
                ResidentialAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = "14 Mort Street, Braddon,ACT -2601"},
                PostalAddress = new ProfileAddressMessage() {Postcode = "2601", StateCode = "ACT", Locality = "BRADDON", StreetAddress1 = "14 Mort Street", StreetAddress2 = "14 Mort Street", SingleLineAddress = "14 Mort Street, Braddon,ACT -2601"},
                GenderCode = "M",
                CountryOfBirthCode = "AUS",
                LanguageCode = "1200",
                HighestSchoolLevelCode = "99"
            };
        }

        protected override void Given()
        {
            message = new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileType = ProfileConstants.Profiletype,
                PhoneNumbers = ProfileConstants.PhoneNumbers.ToList(),
                ResidentialAddress = ProfileConstants.ResidentialAddress,
                PostalAddress = ProfileConstants.PostalAddress
            };
            profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                BirthDate = message.BirthDate.Value,
                EmailAddress = message.EmailAddress,
                ProfileTypeCode = message.ProfileType,
            };
            profileFieldDefinition = new Dictionary<string, Tuple<int, string, bool>>
            {
                {"Surname", Tuple.Create(50, ValidationDataTypes.STRING_TYPE, true)},
                {"FirstName", Tuple.Create(50, ValidationDataTypes.STRING_TYPE, true)},
                {"PhoneNumbers", Tuple.Create(0, ValidationDataTypes.OBJECT_TYPE, false)},
                {"ResidentialAddress", Tuple.Create(0, ValidationDataTypes.OBJECT_TYPE, false)},
                {"PostalAddress", Tuple.Create(0, ValidationDataTypes.OBJECT_TYPE, false)},
                {"LanguageCode", Tuple.Create(10, ValidationDataTypes.STRING_TYPE, false)},
            };
            addressFieldDefinition = new Dictionary<string, Tuple<int, string, bool>>
            {
                {"StreetAddress1", Tuple.Create(80, ValidationDataTypes.STRING_TYPE, false)},
                {"StreetAddress2", Tuple.Create(80, ValidationDataTypes.STRING_TYPE, false)},
                {"StreetAddress3", Tuple.Create(80, ValidationDataTypes.STRING_TYPE, false)},
                {"Locality", Tuple.Create(40, ValidationDataTypes.STRING_TYPE, false)},
                {"StateCode", Tuple.Create(40, ValidationDataTypes.STRING_TYPE, false)},
                {"Postcode", Tuple.Create(4, ValidationDataTypes.STRING_TYPE, false)},
                {"SingleLineAddress", Tuple.Create(375, ValidationDataTypes.STRING_TYPE, false)}
            };
            Container
                .GetMock<IProfileCreator>()
                .Setup(r => r.CreateAsync(message))
                .Returns(Task.FromResult(profile));
        }

        protected override async void When()
        {
            profileResult = await ClassUnderTest.Create(message);
        }

        [TestMethod]
        public void ShouldReturnResult()
        {
            profileResult.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfNameNotValid()
        {
            message = CreateNewProfileMessage("Bob$", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Surname must contain only letters, spaces, hyphens and apostrophies");
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfDefaultValuesareNull()
        {
            message = CreateNewProfileMessage(null, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Surname is required");
        }


        [TestMethod]
        public void ShouldReturnNoValidationErrorIfNameIsValid()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfDisabiliyyStatusCodeNotValid()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype, null, "@", "InvalidCode");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Invalid Self assessed disability code");
        }

        public IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        #region EmailAddressTests

        [TestMethod]
        public void ShouldReturnNoValidationErrorIfEmailIsEmpty()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        public void ShouldReturnNoValidationErrorIfEmailIsNull()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), "", ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfEmailLenghtExceedsMax()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname,
                ProfileConstants.Birthdate, ProfileConstants.Emailaddressmax256 + ProfileConstants.RandomString(100), ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Email address cannot have more than 256 characters");
        }

        [TestMethod]
        public void ShouldReturnNoErrorPhoneNumberisNull()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname,
                ProfileConstants.Birthdate, "", ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        #endregion

        #region Gender

        [TestMethod]
        public void DoNothingWhenGenderIsValid()
        {
            message = GetValidMessage();
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        [TestMethod]
        public void ShowValidationExceptionWhenGenderIsInvalid()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype, null, null, null, null, "MQ");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Gender Code is invalid");
        }

        #endregion

        #region Autovalidator

        [TestMethod]
        public void SetNullForManditoryProfileFiled() //where T : ProfileMessage, new()
        {
            //ProfileMessage result = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            //Tuple<int, string, Boolean> length , variable type and is mandatory 
            SetNullForManditoryFiled(message, profileFieldDefinition);
        }


        private void SetNullForManditoryFiled<T>(T result, Dictionary<string, Tuple<int, string, bool>> fieldDefinition) //where T : ProfileMessage, new()
        {
            PropertyInfo[] tProps = typeof(T).GetProperties();

            foreach (PropertyInfo prop in tProps)
            {
                var propstringvalue = prop.GetValue(result);
                var upperPropName = prop.Name.ToUpper();
                var FieldMapping = fieldDefinition.FirstOrDefault(p => p.Key.ToUpper() == upperPropName);
                // extract value for this property                    
                if (fieldDefinition.All(p => p.Key.ToUpper() != upperPropName))
                {
                    continue;
                }
                else if (FieldMapping.Value.Item2 == ValidationDataTypes.STRING_TYPE)
                {
                    // set null 
                    prop.SetValue(result, null);
                    var lstErrors = ValidateModel(result);
                    // if its a nullable value no error 
                    if (FieldMapping.Value.Item3)
                        lstErrors.Should().HaveCount(1);
                    else
                        lstErrors.Should().HaveCount(0);

                    // true 
                    prop.SetValue(result, "test");
                    lstErrors = ValidateModel(result);
                    lstErrors.Should().HaveCount(0);

                    // exceeds max length
                    prop.SetValue(result, ProfileConstants.RandomString(FieldMapping.Value.Item1 + 10));
                    lstErrors = ValidateModel(result);
                    lstErrors.Should().HaveCountGreaterThan(0);
                }
                else if (FieldMapping.Value.Item2 == ValidationDataTypes.OBJECT_TYPE)
                {
                    // set null to the object
                    prop.SetValue(result, null);
                    var lstErrors = ValidateModel(result);
                    // if its a nullable value no error 
                    if (FieldMapping.Value.Item3)
                        lstErrors.Should().HaveCount(1);
                    else
                        lstErrors.Should().HaveCount(0);
                }

                prop.SetValue(result, propstringvalue);
            }
        }

        #endregion

        #region Address

        [TestMethod]
        public void SetNullForAddressFiled() //where T : ProfileMessage, new()
        {
            SetNullForManditoryFiled(message.ResidentialAddress, addressFieldDefinition);
            SetNullForManditoryFiled(message.PostalAddress, addressFieldDefinition);
        }


        private Boolean addressflag = true;

        private ProfileAddressMessage GetAddressMessage()
        {
            if (addressflag)
                return GetValidMessage().ResidentialAddress;
            else
            {
                return GetValidMessage().PostalAddress;
            }
        }

        [TestMethod]
        public void ShouldReturnValidationErrorForProfileFields()
        {
            for (int j = 1; j < 3; j++)
            {
                for (int i = 1; i < 8; i++)
                {
                    ProfileAddressMessage testAddressMessage = GetAddressMessage();
                    string errorMessage;
                    switch (i)
                    {
                        // message.ResidentialAddress.StreetAddress1
                        case 1:
                            testAddressMessage.StreetAddress1 = ProfileConstants.RandomString(81);
                            errorMessage = "Street Address Line 1 cannot exceed 80 characters in length";
                            break;
                        case 2:

                            testAddressMessage.StreetAddress2 = ProfileConstants.RandomString(81);
                            errorMessage = "Street Address Line 2 cannot exceed 80 characters in length";
                            break;

                        case 3:
                            testAddressMessage.StreetAddress3 = ProfileConstants.RandomString(81);
                            errorMessage = "Street Address Line 3 cannot exceed 80 characters in length";
                            break;
                        case 4:
                            testAddressMessage.Locality = ProfileConstants.RandomString(41);
                            errorMessage = "Suburb cannot exceed 40 characters in length";
                            break;
                        case 5:
                            testAddressMessage.Postcode = ProfileConstants.RandomString(5);
                            errorMessage = "Postcode cannot exceed 4 characters in length";
                            break;
                        case 6:
                            testAddressMessage.StateCode = ProfileConstants.RandomString(41);
                            errorMessage = "State cannot exceed 40 characters in length";
                            break;
                        case 7:
                            testAddressMessage.SingleLineAddress = ProfileConstants.RandomString(376);
                            errorMessage = "Address cannot exceed 375 characters in length";
                            break;
                        default:
                            throw new AssertFailedException();
                    }

                    var lstErrors = ValidateModel(testAddressMessage);
                    lstErrors.Should().HaveCount(1);
                    lstErrors[0].ErrorMessage.Should().StartWith(errorMessage);
                } // For ResidentialAddress 
                addressflag = false;
            }
        }

        #endregion

        #region CountryofBirth

        [TestMethod]
        public void DoNothingWhenCountryOfBirthIsValid()
        {
            message = message = CreateNewProfileMessage("Bob", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        #endregion

        #region Language

        [TestMethod]
        public void DoNothingWhenLanguageIsValid()
        {
            message = message = CreateNewProfileMessage("Bob", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null,
                ProfileConstants.Profiletype,
                null, null, null,
                null, "X", null, "111");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        #endregion


        #region USI

        [TestMethod]
        public void DoNothingWhenUSIIsNull()
        {
            message = message = CreateNewProfileMessage("Bob", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null,
                ProfileConstants.Profiletype,
                null, null, null,
                null, "X", null, "");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        [TestMethod]
        public void DoNothingWhenUSIIsNotNull()
        {
            message = message = CreateNewProfileMessage("Bob", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null,
                ProfileConstants.Profiletype,
                null, null, null,
                null, "X", null, "", "Test");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        #endregion
    }

    #endregion
}