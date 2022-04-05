using AnimalCountingDatabase.Api;
using AnimalCountingDatabase.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnimalCountingDatabase.Tests
{
    public class DemoTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // Create DB context
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables() // the connectionstring in .github\workflows\ci-cd.yaml overrides what is in appsettings.json
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

            var context = new CustomerContext(optionsBuilder.Options);

            // Delete all existing Customers in the DB
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync(); // Similar to dotnet ef database update            

            // Create Controller
            CustomersController controller = new (context);

            // Add a Customer            
            await controller.Add(new Customer { CustomerName = "FooBar" });

            // Check: Does  GetAll() returned the added Customer?
            var result = (await controller.GetAll()).ToArray();

            Assert.Single(result);
            Assert.Equal("FooBar", result[0].CustomerName);
        }
    }
}
