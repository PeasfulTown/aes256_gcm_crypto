using System;
using System.IO;

namespace aes256_gcm_encryption_tool;

class Program
{
    static void Main(string[] args)
    {
        string mode = "";
        string value = "";
        string keyPath = "";

        for (int i = 0; i < args.Length; ++i)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--mode":
                case "-m":
                    if (i + 1 < args.Length)
                    {
                        mode = args[i + 1];
                        ++i;
                    }
                    break;
                case "--value":
                case "-v":
                    if (i + 1 < args.Length)
                    {
                        value = args[i + 1];
                        ++i;
                    }
                    break;
                case "--key":
                case "-k":
                    if (i + 1 < args.Length)
                    {
                        keyPath = args[i + 1];
                        ++i;
                    }
                    break;
                case "--help":
                case "-h":
                    PrintUsage();
                    Environment.ExitCode = 0;
                    return;
            }
        }

        if (string.IsNullOrEmpty(mode))
        {
            Console.WriteLine("Error: Mode is required.");
            PrintUsage();
            Environment.ExitCode = 1;
            return;
        }

        try
        {
            switch (mode.ToLowerInvariant())
            {
                case "gen":
                    string newKeyBase64 = Aes256Crypto.GenerateKeyBase64();
                    if (string.IsNullOrEmpty(keyPath))
                    {
                        Console.WriteLine(newKeyBase64);
                    }
                    else
                    {
                        File.WriteAllText(keyPath, newKeyBase64);
                        Console.WriteLine($"Key successfully written to {keyPath}");
                    }
                    break;

                case "encrypt":
                    if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(keyPath))
                    {
                        Console.WriteLine("Error: Both --value and --key are required for encryption.");
                        Environment.ExitCode = 1;
                        return;
                    }
                    string encKeyBase64 = File.ReadAllText(keyPath).Trim();
                    byte[] encKey = Convert.FromBase64String(encKeyBase64);
                    string encrypted = Aes256Crypto.Encrypt(value, encKey);
                    Console.WriteLine(encrypted);
                    break;

                case "decrypt":
                    if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(keyPath))
                    {
                        Console.WriteLine("Error: Both --value and --key are required for decryption.");
                        Environment.ExitCode = 1;
                        return;
                    }
                    string decKeyBase64 = File.ReadAllText(keyPath).Trim();
                    byte[] decKey = Convert.FromBase64String(decKeyBase64);
                    string decrypted = Aes256Crypto.Decrypt(value, decKey);
                    Console.WriteLine(decrypted);
                    break;

                default:
                    Console.WriteLine($"Error: Unknown mode '{mode}'. Valid modes are 'encrypt', 'decrypt', or 'gen'.");
                    Environment.ExitCode = 1;
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run -- --mode [encrypt/decrypt/gen] --value [value] --key [path to keyfile]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("  -m, --mode     Mode of operation: 'gen', 'encrypt', or 'decrypt'");
        Console.WriteLine("  -v, --value    The plain text to encrypt, or the ciphertext to decrypt");
        Console.WriteLine("  -k, --key      Path to the file containing the Base64 AES-256 key");
        Console.WriteLine("                 (For 'gen' mode, if provided, the key will be written to this file)");
        Console.WriteLine("  -h, --help     Show this help message");
    }
}
