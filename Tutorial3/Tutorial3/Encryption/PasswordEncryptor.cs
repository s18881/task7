using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Tutorial3.Encryption
{
    public class PasswordEncryptor
    {
        public static List<string> Encrypt (string password)
        {
            var salt = CreateSalt();
            var encrypted = Create(password, salt);
            var list = new List<string>();
            list.Add(encrypted);
            list.Add(salt);
            return list;
        }
        public static string Create(string pass, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: pass,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using(var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static Boolean Validate(string pass, string salt, string encrypted)
            => Create(pass, salt) == encrypted;
    }
}