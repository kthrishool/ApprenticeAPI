using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.UnitTests.Constants;
using FluentAssertions;

namespace ADMS.Apprentice.UnitTests.Helpers
{
    public static class PropertyMapper
    {
        /// <summary>
        /// Get Related Codes.
        /// </summary>
        /// <param name="result">Object that will be used</param>
        /// <param name="fieldDefinition">Field Name , field length,field Type,isNullable</param>
        /// <returns></returns>
        private static void SetNullForManditoryFiled<T>(ProfileMessage result, Dictionary<string, Tuple<int, string, bool>> fieldDefinition, IPropertyValidator Validator) //where T : ProfileMessage, new()
        {
            PropertyInfo[] tProps = typeof(ProfileMessage).GetProperties();

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
                    var lstErrors = Validator.ValidateModel(result);
                    // if its a nullable value no error 
                    if (FieldMapping.Value.Item3)
                        lstErrors.Should().HaveCount(1);
                    else
                        lstErrors.Should().HaveCount(0);

                    // true 
                    prop.SetValue(result, "test");
                    lstErrors = Validator.ValidateModel(result);
                    lstErrors.Should().HaveCount(0);

                    // exceeds max length
                    prop.SetValue(result, ProfileConstants.RandomString(FieldMapping.Value.Item1 + 10));
                    lstErrors = Validator.ValidateModel(result);
                    lstErrors.Should().HaveCountGreaterThan(0);
                }


                prop.SetValue(result, propstringvalue);
            }
        }
    }
}