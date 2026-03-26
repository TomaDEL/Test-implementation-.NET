using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Tests.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";
        private readonly string _identityDbName = $"TestIdentityDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAntiforgery(t =>
                {
                    t.Cookie.Name = "X-CSRF-TOKEN-TEST";
                    t.HeaderName = "X-CSRF-TOKEN-TEST";
                });

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<P3Referential>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<P3Referential>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                var identityDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppIdentityDbContext>));
                if (identityDescriptor != null)
                    services.Remove(identityDescriptor);
                services.AddDbContext<AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_identityDbName);
                });
            });
        }

        public void SeedDatabase()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<P3Referential>();
            db.Database.EnsureCreated();
            DatabaseSeeder.Seed(db);
        }

        public async Task SeedAdminUser()
        {
            using var scope = Services.CreateScope();
            var services = scope.ServiceProvider;
            var userManager = services.GetService(typeof(UserManager<IdentityUser>)) as UserManager<IdentityUser>;

            var adminUser = new IdentityUser("Admin");
            var result = await userManager.CreateAsync(adminUser, "P@ssword123");
        }
    }
}