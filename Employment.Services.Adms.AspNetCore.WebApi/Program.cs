using Employment.Services.Adms.AspNetCore.WebApi;
using Employment.Services.Infrastructure.WebApi;

namespace Employment.Services.Adms.Web
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
