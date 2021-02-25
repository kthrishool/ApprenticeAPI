using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using LumenWorks.Framework.IO.Csv;

namespace ADMS.Apprentice.Database.Seeders
{
    #region NotInUse - no seeding at the moment
    public class ClaimSubmissionSeeder : IDataSeeder
    {
        private readonly IRepository repository;
        private readonly IEmbeddedResourceHelper embeddedResourceHelper;

        public ClaimSubmissionSeeder(IRepository repository, IEmbeddedResourceHelper embeddedResourceHelper)
        {
            this.repository = repository;
            this.embeddedResourceHelper = embeddedResourceHelper;
        }

        public int Order => 100;

        public void Seed()
        {
            //if (!repository.Retrieve<ClaimSubmission>().Any())
            //{
            //    foreach (ClaimSubmission claimSubmission in GetMockClaimSubmissions())
            //    {
            //        repository.Insert(claimSubmission);
            //    }
            //}
        }

        //private ClaimSubmission[] GetMockClaimSubmissions()
        //{
        //    var results = new List<ClaimSubmission>();
        //    Stream csv = embeddedResourceHelper.GetResourceAsStream(GetType().Assembly, "Seeders.sample-claim-submissions.csv");
        //    foreach (string[] row in ParseCsv(csv))
        //    {
        //        int col = 0;
        //        results.Add(new ClaimSubmission
        //        {
        //            SubmissionStatus = row[col++].ParseAsEnum<ClaimSubmissionStatus>(),
        //            Type = row[col++].ParseAsEnum<ClaimType>(),
        //            Category = row[col++].ParseAsEnum<ClaimCategory>(),
        //            ApprenticeId = int.Parse(row[col++]),
        //            ApprenticeName = row[col++],
        //            EmployerId = int.Parse(row[col++]),
        //            EmployerName = row[col++],
        //            NetworkProviderId = int.Parse(row[col++]),
        //            NetworkProviderName = row[col++],
        //            CreatedDate = DateTime.Parse(row[col++]),
        //            LastModifiedDate = DateTime.Parse(row[col++])
        //        });
        //    }
        //    return results.ToArray();
        //}

        //    private string[][] ParseCsv(Stream stream)
        //    {
        //        using (CsvReader reader = new CsvReader(new StreamReader(stream), true))
        //        {
        //            reader.MissingFieldAction = MissingFieldAction.ReplaceByNull;
        //            int fieldCount = reader.FieldCount;
        //            var results = new List<string[]>();
        //            reader.GetFieldHeaders();
        //            while (reader.ReadNextRecord())
        //            {
        //                string[] row = new string[fieldCount];
        //                for (int i = 0; i < fieldCount; i++)
        //                {
        //                    row[i] = reader[i];
        //                }
        //                results.Add(row);
        //            }
        //            return results.ToArray();
        //        }
        //    }

     }
     #endregion
}
