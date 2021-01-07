using ADMS.Services.Apprentice.Model;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Services.Apprentice.Repository
{
    /// <summary>
    /// Reference Data Repository.
    /// </summary>
    public class ApprenticeRepository : RepositoryBase, IApprenticeRepository
    {
        private IDataSource _dataSource;

        public ApprenticeRepository(IContext context) : base(context) { }



        /// <summary>
        /// Returns a list that contains all the <see cref="CodeModel" /> that meet the specified criteria.
        /// </summary>
        public async Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request)
        {
            CheckIfDisposed();
            List<RelatedCode> data = new List<RelatedCode>();

            _dataSource = await Context.EnsureDataSourceAsync("Db_ConnADW");

            DateTime? currentDate = request.CurrentDate.HasValue ? request.CurrentDate.Value : Context.User.EffectiveDateTime;

            if (currentDate == DateTime.MinValue)
            {
                currentDate = null;
            }

            SqlConnection connection = _dataSource.Connection as SqlConnection;
            using (SqlCommand command = new SqlCommand("up_ListRelatedCode", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter { ParameterName = "@return_value", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });
                command.Parameters.Add(new SqlParameter { ParameterName = "@RelationshipType", SqlDbType = SqlDbType.VarChar, Value = request.RelatedCodeType });
                command.Parameters.Add(new SqlParameter { ParameterName = "@SearchCode", SqlDbType = SqlDbType.VarChar, Value = !string.IsNullOrEmpty(request.SearchCode) ? request.SearchCode : string.Empty });
                command.Parameters.Add(new SqlParameter { ParameterName = "@SearchType", SqlDbType = SqlDbType.Char, Value = request.DominantSearch ? "d" : "s" });
                command.Parameters.Add(new SqlParameter { ParameterName = "@ListType", SqlDbType = SqlDbType.Char, Value = request.CurrentCodesOnly ? "c" : string.Empty });
                command.Parameters.Add(new SqlParameter { ParameterName = "@ExactLookup", SqlDbType = SqlDbType.Char, Value = !string.IsNullOrEmpty(request.SearchCode) || request.ExactLookup ? "y" : "n" });
                command.Parameters.Add(new SqlParameter { ParameterName = "@MaxRows", SqlDbType = SqlDbType.Int, Value = request.MaxRows });
                command.Parameters.Add(new SqlParameter { ParameterName = "@RowPosition", SqlDbType = SqlDbType.Int, Value = request.RowPosition });


                command.Parameters.Add(new SqlParameter { ParameterName = "@Current_Date", SqlDbType = SqlDbType.DateTime, Value = currentDate });

                if (request.EndDateInclusive.HasValue && request.EndDateInclusive.Value)
                {
                    command.Parameters.Add(new SqlParameter { ParameterName = "@EndDateInclusive", SqlDbType = SqlDbType.Bit, Value = true });
                }

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        RelatedCode item = new RelatedCode();
                        item.RelatedCodeValue = reader.GetString(0);
                        item.SubordinateCode = reader.GetString(1);
                        item.DominantCode = reader.GetString(2);
                        item.StartDate = reader.GetDateTime(3);
                        if (!reader.IsDBNull(4)) item.EndDate = reader.GetDateTime(4);
                        item.SubordinateShortDescription = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                        item.SubordinateDescription = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                        item.DominantShortDescription = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                        item.DominantDescription = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                        item.Position = reader.GetInt32(9);
                        item.Dominant = request.DominantSearch;
                        data.Add(item);
                    }
                }
            }

            // Filter for current codes only
            if (request.CurrentCodesOnly)
            {
                var filtered = new List<RelatedCode>();
                DateTime filterDate = DateTime.Now.Date;
                if (currentDate != null && currentDate.Value != DateTime.MinValue)
                    filterDate = currentDate.Value.Date; 

                // Filters date range based on user context
                filtered = data.Where(d => d.StartDate.HasValue && d.StartDate.Value.Date <= filterDate && (!d.EndDate.HasValue || (request.EndDateInclusive.HasValue && request.EndDateInclusive.Value ? d.EndDate.Value.Date >= filterDate : d.EndDate.Value.Date > filterDate))).ToList();
                return filtered;
            }
            return data;
        }


    }
}