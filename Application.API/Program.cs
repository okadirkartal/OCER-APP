using Application.Infrastructure.DAL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Application.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {


                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDBContext>();
                    DataSeeder.SeedData(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while seeding the database. {ex.Message}");
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
