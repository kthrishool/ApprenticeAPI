using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Database.Seeders
{
    [RegisterWithIocContainer(HasMultipleImplementations = true)]
    public interface IDataSeeder
    {
        Task SeedAsync();
        int Order { get; }
    }
}