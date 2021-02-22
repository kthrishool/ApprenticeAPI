using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Database.Seeders
{
    [RegisterWithIocContainer(HasMultipleImplementations = true)]
    public interface IDataSeeder
    {
        void Seed();
        int Order { get; }
    }
}