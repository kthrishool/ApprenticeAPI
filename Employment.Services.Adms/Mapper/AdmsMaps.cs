using ADMS.Services.Apprentice.Contract;
using ADMS.Services.Apprentice.Model;

namespace ADMS.Services.Apprentice.WebApi
{
    /// <remarks />
    public class AdmsMaps
    {

        /// <remarks />
        public static RelatedCodeResponseV1 MapToRelatedCodeResponseV1(RelatedCode model)
        {
            if (model == null) return null;

            RelatedCodeResponseV1 result = new RelatedCodeResponseV1();
            result.Dominant = model.Dominant;
            result.DominantCode = model.DominantCode;
            result.DominantDescription = model.DominantDescription;
            result.DominantShortDescription = model.DominantShortDescription;
            result.EndDate = model.EndDate;
            result.Position = model.Position;
            result.RelatedCode = model.RelatedCodeValue;
            result.StartDate = model.StartDate;
            result.SubordinateCode = model.SubordinateCode;
            result.SubordinateDescription = model.SubordinateDescription;
            result.SubordinateShortDescription = model.SubordinateShortDescription;
            return result;
        }

    }
}