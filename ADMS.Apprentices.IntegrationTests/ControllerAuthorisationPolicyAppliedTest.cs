using System.Linq;
using System.Reflection;
using Adms.Shared.Extensions;
using ADMS.Apprentices.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.IntegrationTests
{
    [TestClass]
    public class ControllerAuthorisationPolicyAppliedTest
    {
        [TestMethod]    
        public void CheckAuthorisationAttributeForAllPublicControllerMethods()
        {
            var asc = typeof(ADMS.Apprentices.Api.Controllers.ApprenticeProfileController).Assembly;    
            var controllers = asc.GetTypes()
                .Where(t => t.IsPublic && t.IsClass
                            && t.IsSubclassOf(typeof(ControllerBase))
                            && !(t == typeof(HomeController)));
            
            controllers.Each(c => {
                var methods = c.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
                methods.Each(m => {
                        var methodHasAuthoriseAttribute = m.GetCustomAttributes(true).Any(a => a.GetType() == typeof(AuthorizeAttribute));
                        if(!methodHasAuthoriseAttribute) {
                            System.Console.Error.WriteLine("AuthorizeAttribute missing for {0} on {1}", c.Name, m.Name);
                        }
                        Assert.IsTrue(methodHasAuthoriseAttribute);
                });
            }
            );
        }

        [TestMethod]    
        public void CheckAuthorisationAttributeIsNotAppliedAtControllerLevel()
        {
            var asc = typeof(ADMS.Apprentices.Api.Controllers.ApprenticeProfileController).Assembly;    
            var controllers = asc.GetTypes().Where(t => t.IsPublic && t.IsClass && t.IsSubclassOf(typeof(ControllerBase)));
            
            controllers.Each(c => {
                var controllerHasAuthoriseAttribute = c.CustomAttributes.Any(a => a.AttributeType == typeof(AuthorizeAttribute));
                if(controllerHasAuthoriseAttribute) {
                    System.Console.Error.WriteLine("AuthorizeAttribute found for {0} please apply authorisation security at the method level.", c.Name);
                }
                Assert.IsFalse(controllerHasAuthoriseAttribute);
            });
        }
    }
}