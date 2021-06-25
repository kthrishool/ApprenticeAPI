using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using Adms.Shared;
using Microsoft.EntityFrameworkCore;

namespace ADMS.Apprentices.Core.Services
{
    public class TFNStatsRetriever : ITFNStatsRetriever
    {
        private readonly IRepository _repository;

        public TFNStatsRetriever(
            IRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<ApprenticeTFN> RetrieveTfnStats(TFNStatsCriteria criteria)
        {
            IQueryable<ApprenticeTFN> tfnRecords = null;

            if (criteria != null && GetStatusCodeFromCriteria(criteria.StatusCode, out TFNStatus[] tfnStatus) && tfnStatus != null && tfnStatus.Any())
                tfnRecords = _repository.Retrieve<ApprenticeTFN>().Include(x => x.Profile).Where(x => tfnStatus.Contains(x.StatusCode)).AsQueryable();
            else
                tfnRecords = _repository.Retrieve<ApprenticeTFN>().Include(x => x.Profile).AsQueryable();

            // Apply filter
            if (criteria != null && !string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                tfnRecords = tfnRecords.Where(x =>
                    x.ApprenticeId.ToString() == criteria.Keyword
                    ||
                    (x.Profile != null
                     &&
                     (x.Profile.FirstName.ToLower().Contains(criteria.Keyword.ToLower())
                      ||
                      x.Profile.Surname.ToLower().Contains(criteria.Keyword.ToLower())
                      ||
                      (x.Profile.BirthDate.Day + "/" + x.Profile.BirthDate.Month + "/" + x.Profile.BirthDate.Year).Equals(criteria.Keyword)
                         //string.Format("{0:d/M/yyyy}",x.Profile.BirthDate).Equals(criteria.Keyword)
                         //DateTime.ToString() is not accepted by Linq so you could use x.Profile.BirthDate.Date+"/"+
                     ))
                );
            }
            
            return tfnRecords;
        }

        private bool GetStatusCodeFromCriteria(string statusCode, out TFNStatus[] tfnStatus)
        {
            tfnStatus = null;

            if (string.IsNullOrWhiteSpace(statusCode))
                return false;

            if (statusCode.Equals(TFNStatus.MTCH.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.MTCH };
            else if (statusCode.Equals(TFNStatus.NOCH.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.NOCH };
            else if (statusCode.Equals(TFNStatus.SBMT.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.SBMT };
            else if (statusCode.Equals(TFNStatus.TBVE.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.TBVE };
            else if (statusCode.Equals(TFNStatus.TERR.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.TERR };
            else if (statusCode.Equals("INPROG", StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = new TFNStatus[] { TFNStatus.TERR, TFNStatus.SBMT, TFNStatus.TBVE };
            else
                return false;

            return true;
        }
    }
}
