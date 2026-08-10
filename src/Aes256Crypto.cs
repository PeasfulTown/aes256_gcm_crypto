using System.Security.Cryptography;
using System.Text;

namespace aes256_gcm_encryption_tool;

public class Aes256Crypto
{
    public static string Encrypt(string plainText, byte[] key)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce); // Generate a random Nonce

        byte[] cipherText = new byte[plainBytes.Length];
        byte[] tag = new byte[16];

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);

        // Combine: Nonce + Tag + CipherText
        byte[] encryptedPayload = new byte[nonce.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, encryptedPayload, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, encryptedPayload, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, encryptedPayload, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(encryptedPayload);
    }
    
    // Unpacks the string and decrypts it
    public static string Decrypt(string encryptedBase64, byte[] key)
    {
        byte[] encryptedPayload = Convert.FromBase64String(encryptedBase64);

        // Extract Nonce (First 12 bytes)
        byte[] nonce = new byte[12];
        Buffer.BlockCopy(encryptedPayload, 0, nonce, 0, nonce.Length);

        // Extract Tag (Next 16 bytes)
        byte[] tag = new byte[16];
        Buffer.BlockCopy(encryptedPayload, nonce.Length, tag, 0, tag.Length);

        // Extract CipherText (Everything else)
        byte[] cipherText = new byte[encryptedPayload.Length - nonce.Length - tag.Length];
        Buffer.BlockCopy(encryptedPayload, nonce.Length + tag.Length, cipherText, 0, cipherText.Length);

        byte[] plainBytes = new byte[cipherText.Length];

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Decrypt(nonce, cipherText, tag, plainBytes); // Will crash if Tag is invalid!

        return Encoding.UTF8.GetString(plainBytes);
    }

    // Generates a 256-bit (32 bytes) key and returns it as a Base64 string
    public static string GenerateKeyBase64()
    {
        byte[] key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }
}