using ADMS.Services.Apprentice.Model;
using ADMS.Services.Apprentice.Repository;
using ADMS.Services.Infrastructure.Core.Business;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMS.Services.Apprentice.Business
{
    /// <remarks />
    public class ApprenticeBusiness
    {
        private readonly IContext _context;
        /// <remarks />
        protected ValidationException codeTypeException, relatedCodeTypeException, propertyTypeException, siteCodeException;

        /// <remarks />
        public ApprenticeBusiness(IContext context)
        {
            _context = context;
            codeTypeException = new ValidationException(_context, BusinessValidationErrors.CreateRequiredFieldMissingValidationError("RDEV1", "CodeType"));
            relatedCodeTypeException = new ValidationException(_context, BusinessValidationErrors.CreateRequiredFieldMissingValidationError("RDVE2", "RelatedCodeType"));
            propertyTypeException = new ValidationException(_context, BusinessValidationErrors.CreateRequiredFieldMissingValidationError("RDVE1", "PropertyType"));
            siteCodeException = new ValidationException(context, BusinessValidationErrors.CreateRequiredFieldMissingValidationError("RDEV4", "SiteCode"));
        }


        /// <remarks />
        public async Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request)
        {
            if (request.RelatedCodeType == null) throw relatedCodeTypeException;
            IApprenticeRepository AdmsRepository = _context.EnsureRepository<IApprenticeRepository>();
            return await AdmsRepository.GetRelatedCodesAsync(request);
        }

    }
}


