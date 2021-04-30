using System.Linq;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared;
using Adms.Shared.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace ADMS.Apprentice.Core.Services
{
    //TODO: implement unit testing
    [ExcludeFromCodeCoverage]
    public class ProfileRetreiver : IProfileRetreiver
    {
        private readonly IRepository _repository;
        private readonly IExceptionFactory exceptionFactory;

        public ProfileRetreiver(
            IRepository repository,
            IExceptionFactory _exceptionFactory)
        {
            _repository = repository;
            exceptionFactory = _exceptionFactory;
        }

        /// <summary>
        /// Returns as list of matching apprentice based on the search Criteria
        /// Search Id.
        /// </summary>
        /// <param name="ProfileSearchCriteria">Search criteria for searching up a profile</param>
        public IQueryable<Profile> RetreiveList() //(ProfileSearchCriteria criteria)
        {
            IQueryable<Profile> tfnRecords = null;

            tfnRecords = _repository.Retrieve<Profile>().Where(x => x.ActiveFlag == true).AsQueryable().Take(500);

            return tfnRecords;
        }
    }
}