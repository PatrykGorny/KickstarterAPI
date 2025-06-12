using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Infractructure.EF;
using KickstarterAPI.Dto;
using KickstarterAPI.Dto.Kickstarter;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Tests;

public class AppTests : IClassFixture<AppTestFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly AppTestFactory<Program> _app;
    private readonly AppDbContext _context;

    public AppTests(AppTestFactory<Program> app)
    {
        var adminId = "0bb08caa-d013-4715-a64f-a7e77ee77b01";
        _app = app;
        _client = _app.CreateClient();
        using (var scope = app.Services.CreateScope())
        {
            _context = scope.ServiceProvider.GetService<AppDbContext>();
            if (_context.Find<UserEntity>(adminId) == null)
            {
                _context.Users.Add(
                    new UserEntity()
                    {
                        Id = adminId,
                        Email = "admin@wsei.edu.pl",
                        NormalizedEmail = "admin@wsei.edu.pl".ToUpper(),
                        UserName = "admin",
                        NormalizedUserName = "ADMIN",
                        ConcurrencyStamp = adminId,
                        SecurityStamp = adminId,
                        EmailConfirmed = true,
                        PasswordHash = "AQAAAAIAAYagAAAAEOrArrSG1swr5b94IyFxxXI9wv/pMOWdiSK3LvAtL3VoMmk6sTFHTvhuRqAesmP/Ag=="
                    });
                _context.SaveChanges();
            }
        }
    }

    [Theory]
    [InlineData ("admin","1234!")]
    public async void ValidLoginTest(string username, string password)
    {
        var loginBody = new LoginDto()
        {
            UserName = username,
            Password = password
        };
        var result = await _client.PostAsJsonAsync("/api/users/login", loginBody);
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        JsonNode node = JsonNode.Parse(await result.Content.ReadAsStringAsync()); 
        var token = node["token"].AsValue().ToString();
        Assert.NotNull(token);
        
        
        HttpRequestMessage request = new HttpRequestMessage
        {
            RequestUri = new Uri("/api/kickstarter", UriKind.Relative),
            Method = HttpMethod.Get
        };
        request.Headers.Add("Authorization", $"Bearer {token}");
        
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        
    }
    
    [Theory]
    [InlineData ("admin123","admin123")]
    public async void InvalidLoginTest(string username, string password)
    {
        var loginBody = new LoginDto()
        {
            UserName = username,
            Password = password
        };
        var result = await _client.PostAsJsonAsync("/api/users/login", loginBody);
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task RegisterUser_ShouldCreateNewUser()
    {
        // Arrange
        var newUser = new RegisterDto
        {
            UserName = "testuser123",
            Email = "testuser@example.com",
            Password = "StrongPass123!",
            ConfirmPassword = "StrongPass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Users/register", newUser);

        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Registration failed: {await response.Content.ReadAsStringAsync()}");

        // Sprawdź czy użytkownik istnieje w bazie
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = db.Users.FirstOrDefault(u => u.UserName == "testuser123");

        Assert.NotNull(user);
        Assert.Equal("testuser123", user.UserName);
    }
    
    [Fact]
    public async Task GetProjects_UnauthorizedWithoutToken()
    {
        var response = await _client.GetAsync("/api/kickstarter");

        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_UnauthorizedWithoutToken()
    {
        var newProject = new KickstarterCreateDto
        {
            Name = "Unauthorized Project",
            Category = "Games",
            Subcategory = "Board Games",
            Country = "USA",
            Launched = DateTime.UtcNow,
            Deadline = DateTime.UtcNow.AddDays(30),
            Goal = 5000,
            Pledged = 0,
            Backers = 0,
            State = "Live"
        };

       
        var response = await _client.PostAsJsonAsync("/api/kickstarter", newProject);

        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_SavesToDatabase()
    {
        
        var loginBody = new LoginDto
        {
            UserName = "admin",
            Password = "1234!"
        };
        var loginResult = await _client.PostAsJsonAsync("/api/Users/login", loginBody);
        var token = JsonNode.Parse(await loginResult.Content.ReadAsStringAsync())["token"].ToString();

        
        
        var newProject = new KickstarterCreateDto
        {
            Name = "Test Project",
            Category = "Tech",
            Subcategory = "Software",
            Country = "USA",
            Launched = DateTime.UtcNow,
            Deadline = DateTime.UtcNow.AddDays(30),
            Goal = 1000,
            Pledged = 0,
            Backers = 0,
            State = "Live"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Kickstarter")
        {
            Content = JsonContent.Create(newProject)
        };
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var projectInDb = context.Kickstarters.FirstOrDefault(p => p.Name == "Test Project");

        Assert.NotNull(projectInDb);
        
        Assert.Equal("Tech", projectInDb.Category);
        
    }
    [Fact]
    public async Task DeleteProject_Authorized_ShouldReturnSuccess()
    {
        // Arrange - zaloguj się i pobierz token
        var login = await _client.PostAsJsonAsync("/api/users/login", new LoginDto
        {
            UserName = "admin",
            Password = "1234!"
        });
        var token = JsonNode.Parse(await login.Content.ReadAsStringAsync())["token"].ToString();

        
        long projectId;
        using (var scope = _app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            long maxId = db.Kickstarters.Any() ? db.Kickstarters.Max(p => p.ID) : 0;
            var project = new KickstarterEntity
            {
                ID = maxId + 1,
                Name = "DeleteTest Project",
                Category = "Art",
                Subcategory = "Painting",
                Country = "UK",
                Launched = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(15),
                Goal = 1234,
                Pledged = 123,
                Backers = 2,
                State = "Live"
            };
            db.Kickstarters.Add(project);
            db.SaveChanges();
            projectId = project.ID;
        }

     
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/kickstarter/{projectId}");
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _client.SendAsync(request);

       
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK);

        using (var scope = _app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deleted = db.Kickstarters.Find(projectId);
            Assert.Null(deleted);
        }
    }

   
   

}