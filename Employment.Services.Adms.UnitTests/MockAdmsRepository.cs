using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Infrastructure.Repository;
using Employment.Services.Adms.Model;
using Employment.Services.Adms.Repository;

namespace Employment.Services.Adms.UnitTests
{
    /// <summary> A mock reference data repository. </summary>
    [ExcludeFromCodeCoverage]
    public class MockAdmsRepository : RepositoryBase, IAdmsRepository
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
