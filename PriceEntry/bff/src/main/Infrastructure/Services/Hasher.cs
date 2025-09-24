namespace Infrastructure.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Hasher
    {
        public static string GetSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
