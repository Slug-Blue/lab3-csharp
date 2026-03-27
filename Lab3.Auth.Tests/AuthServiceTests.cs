// Lab3.Auth.Tests/AuthServiceTests.cs
using Lab3.Auth.Services;
using Xunit;

namespace Lab3.Auth.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly IAuthService _svc = new AuthService();

    // IDisposable: очистка после каждого теста
    public void Dispose() => _svc.ClearUsers();

    // ── Регистрация ──────────────────────────────────────────────────────────

    [Fact]
    public void Register_ValidData_ReturnsSuccess()
    {
        var result = _svc.Register("alice", "secret123");
        Assert.True(result.Success);
    }

    [Fact]
    public void Register_Duplicate_ReturnsFail()
    {
        _svc.Register("alice", "secret123");
        var result = _svc.Register("alice", "other123");
        Assert.False(result.Success);
        Assert.Equal(AuthErrorCode.UserAlreadyExists, result.ErrorCode); ;
    }

    [Fact]
    public void Register_ShortPassword_ReturnsFail()
    {
        var result = _svc.Register("bob", "123");
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData("", "password123")]   // пустой логин
    [InlineData("ab", "password123")] // логин слишком короткий
    [InlineData("bad user!", "pass123")] // недопустимые символы
    public void Register_InvalidUsername_ReturnsFail(string username, string password)
    {
        var result = _svc.Register(username, password);
        Assert.False(result.Success);
    }

    // ── Авторизация ──────────────────────────────────────────────────────────

    [Fact]
    public void Login_ValidCredentials_ReturnsSuccess()
    {
        _svc.Register("alice", "secret123");
        var result = _svc.Login("alice", "secret123");
        Assert.True(result.Success);
        Assert.Contains("alice", result.Message);
    }

    [Fact]
    public void Login_WrongPassword_ReturnsFail()
    {
        _svc.Register("alice", "secret123");
        var result = _svc.Login("alice", "wrongpass");
        Assert.False(result.Success);
    }

    [Fact]
    public void Login_UnknownUser_ReturnsFail()
    {
        var result = _svc.Login("ghost", "password123");
        Assert.False(result.Success);
    }

    // ── Смена пароля ─────────────────────────────────────────────────────────

    [Fact]
    public void ChangePassword_ValidData_ReturnsSuccess()
    {
        _svc.Register("carol", "oldpass123");
        var result = _svc.ChangePassword("carol", "oldpass123", "newpass456");
        Assert.True(result.Success);
        // Новый пароль должен работать
        Assert.True(_svc.Login("carol", "newpass456").Success);
    }

    [Fact]
    public void ChangePassword_WrongOldPassword_ReturnsFail()
    {
        _svc.Register("carol", "oldpass123");
        var result = _svc.ChangePassword("carol", "WRONG", "newpass456");
        Assert.False(result.Success);
    }

    [Fact]
    public void ChangePassword_SameAsOld_ReturnsFail()
    {
        _svc.Register("carol", "oldpass123");
        var result = _svc.ChangePassword("carol", "oldpass123", "oldpass123");
        Assert.False(result.Success);
        Assert.Equal(AuthErrorCode.PasswordsDoNotMatch, result.ErrorCode); ;
    }

    [Fact]
    public void ChangePassword_UnknownUser_ReturnsFail()
    {
        var result = _svc.ChangePassword("ghost", "old123", "new123456");
        Assert.False(result.Success);
    }
}
