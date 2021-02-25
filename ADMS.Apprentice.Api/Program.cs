using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Database.Seeders;
using Adms.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace ADMS.Apprentice.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            //MigrateDatabase(host);
            //SeedDatabase(host);
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;
            return WebHost.CreateDefaultBuilder(args).UseStartup(assemblyName);
        }

        //private static void MigrateDatabase(IWebHost host)
        //{
        //    if (GetDatabaseSettings(host).Value.MigrateDatabaseOnStartup)
        //    {
        //        using IServiceScope scope = host.Services.CreateScope();
        //        IServiceProvider services = scope.ServiceProvider;
        //        var repository = services.GetRequiredService<IRepository>();
        //        repository.Migrate();
        //    }
        //}

        //private static void SeedDatabase(IWebHost host)
        //{
        //    if (GetDatabaseSettings(host).Value.SeedSampleData)
        //    {
        //        using IServiceScope scope = host.Services.CreateScope();
        //        IServiceProvider services = scope.ServiceProvider;
        //        var dataSeeders = services.GetRequiredService<IEnumerable<IDataSeeder>>();
        //        var repository = services.GetRequiredService<IRepository>();
        //        foreach (IDataSeeder seeder in dataSeeders.OrderBy(s => s.Order))
        //        {
        //            seeder.Seed();
        //            repository.Save();
        //        }
        //    }
        //}

        //private static IOptions<OurDatabaseSettings> GetDatabaseSettings(IWebHost host)
        //{
        //    return host.Services.GetRequiredService<IOptions<OurDatabaseSettings>>();
        //}
    }
}