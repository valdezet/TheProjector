using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;


namespace TheProjector.Services;

public class PasswordService
{
    private static int _iterations = 600000;

    private static int _saltBytes = 128 / 8;

    private static int _derivedKeyBytes = 256 / 8;

    public bool VerifyPassword(string inputPassword, string storedPasswordHashBase64)
    {
        if (String.IsNullOrEmpty(inputPassword))
        {
            return false;
        }
        byte[] storedPasswordHashBytes = Convert.FromBase64String(storedPasswordHashBase64);
        byte[] storedPasswordHashSalt = GetSalt(storedPasswordHashBytes);
        byte[] inputPasswordHashBytes = MakeHashedPassword(storedPasswordHashSalt, inputPassword);
        bool result = inputPasswordHashBytes.SequenceEqual(storedPasswordHashBytes);
        return inputPasswordHashBytes.SequenceEqual(storedPasswordHashBytes);
    }

    public byte[] MakeHashedPassword(byte[] salt, string plaintextPassword)
    {
        byte[] derivedKey = KeyDerivation.Pbkdf2(
            plaintextPassword,
            salt,
            KeyDerivationPrf.HMACSHA512,
            _iterations,
            _derivedKeyBytes
        );
        byte[] output = new byte[salt.Length + derivedKey.Length];
        Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
        Buffer.BlockCopy(derivedKey, 0, output, salt.Length, derivedKey.Length);
        return output;
    }


    public byte[] MakeHashedPassword(string plaintextPassword)
    {
        byte[] salt = GenerateSalt();
        return MakeHashedPassword(salt, plaintextPassword);
    }

    public byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(_saltBytes);

    }

    public byte[] GetSalt(byte[] passwordHash)
    {
        byte[] salt = new byte[_saltBytes];
        Buffer.BlockCopy(passwordHash, 0, salt, 0, _saltBytes);
        return salt;
    }




}