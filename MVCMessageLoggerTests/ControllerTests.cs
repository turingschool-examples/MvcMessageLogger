using System.Net;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using MvcMessageLogger.FeatureTests;

namespace MVCMessageLoggerTest
{


	public class ControllerTests : IClassFixture<WebApplicationFactory<Program>>
	{

		private readonly WebApplicationFactory<Program> _factory;

		public ControllerTests(WebApplicationFactory<Program> factory)
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
		public async void Index_ReturnsViewOfListOfUsers()
		{
			var context = GetDbContext();
			context.Users.Add(new User { Name = "Isiah Worsham", Username = "15iworsham" });
			context.SaveChanges();

			var client = _factory.CreateClient();
			var response = await client.GetAsync("/users");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Isiah", html);

		}
		[Fact]
		public async Task New_ReturnsViewWithForm()
		{
			var context = GetDbContext();
			var client = _factory.CreateClient();

			var response = await client.GetAsync("/users/new");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Name:", html);
			Assert.Contains("Username:", html);
			Assert.Contains($"<form method=\"post\" action=\"/users/create\">", html);
		}
		[Fact]
		public async Task Show_ReturnsViewWithFormToAddMessage()
		{
			var context = GetDbContext();
			context.Users.Add(new User { Name = "Isiah Worsham", Username = "15iworsham" });
			context.Messages.Add(new Message("Hi"));
			context.SaveChanges();
			var client = _factory.CreateClient();

			var response = await client.GetAsync("/users/1");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Isiah Worsham", html);
			Assert.Contains("Username:", html);
			Assert.Contains("Message", html);



		}
		[Fact]
		public async void Index_HasButtonToLogIn()
		{
			var context = GetDbContext();
			context.Users.Add(new User { Name = "Isiah Worsham", Username = "15iworsham" });
			context.SaveChanges();

			var client = _factory.CreateClient();
			var response = await client.GetAsync("/users");
			var html = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
			Assert.Contains("Login", html);

		}
	}
}