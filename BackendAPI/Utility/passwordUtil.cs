using Microsoft.AspNetCore.Identity;

namespace BackendAPI.Helpers;

public static class PasswordHelper
{
    private static readonly PasswordHasher<string> _hasher = new();

    public static string HashPassword(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public static bool Verify(string hashedPassword, string providedPassword)
    {
        return _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword)
               == PasswordVerificationResult.Success;
    }
}
