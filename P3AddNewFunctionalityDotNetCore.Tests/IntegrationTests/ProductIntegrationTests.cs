using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.IntegrationTests
{
    public class ProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private static bool _seeded = false;

        public ProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            if (!_seeded)
            {
                _factory.SeedDatabase();
                _factory.SeedAdminUser().Wait();
                _seeded = true;
            }
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private async Task<string> GetAntiForgeryToken(string url)
        {
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var match = System.Text.RegularExpressions.Regex.Match(
                content,
                @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"""
            );
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private async Task LoginAsAdmin()
        {
            var token = await GetAntiForgeryToken("/Account/Login");
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Admin"),
                new KeyValuePair<string, string>("Password", "P@ssword123"),
                new KeyValuePair<string, string>("ReturnUrl", "/Product/Admin"),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });
            await _client.PostAsync("/Account/Login", loginData);
        }

        // TEST 1 : La page Admin redirige vers login si non authentifié
        [Fact]
        public async Task AdminPage_RedirectsToLogin_WhenNotAuthenticated()
        {
            // ACT
            var response = await _client.GetAsync("/Product/Admin");

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        // TEST 2 : Un admin peut se connecter
        [Fact]
        public async Task Admin_CanLogin_WithValidCredentials()
        {
            // ARRANGE
            var token = await GetAntiForgeryToken("/Account/Login");

            // ACT
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Admin"),
                new KeyValuePair<string, string>("Password", "P@ssword123"),
                new KeyValuePair<string, string>("ReturnUrl", "/Product/Admin"),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });
            var response = await _client.PostAsync("/Account/Login", loginData);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        // TEST 3 : Un admin peut ajouter un produit et il apparaît côté client
        [Fact]
        public async Task Admin_AddProduct_AppearsInClientList()
        {
            // ARRANGE
            await LoginAsAdmin();
            var token = await GetAntiForgeryToken("/Product/Create");

            // ACT
            var createData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Monitor"),
                new KeyValuePair<string, string>("Price", "300"),
                new KeyValuePair<string, string>("Stock", "5"),
                new KeyValuePair<string, string>("Description", "Ecran 27 pouces"),
                new KeyValuePair<string, string>("Details", "4K HDR"),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });
            var createResponse = await _client.PostAsync("/Product/Create", createData);

            // ASSERT 1 - La création redirige bien
            createResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

            // ASSERT 2 - Le produit apparaît côté client
            var clientResponse = await _client.GetAsync("/Product/Index");
            var content = await clientResponse.Content.ReadAsStringAsync();
            content.Should().Contain("Monitor");
        }

        // TEST 4 : Un admin peut supprimer un produit et il disparaît côté client
        [Fact]
        public async Task Admin_DeleteProduct_DisappearsFromClientList()
        {
            // ARRANGE - Connexion Admin
            await LoginAsAdmin();

            // Récupérer la page Admin et chercher "ProductToDelete" par son nom
            var adminResponse = await _client.GetAsync("/Product/Admin");
            var adminContent = await adminResponse.Content.ReadAsStringAsync();

            // Chercher l'Id du produit par son nom
            var match = System.Text.RegularExpressions.Regex.Match(
                adminContent,
                @"Laptop.*?<form id=""(\d+)""",
                System.Text.RegularExpressions.RegexOptions.Singleline
            );
            var productId = match.Groups[1].Value;

            // ACT - Supprimer le produit
            var tokenDelete = await GetAntiForgeryToken("/Product/Admin");
            var deleteData = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("id", productId),
        new KeyValuePair<string, string>("__RequestVerificationToken", tokenDelete)
    });
            var deleteResponse = await _client.PostAsync("/Product/DeleteProduct", deleteData);

            // ASSERT 1 - La suppression redirige bien
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

            // ASSERT 2 - Le produit n'apparaît plus côté client
            var clientResponse = await _client.GetAsync("/Product/Index");
            var content = await clientResponse.Content.ReadAsStringAsync();
            content.Should().NotContain("Laptop");
        }
    }
}