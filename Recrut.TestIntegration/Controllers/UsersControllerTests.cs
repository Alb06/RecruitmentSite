using Microsoft.AspNetCore.Mvc.Testing;
using Recrut.API.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace Recrut.API.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private string _adminToken;

        public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Nous allons obtenir un token d'authentification pour les tests qui en ont besoin
            SetupAdminAuthentication().Wait();
        }

        private async Task SetupAdminAuthentication()
        {
            // Login pour obtenir un token
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResultDto>(content, _jsonOptions);

            _adminToken = result.Token;
        }

        private void AuthenticateClient()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
        }

        [Fact]
        public async Task GetUserByEmail_ExistingEmail_ReturnsUser()
        {
            // Arrange
            AuthenticateClient();
            var email = "user@example.com";

            // Act
            var response = await _client.GetAsync($"/api/users/{email}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserResponseDto>(content, _jsonOptions);

            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
            Assert.Equal("Regular User", user.Name);
            //Assert.Contains("User", user.Roles);
        }

        [Fact]
        public async Task GetUserByEmail_NonExistingEmail_ReturnsNotFound()
        {
            // Arrange
            AuthenticateClient();
            var email = "nonexistent@example.com";

            // Act
            var response = await _client.GetAsync($"/api/users/{email}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUsersByIds_ExistingIds_ReturnsUsers()
        {
            // Arrange
            AuthenticateClient();
            var ids = new[] { 1, 2 };

            // Act
            var response = await _client.GetAsync($"/api/users/byIds?ids={ids[0]}&ids={ids[1]}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<UserResponseDto>>(content, _jsonOptions);

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Id == 1);
            Assert.Contains(users, u => u.Id == 2);
        }

        [Fact]
        public async Task CreateUsers_ValidData_CreatesUsers()
        {
            // Arrange
            AuthenticateClient();
            var newUsers = new List<UserCreateDto>
            {
                new UserCreateDto
                {
                    Name = "New Test User",
                    Email = "newuser@example.com",
                    PasswordHash = "Password123!"
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users/createUsers", newUsers);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OperationResult>(content, _jsonOptions);

            Assert.NotNull(result);
            Assert.True(result.Success);

            // Vérifier que l'utilisateur a bien été créé
            var getUserResponse = await _client.GetAsync("/api/users/newuser@example.com");
            getUserResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateUser_ValidData_UpdatesUser()
        {
            // Arrange
            AuthenticateClient();
            var userId = 2; // ID de l'utilisateur régulier
            var updateDto = new UserUpdateDto
            {
                Name = "Updated User Name",
                Email = "user@example.com"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OperationResult>(content, _jsonOptions);

            Assert.NotNull(result);
            Assert.True(result.Success);

            // Vérifier que l'utilisateur a bien été mis à jour
            var getUserResponse = await _client.GetAsync("/api/users/user@example.com");
            getUserResponse.EnsureSuccessStatusCode();

            var userContent = await getUserResponse.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserResponseDto>(userContent, _jsonOptions);

            Assert.NotNull(user);
            Assert.Equal("Updated User Name", user.Name);
        }

        [Fact]
        public async Task DeleteUserByEmail_ExistingEmail_DeletesUser()
        {
            // Arrange - Créer d'abord un utilisateur à supprimer
            AuthenticateClient();
            var email = "todelete@example.com";

            var newUsers = new List<UserCreateDto>
            {
                new UserCreateDto
                {
                    Name = "User To Delete",
                    Email = email,
                    PasswordHash = "Password123!"
                }
            };

            await _client.PostAsJsonAsync("/api/users/createUsers", newUsers);

            // Act
            var response = await _client.DeleteAsync($"/api/users/{email}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OperationResult>(content, _jsonOptions);

            Assert.NotNull(result);
            Assert.True(result.Success);

            // Vérifier que l'utilisateur a bien été supprimé
            var getUserResponse = await _client.GetAsync($"/api/users/{email}");
            Assert.Equal(HttpStatusCode.NotFound, getUserResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteUsersByIds_ExistingIds_DeletesUsers()
        {
            // Arrange - Créer d'abord des utilisateurs à supprimer
            AuthenticateClient();

            var newUsers = new List<UserCreateDto>
            {
                new UserCreateDto
                {
                    Name = "User To Delete 1",
                    Email = "todelete1@example.com",
                    PasswordHash = "Password123!"
                },
                new UserCreateDto
                {
                    Name = "User To Delete 2",
                    Email = "todelete2@example.com",
                    PasswordHash = "Password123!"
                }
            };

            await _client.PostAsJsonAsync("/api/users/createUsers", newUsers);

            // Récupérer les IDs des utilisateurs créés
            var response1 = await _client.GetAsync("/api/users/todelete1@example.com");
            var content1 = await response1.Content.ReadAsStringAsync();
            var user1 = JsonSerializer.Deserialize<UserResponseDto>(content1, _jsonOptions);

            var response2 = await _client.GetAsync("/api/users/todelete2@example.com");
            var content2 = await response2.Content.ReadAsStringAsync();
            var user2 = JsonSerializer.Deserialize<UserResponseDto>(content2, _jsonOptions);

            var ids = new List<int> { user1.Id, user2.Id };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/users/byIds")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(ids),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };
            var deleteResponse = await _client.SendAsync(request);

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OperationResult>(deleteContent, _jsonOptions);

            Assert.NotNull(result);
            Assert.True(result.Success);

            // Vérifier que les utilisateurs ont bien été supprimés
            var getUserResponse1 = await _client.GetAsync("/api/users/todelete1@example.com");
            var getUserResponse2 = await _client.GetAsync("/api/users/todelete2@example.com");

            Assert.Equal(HttpStatusCode.NotFound, getUserResponse1.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getUserResponse2.StatusCode);
        }
    }
}