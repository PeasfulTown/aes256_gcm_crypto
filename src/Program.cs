using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace aes256_gcm_encryption_tool;

class Program
{
    static void Main(string[] args)
    {
        string mode = "";
        string value = "";
        string keyfile = "";
        string key = "";

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
                case "--keyfile":
                case "-f":
                    if (i + 1 < args.Length)
                    {
                        keyfile = args[i + 1];
                        ++i;
                    }
                    break;
                case "--key":
                case "-k":
                    if (i + 1 < args.Length)
                    {
                        key = args[i + 1];
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
            Console.Error.WriteLine("Error: --mode is required.");
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
                    if (string.IsNullOrEmpty(keyfile))
                    {
                        Console.WriteLine(newKeyBase64);
                    }
                    else
                    {
                        if (File.Exists(keyfile))
                        {
                             Console.Error.WriteLine($"Error: File already exists at {keyfile}. Don't want to " + 
                                 $"overwrite.");
                             Environment.ExitCode = 1;
                             return;
                        }
                            
                        File.WriteAllText(keyfile, newKeyBase64);
                        Console.WriteLine($"Key successfully written to {keyfile}");
                    }
                    break;

                case "encrypt":
                    if (string.IsNullOrEmpty(value))
                    {
                        Console.Error.WriteLine("Error: --value is needed for encryption.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        byte[] encKey = Convert.FromBase64String(key);
                        string encrypted = Aes256Crypto.Encrypt(value, encKey);
                        Console.WriteLine(encrypted);
                    }
                    else if (!string.IsNullOrEmpty(keyfile))
                    {
                        string encKeyBase64 = File.ReadAllText(keyfile).Trim();
                        byte[] encKey = Convert.FromBase64String(encKeyBase64);
                        string encrypted = Aes256Crypto.Encrypt(value, encKey);
                        Console.WriteLine(encrypted);
                    }
                    else
                    {
                        Console.Error.WriteLine("Error: Either --key or --keyfile is needed for encryption");
                        Environment.ExitCode = 1;
                        return;
                    }

                    break;

                case "decrypt":
                    if (string.IsNullOrEmpty(value))
                    {
                        Console.Error.WriteLine("Error: --value is needed for decryption.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        byte[] decKey = Convert.FromBase64String(key);
                        string decrypted = Aes256Crypto.Decrypt(value, decKey);
                        Console.WriteLine(decrypted);
                    }
                    else if (!string.IsNullOrEmpty(keyfile))
                    {
                        string decKeyBase64 = File.ReadAllText(keyfile).Trim();
                        byte[] decKey = Convert.FromBase64String(decKeyBase64);
                        string decrypted = Aes256Crypto.Decrypt(value, decKey);
                        Console.WriteLine(decrypted);
                    }
                    else
                    {
                        Console.Error.WriteLine("Error: Either --key or --keyfile is needed for decryption.");
                        Environment.ExitCode = 1;
                        return;
                    }
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
        string appName = AppDomain.CurrentDomain.FriendlyName;
        Console.WriteLine($"Usage: {appName} --mode [encrypt/decrypt/gen] --value [value] [--key <base64_key> | --keyfile <path>]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("  -m, --mode     Mode of operation: 'gen', 'encrypt', or 'decrypt'");
        Console.WriteLine("  -v, --value    The plain text to encrypt, or the ciphertext to decrypt");
        Console.WriteLine("  -k, --key      The Base64 AES-256 key string directly");
        Console.WriteLine("  -f, --keyfile  Path to the file containing the Base64 AES-256 key");
        Console.WriteLine("                 (For 'gen' mode, if provided, the key will be written to this file)");
        Console.WriteLine("  -h, --help     Show this help message");
    }
}
