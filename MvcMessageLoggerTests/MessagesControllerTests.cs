using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.Models;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.FeatureTests;
using System.Runtime.CompilerServices;

namespace MvcMessageLoggerTests
{
    [Collection("Messages Controller Tests")]
    public class MvcMessageLoggerMessagesTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public MvcMessageLoggerMessagesTest(WebApplicationFactory<Program> factory)
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
        public async Task New_ShowsForm()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user1 = new User { Name = "John", Username = "Doe" };

            var message = new Message { Content = "hello", CreatedAt = new DateTime(2000, 2, 1).ToUniversalTime() };

            user1.Messages.Add(message);
            context.Users.Add(user1);
            context.SaveChanges();

            var response = await client.GetAsync("/users/1/messages/new");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

        }
        [Fact]
        public async Task CreateMessage_AddsMessageToDB()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();
            var user = new User { Name = "John", Username = "Doe" };
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"Content","Hello" }
            };


            var response = await client.PostAsync($"/users/{user.Id}/messages", new FormUrlEncodedContent(formData));

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Hello", html);
        }
    }
}
