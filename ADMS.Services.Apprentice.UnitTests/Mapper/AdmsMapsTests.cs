using ADMS.Services.Apprentice.Model;
using ADMS.Services.Apprentice.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Employment.Services.Adms.UnitTests.Mapper
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AdmsMapsTests
    {


        [TestMethod]
        public void AdmsMaps_MapToRelatedCodeV1_Null()
        {
            var result = ApprenticeMaps.MapToRelatedCodeResponseV1(null);
            Assert.IsTrue(result==null);
        }
        [TestMethod]
        public void AdmsMaps_MapToRelatedCodeV1_Empty()
        {
            RelatedCode model = new RelatedCode(){ };

            var result = ApprenticeMaps.MapToRelatedCodeResponseV1(model);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Dominant==model.Dominant);
            Assert.IsTrue(result.DominantCode==model.DominantCode);
            Assert.IsTrue(result.DominantDescription==model.DominantDescription);
            Assert.IsTrue(result.DominantShortDescription==model.DominantShortDescription);
            Assert.IsTrue(result.EndDate==model.EndDate);
            Assert.IsTrue(result.StartDate==model.StartDate);
            Assert.IsTrue(result.SubordinateCode==model.SubordinateCode);
            Assert.IsTrue(result.SubordinateDescription==model.SubordinateDescription);
            Assert.IsTrue(result.SubordinateShortDescription==model.SubordinateShortDescription);
            Assert.IsTrue(result.RelatedCode==model.RelatedCodeValue);
            Assert.IsTrue(result.Position==model.Position);
        }

        [TestMethod]
        public void AdmsMaps_MapToRelatedCodeV1_ValidMappings()
        {
            RelatedCode model = new RelatedCode(){ 
                
                Dominant=true,
                DominantCode="xDominantCode",
                DominantDescription="xDominantDescription",
                DominantShortDescription="xDominantShortDescription",
                SubordinateCode="xSubordinateCode",
                SubordinateDescription="xSubordinateDescription",
                SubordinateShortDescription="xSubordinateShortDescription",
                RelatedCodeValue="xRelatedCodeValue",
                Position=5,
                EndDate = new DateTime(2025,1,2), 
                 StartDate = new DateTime(2020,1,2)};

            var result = ApprenticeMaps.MapToRelatedCodeResponseV1(model);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Dominant==model.Dominant);
            Assert.IsTrue(result.DominantCode==model.DominantCode);
            Assert.IsTrue(result.DominantDescription==model.DominantDescription);
            Assert.IsTrue(result.DominantShortDescription==model.DominantShortDescription);
            Assert.IsTrue(result.EndDate==model.EndDate);
            Assert.IsTrue(result.StartDate==model.StartDate);
            Assert.IsTrue(result.SubordinateCode==model.SubordinateCode);
            Assert.IsTrue(result.SubordinateDescription==model.SubordinateDescription);
            Assert.IsTrue(result.SubordinateShortDescription==model.SubordinateShortDescription);
            Assert.IsTrue(result.RelatedCode==model.RelatedCodeValue);
            Assert.IsTrue(result.Position==model.Position);
        }

 
    }
}
