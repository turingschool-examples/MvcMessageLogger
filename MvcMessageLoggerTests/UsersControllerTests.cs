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
        public async Task Index_ShowsUsers()
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
        public async Task Create_AddsUserToDB()
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

        [Fact]
        public async Task Details_ShowsAllMessagesForUser()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user1 = new User { Name = "John", Username = "Doe" };

            var message = new Message { Content = "hello", CreatedAt = new DateTime(2000, 2, 1).ToUniversalTime() };

            user1.Messages.Add(message);
            context.Users.Add(user1);
            context.SaveChanges();

            var response = await client.GetAsync("/users/details/1");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Doe", html);
            Assert.Contains("hello", html);

        }

        //[Fact]
        //public async Task NewMessage_ShowsForm()
        //{
        //    var context = GetDbContext();
        //    var client = _factory.CreateClient();

        //    var user1 = new User { Name = "John", Username = "Doe" };

        //    var message = new Message { Content = "hello", CreatedAt = new DateTime(2000, 2, 1).ToUniversalTime() };

        //    user1.Messages.Add(message);
        //    context.Users.Add(user1);
        //    context.SaveChanges();

        //    var response = await client.GetAsync("/users/1/messages/new");
        //    response.EnsureSuccessStatusCode();

        //    var html = await response.Content.ReadAsStringAsync();

        //}

        //[Fact]
        //public async Task CreateMessage_AddsMessageToDB()
        //{
        //    var context = GetDbContext();
        //    var client = _factory.CreateClient();
        //    var user = new User { Name = "John", Username = "Doe" };
        //    context.Users.Add(user);
        //    context.SaveChanges();

        //    var formData = new Dictionary<string, string>
        //    {
        //        {"Content","Hello" }
        //    };


        //    var response = await client.PostAsync($"/users/{user.Id}/messages", new FormUrlEncodedContent(formData));
  
        //    var html = await response.Content.ReadAsStringAsync();

        //    Assert.Contains("Hello", html);
        //}
    }
}