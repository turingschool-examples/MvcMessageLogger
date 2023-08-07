﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.FeatureTests
{
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
        public async Task Index_DisplaysAllUsers()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync("/users");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.Contains(user2.UserName, html);

        }

        [Fact]
        public async Task Show_DisplaysUsersPage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var user = new User { UserName = "eli", Email = "eli@Yahoo.com", Password = "password123" };
            var user2 = new User { UserName = "Zoro", Email = "Zoro@Yahoo.com", Password = "sword" };

            context.Users.Add(user);
            context.Users.Add(user2);
            context.SaveChanges();

            var response = await client.GetAsync($"/users/account/{user.Id}");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains(user.UserName, html);
            Assert.DoesNotContain(user2.UserName, html);

        }

        [Fact]
        public async Task New_DisplaysFormToCreateNewAccount()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/users/newaccount");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users/newaccount\">", html);
            Assert.Contains("<input type=\"password\" id=\"Password\" name=\"Password\" />", html);
            Assert.Contains("<input type=\"email\" id=\"Email\" name=\"Email\" />", html);
            Assert.Contains("<input type=\"text\" id=\"UserName\" name=\"UserName\" />", html);
        }


        [Fact]
        public async Task Create_AddsUserToDataBase()
        {
            var client = _factory.CreateClient();

            var formData = new Dictionary<string, string>
            {
                {"UserName","Usopp" },
                {"Email", "Usopp@hotmail" },
                {"Password", "sniperking" }
            };

            var response = await client.PostAsync($"users/newaccount", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Usopp", html);
        }

        [Fact]
        public async Task Login_DisplaysLoginPage()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/users/login");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<form method=\"post\" action=\"/users/login/\">", html);
        }

        

        [Fact]
        public async Task Signin_RedirectsUserToAccountPage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();


            var user = new User { UserName = "Usopp", Email = "Usopp@hotmail", Password = "sniperking" };
            context.Users.Add(user);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                {"UserName","Usopp" },
                {"Email", "Usopp@hotmail" },
                {"Password", "sniperking" }
            };

            var response = await client.PostAsync($"users/account/1", new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Usopp",html);
        }
    }
}