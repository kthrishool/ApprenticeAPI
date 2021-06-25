using Adms.Shared.Attributes;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Database.Seeders
{
    [RegisterWithIocContainer(HasMultipleImplementations = true)]
    public interface IDataSeeder
    {
        Task SeedAsync();
        int Order { get; }
    }
}