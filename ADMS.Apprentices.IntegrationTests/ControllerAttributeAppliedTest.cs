
using System.Linq;
using System.Reflection;
using Adms.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Employers.IntegrationTests
{
    [TestClass]
    public class ControllerApiAttributeAppliedTest
    {
        [TestMethod]    
        public void CheckApiControllerAuthorisationAttributeIsNotAppliedAtControllerLevel()
        {
            var asc = typeof(ADMS.Apprentices.Api.Controllers.ApprenticeProfileController).Assembly;    
            var controllers = asc.GetTypes().Where(t => t.IsPublic && t.IsClass && t.IsSubclassOf(typeof(ControllerBase)));
            
            controllers.Each(c => {
                var controllerHasAuthoriseAttribute = c.CustomAttributes.Any(a => a.AttributeType == typeof(ApiControllerAttribute));
                if(!controllerHasAuthoriseAttribute) {
                    System.Console.Error.WriteLine("ApiControllerAttribute not found for {0} please apply ApiControllerAttribute for the controller.", c.Name);
                }
                Assert.IsTrue(controllerHasAuthoriseAttribute);
            });
        }
    }
}