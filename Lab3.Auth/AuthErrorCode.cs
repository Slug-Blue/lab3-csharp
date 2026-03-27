namespace Lab3.Auth.Services;

public enum AuthErrorCode
{
    None = 0,
    UserAlreadyExists = 1,
    PasswordsDoNotMatch = 2,
    InvalidCredentials = 3
}