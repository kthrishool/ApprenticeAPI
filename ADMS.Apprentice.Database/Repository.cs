using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core;
using Adms.Shared;
using Adms.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Database
{
    public class Repository : RepositoryBase, IRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public Repository(IOptions<OurDatabaseSettings> ourDatabaseSettings) : base(ourDatabaseSettings)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        public IQueryable<T> Retrieve<T>() where T : class, IAmAnAggregateRoot
        {
            return Set<T>();
        }

        public void Insert<T>(T entity) where T : class, IAmAnAggregateRoot
        {
            Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class, IAmAnAggregateRoot
        {
            Set<T>().Remove(entity);
        }

        public T Get<T>(int id) where T : class, IAmAnAggregateRoot<int>
        {
            return Retrieve<T>().Get(id);
        }

        public void DeleteAll<T>() where T : class, IAmAnAggregateRoot
        {
            IEntityType mapping = Model.FindEntityType(typeof(T));
            string schema = mapping.GetSchema();
            string tableName = mapping.GetTableName();
            string sql = $"DELETE FROM {schema}.[{tableName}]";
            Database.ExecuteSqlRaw(sql);
        }

        public void DetachAll()
        {
            EntityEntry[] entries = ChangeTracker.Entries().ToArray();
            foreach (EntityEntry entity in entries)
            {
                Entry(entity.Entity).State = EntityState.Detached;
            }
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public void Save()
        {
            SaveChanges();
        }

        public async Task SaveAsync()
        {
            await SaveChangesAsync();
        }

    }
}