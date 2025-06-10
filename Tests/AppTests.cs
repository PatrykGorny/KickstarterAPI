using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Infractructure.EF;
using KickstarterAPI.Dto;
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


    // [Fact]
    // public async void TestKickstarterControllerUnauthorized()
    // {
    //     var response =await  _client.GetAsync("/api/Kickstarter");
    //     Assert.Equal( HttpStatusCode.Unauthorized,response.StatusCode);
    // }
}