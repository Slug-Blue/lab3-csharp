using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab3.Auth.Services;

public record AuthResult(bool Success, string Message, AuthErrorCode ErrorCode = AuthErrorCode.None);

public interface IAuthService
{
    AuthResult Register(string username, string password);
    AuthResult Login(string username, string password);
    AuthResult ChangePassword(string username, string oldPassword, string newPassword);
    IReadOnlyList<string> GetUsers();
    void ClearUsers(); // для тестов
}

public class AuthService : IAuthService
{
    // Потокобезопасное хранилище: {username: hashedPassword}
    private readonly ConcurrentDictionary<string, string> _users = new();

    private static string Hash(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    private static bool IsValidUsername(string username) =>
        Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");

    private static bool IsValidPassword(string password) =>
        password.Length >= 6;

    public AuthResult Register(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new AuthResult(false, "Логин и пароль обязательны");

        if (!IsValidUsername(username))
            return new AuthResult(false, "Логин: 3–20 символов, буквы/цифры/подчёркивание");

        if (!IsValidPassword(password))
            return new AuthResult(false, "Пароль: минимум 6 символов");

        if (_users.ContainsKey(username))
            return new AuthResult(false, "Пользователь уже существует");

        _users[username] = Hash(password);
        return new AuthResult(true, "Регистрация успешна");
    }

    public AuthResult Login(string username, string password)
    {
        if (!_users.TryGetValue(username, out var stored) || stored != Hash(password))
            return new AuthResult(false, "Неверный логин или пароль");

        return new AuthResult(true, $"Добро пожаловать, {username}!");
    }

    public AuthResult ChangePassword(string username, string oldPassword, string newPassword)
    {
        if (!_users.TryGetValue(username, out var stored))
            return new AuthResult(false, "Пользователь не найден");

        if (stored != Hash(oldPassword))
            return new AuthResult(false, "Неверный текущий пароль");

        if (!IsValidPassword(newPassword))
            return new AuthResult(false, "Пароль: минимум 6 символов");

        if (oldPassword == newPassword)
            return new AuthResult(false, "Новый пароль совпадает со старым");

        _users[username] = Hash(newPassword);
        return new AuthResult(true, "Пароль успешно изменён");
    }
    public AuthResult Register(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new AuthResult(false, "Логин и пароль обязательны", AuthErrorCode.InvalidCredentials);

        if (!IsValidUsername(username))
            return new AuthResult(false, "Логин: 3–20 символов...", AuthErrorCode.InvalidCredentials);

        if (!IsValidPassword(password))
            return new AuthResult(false, "Пароль: минимум 6 символов", AuthErrorCode.InvalidCredentials);

        if (_users.ContainsKey(username))
            return new AuthResult(false, "Пользователь уже существует", AuthErrorCode.UserAlreadyExists);

        _users[username] = Hash(password);
        return new AuthResult(true, "Регистрация успешна", AuthErrorCode.None);
    }
    public AuthResult Login(string username, string password)
    {
        if (!_users.TryGetValue(username, out var stored) || stored != Hash(password))
            return new AuthResult(false, "Неверный логин или пароль", AuthErrorCode.InvalidCredentials);

        return new AuthResult(true, $"Добро пожаловать, {username}!", AuthErrorCode.None);
    }
    public AuthResult ChangePassword(string username, string oldPassword, string newPassword)
    {
        if (!_users.TryGetValue(username, out var stored))
            return new AuthResult(false, "Пользователь не найден", AuthErrorCode.InvalidCredentials);

        if (stored != Hash(oldPassword))
            return new AuthResult(false, "Неверный текущий пароль", AuthErrorCode.InvalidCredentials);

        if (!IsValidPassword(newPassword))
            return new AuthResult(false, "Пароль: минимум 6 символов", AuthErrorCode.InvalidCredentials);

        if (oldPassword == newPassword)
            return new AuthResult(false, "Новый пароль совпадает со старым", AuthErrorCode.PasswordsDoNotMatch);

        _users[username] = Hash(newPassword);
        return new AuthResult(true, "Пароль успешно изменён", AuthErrorCode.None);
    }
    public IReadOnlyList<string> GetUsers() => _users.Keys.ToList();

    public void ClearUsers() => _users.Clear();
}
