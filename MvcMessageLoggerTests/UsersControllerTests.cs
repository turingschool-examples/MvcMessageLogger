using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.Models;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.FeatureTests;
using System.Runtime.CompilerServices;

namespace MvcMessageLoggerTests
{
    [Collection("Users Controller Tests")]
    public class MvcMessageLoggerUsersTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public MvcMessageLoggerUsersTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private MvcMessageLoggerContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MvcMessageLoggerContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new MvcMessageLoggerContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
        [Fact]
        public async Task Index_ShowsItems()
        {
            var context = GetDbContext();


            context.Users.Add(new User { Name = "John", Username = "Doe" });
            context.Users.Add(new User { Name = "Jon", Username = "Don" });
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Users");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            // usernames only because that is what the website is showing on the index, name will be shown in Details
            Assert.Contains("Doe", html); 
            Assert.Contains("Don", html);


            Assert.DoesNotContain("Karen", html);
        }

        [Fact]
        public async Task New_ShowsNewForm()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Users/new");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("<form id=\"new-user-form\" method=\"post\" action=\"/users\">", html);
            Assert.Contains("<button type=\"submit\">Add Customer</button>", html);
            Assert.Contains("Name", html);
            Assert.Contains("Username", html);

        }

        [Fact]
        public async Task Create_AddsItemToDB()
        {
            var client = _factory.CreateClient();

            var addUserFormData = new Dictionary<string, string>
            {
                {"Name", "John" },
                {"Username", "Doe" }
            };

            var response = await client.PostAsync("/users", new FormUrlEncodedContent(addUserFormData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            Assert.Contains("Doe", html);
        }
    }
}