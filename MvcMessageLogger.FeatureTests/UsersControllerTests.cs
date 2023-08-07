using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.FeatureTests
{
    [Collection("Controller Tests")]
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UsersControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Index_ReturnsViewWithAllUsers()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();

            var user1 = new User { Name = "John Doe", Username = "jdoe",Email = "john@gmail.com",Password = "abcdefg" };
            var user2 = new User { Name = "Jane Doe", Username = "j_doe", Email = "jane@gmail.com", Password = "abdefg" };
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync("/users");
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user1.Name, html);
            Assert.Contains(user2.Name, html);
            Assert.Contains(user1.Username, html);
            Assert.Contains(user2.Username, html);
            Assert.DoesNotContain(user1.Email, html);
            Assert.DoesNotContain(user2.Email, html);
        }

        [Fact]
        public async Task New_ReturnsViewWithNewUserForm()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/users");
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users\">", html);
            Assert.Contains("input", html);
            Assert.Contains("label", html);
            Assert.Contains("Username", html);
            Assert.Contains("Email", html);
            Assert.Contains("Name", html);
            Assert.Contains("Password", html);
        }

        [Fact]
        public async Task Create_AddsNewUserToDbAndRedirectsToIndexViewWithNewUser()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();

            var user1 = new User { Name = "John Doe", Username = "jdoe", Email = "john@gmail.com", Password = "abcdefg" };
            var user2 = new User{Name = "Jane Doe", Username = "j_doe", Email = "jane@gmail.com", Password = "abdefg"};
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"Name", "Jane Doe"},
                {"Username", "j_doe" },
                {"Email", "jane@gmail.com" },
                {"Password", "abdefg" }
            };

            var response = await client.PostAsync("/users", new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("Jane Doe", html);
            Assert.Contains("j_doe", html);
        }
    }
}