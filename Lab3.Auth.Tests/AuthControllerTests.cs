// Lab3.Auth.Tests/AuthControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using Lab3.Auth.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lab3.Auth.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly IAuthService _auth;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        var app = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddSingleton<IAuthService, AuthService>()));

        _client = app.CreateClient();
        _auth = app.Services.GetRequiredService<IAuthService>();
    }

    public void Dispose() => _auth.ClearUsers();

    // ── POST /api/register ───────────────────────────────────────────────────

    [Fact]
    public async Task Register_ValidData_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/register",
            new { Username = "alice", Password = "secret123" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_Duplicate_Returns400()
    {
        await _client.PostAsJsonAsync("/api/register",
            new { Username = "alice", Password = "secret123" });
        var response = await _client.PostAsJsonAsync("/api/register",
            new { Username = "alice", Password = "secret123" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── POST /api/login ──────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        await _client.PostAsJsonAsync("/api/register",
            new { Username = "bob", Password = "mypassword" });
        var response = await _client.PostAsJsonAsync("/api/login",
            new { Username = "bob", Password = "mypassword" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        await _client.PostAsJsonAsync("/api/register",
            new { Username = "bob", Password = "mypassword" });
        var response = await _client.PostAsJsonAsync("/api/login",
            new { Username = "bob", Password = "wrong!" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── POST /api/change-password ────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_ValidData_Returns200()
    {
        await _client.PostAsJsonAsync("/api/register",
            new { Username = "carol", Password = "oldpass123" });
        var response = await _client.PostAsJsonAsync("/api/change-password", new
        {
            Username = "carol",
            OldPassword = "oldpass123",
            NewPassword = "newpass456"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
