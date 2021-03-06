using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Database.Seeders;
using Adms.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace ADMS.Apprentices.Api
{
    /// <summary>
    /// Application start point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Start point for the application
        /// </summary>
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            await SeedDatabaseAsync(host);
            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;
            return WebHost.CreateDefaultBuilder(args).UseStartup(assemblyName);
        }

        private static async Task SeedDatabaseAsync(IWebHost host)
        {
            if (GetDatabaseSettings(host).Value.SeedSampleData)
            {
                using IServiceScope scope = host.Services.CreateScope();
                IServiceProvider services = scope.ServiceProvider;
                var dataSeeders = services.GetRequiredService<IEnumerable<IDataSeeder>>();
                var repository = services.GetRequiredService<IRepository>();
                foreach (IDataSeeder seeder in dataSeeders.OrderBy(s => s.Order))
                {
                    await seeder.SeedAsync();
                    await repository.SaveAsync();
                }
            }
        }
        private static IOptions<OurDatabaseSettings> GetDatabaseSettings(IWebHost host)
        {
            return host.Services.GetRequiredService<IOptions<OurDatabaseSettings>>();
        }
    }
}