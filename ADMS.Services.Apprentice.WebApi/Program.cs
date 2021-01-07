using ADMS.Services.Apprentice.WebApi;
using Employment.Services.Infrastructure.WebApi;

namespace ADMS.Services.Apprentice.Web
{
    /// <remarks />
    public class Program
    {
        /// <remarks />
        public static void Main(string[] args)
        {
            new ProgramLauncher<Startup>().Launch(args);
        }
    }
}
