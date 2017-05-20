using System;
using System.IO;
using System.Security.Cryptography;

namespace Binder.Nsudotnet.Enigma
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (0 == args.Length)
                {
                    throw new ArgumentException("Invalid number of arguments");
                }

                switch (args[0])
                {
                    case "encrypt":
                        {
                            if (4 != args.Length)
                            {
                                throw new ArgumentException("Invalid number of arguments");
                            }

                            var inputFile = new FileInfo(args[1]);
                            var outputFile = new FileInfo(args[3]);

                            if (!inputFile.Exists)
                            {
                                throw new ArgumentException("The file doesn't exist");
                            }

                            var algorithm = args[2].ToLower();

                            Encrypt(inputFile, outputFile, algorithm);

                            break;
                        }
                    case "decrypt":
                        {
                            if (5 != args.Length)
                            {
                                throw new ArgumentException("Invalid number of arguments");
                            }

                            var inputFile = new FileInfo(args[1]);
                            var keyFile = new FileInfo(args[3]);
                            var outputFile = new FileInfo(args[4]);

                            if (!inputFile.Exists)
                            {
                                throw new ArgumentException("The file doesn't exist");
                            }

                            var algorithm = args[2].ToLower();

                            Decrypt(inputFile, keyFile, outputFile, algorithm);

                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Invalid command: must be \"encrypt\" or \"decrypt\"");
                        }
                }

                Console.WriteLine("Done");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        private static void Encrypt(FileInfo inputFile, FileInfo outputFile, string algorithm)
        {
            SymmetricAlgorithm symmetricAlgorithm;

            switch (algorithm)
            {
                case "aes":
                    symmetricAlgorithm = Aes.Create();
                    break;
                case "des":
                    symmetricAlgorithm = DES.Create();
                    break;
                case "rc2":
                    symmetricAlgorithm = RC2.Create();
                    break;
                case "rijndael":
                    symmetricAlgorithm = Rijndael.Create();
                    break;
                default:
                    throw new ArgumentException("Invalid algorithm identifier");
            }

            if (symmetricAlgorithm == null) return;

            var encryptor = symmetricAlgorithm.CreateEncryptor();
            var iv = symmetricAlgorithm.IV;
            var key = symmetricAlgorithm.Key;

            using (var infs = inputFile.OpenRead())
            {
                using (var outfs = outputFile.Create())
                {
                    using (var cs = new CryptoStream(outfs, encryptor, CryptoStreamMode.Write))
                    {
                        var bytes = new byte[64 * 1024];
                        int read;

                        while ((read = infs.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            cs.Write(bytes, 0, read);
                        }
                    }
                }
            }

            var keyFile = new FileInfo("file.key.txt");
            using (var fs = keyFile.Create())
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(Convert.ToBase64String(iv));
                    sw.WriteLine(Convert.ToBase64String(key));
                }
            }

            symmetricAlgorithm.Dispose();
        }

        private static void Decrypt(FileInfo inputFile, FileInfo keyFile, FileInfo outputFile, string algorithm)
        {
            SymmetricAlgorithm symmetricAlgorithm;

            switch (algorithm)
            {
                case "aes":
                    symmetricAlgorithm = Aes.Create();
                    break;
                case "des":
                    symmetricAlgorithm = DES.Create();
                    break;
                case "rc2":
                    symmetricAlgorithm = RC2.Create();
                    break;
                case "rijndael":
                    symmetricAlgorithm = Rijndael.Create();
                    break;
                default:
                    throw new ArgumentException("Invalid algorithm identifier");
            }

            if (symmetricAlgorithm == null) return;
            byte[] iv;
            byte[] key;

            using (var fs = keyFile.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var str = sr.ReadLine();
                    if (null == str)
                    {
                        throw new ArgumentException("Invalid key file");
                    }
                    iv = Convert.FromBase64String(str);
                    str = sr.ReadLine();
                    if (null == str)
                    {
                        throw new ArgumentException("Invalid key file");
                    }
                    key = Convert.FromBase64String(str);
                }
            }

            var decryptor = symmetricAlgorithm.CreateDecryptor(key, iv);

            using (var infs = inputFile.OpenRead())
            {
                using (var outfs = outputFile.Create())
                {
                    using (var cs = new CryptoStream(infs, decryptor, CryptoStreamMode.Read))
                    {
                        var bytes = new byte[64 * 1024];
                        int read;

                        while ((read = cs.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            outfs.Write(bytes, 0, read);
                        }
                    }
                }
            }

            symmetricAlgorithm.Dispose();
        }
    }
}
