using ADMS.Services.Apprentice.Model;
using ADMS.Services.Apprentice.Repository;
using ADMS.Services.Infrastructure.Core.Interface;
using ADMS.Services.Infrastructure.Repository;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AMDS.Services.Apprentice.UnitTests
{
    /// <summary> A mock reference data repository. </summary>
    [ExcludeFromCodeCoverage]
    public class MockAdmsRepository : RepositoryBase, IApprenticeRepository
    {
        public MockAdmsRepository(IContext context) : base(context) {}


        public  Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request)
        {
            List<RelatedCode> codes = new List<RelatedCode>();
            switch (request.RelatedCodeType)
            {
                case "ORGF":
                    if (request.SearchCode == "AAAB")
                    {
                        codes.Add(new RelatedCode() {DominantCode = "AAAB", SubordinateCode = "CP20", SubordinateDescription = "CP20 test", SubordinateShortDescription = "CP20" });
                        codes.Add(new RelatedCode() {DominantCode = "AAAB", SubordinateCode = "NHMZ", SubordinateDescription = "NHMZ test", SubordinateShortDescription = "NHMZ" });
                    }
                    break;
            }
            return Task.FromResult<IList<RelatedCode>>(codes);
        }


    }
}
