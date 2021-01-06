using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Business;
using Employment.Services.Infrastructure.Core.Exceptions;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Adms.Model;
using Employment.Services.Adms.Repository;
using System.Collections.Generic;
using System;
using Employment.Services.Infrastructure.Core.Validation;

namespace Employment.Services.Adms.Business
{
    /// <remarks />
    public class AdmsBusiness
    {
        private readonly IContext _context;
        /// <remarks />
        protected ValidationException codeTypeException, relatedCodeTypeException, propertyTypeException, siteCodeException;

        /// <remarks />
        public AdmsBusiness(IContext context)
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
            IAdmsRepository AdmsRepository = _context.EnsureRepository<IAdmsRepository>();
            return await AdmsRepository.GetRelatedCodesAsync(request);
        }

    }
}


