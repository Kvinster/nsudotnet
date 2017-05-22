using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Binder.Nsudotnet.LinesCounter
{
    class Program
    {
        public string Extension { get; set; }
        public int LinesCount { get; set; }

        public Program(string extension)
        {
            Extension = extension;
        }

        public void Process()
        {
            ProcessDirectory(new DirectoryInfo("./"));

            Console.WriteLine(LinesCount);
        }

        static void Main(string[] args)
        {
            if (1 != args.Length)
            {
                Console.WriteLine("Error: invalid number of arguments");
            }
            else
            {
                var program = new Program(args[0]);
                program.Process();
            }

            Console.ReadKey();
        }

        private void ProcessDirectory(DirectoryInfo di)
        {
            foreach (var file in di.EnumerateFiles())
            {
                if (file.Extension.Equals(Extension))
                {
                    Console.WriteLine(file.Name);
                    ProcessFile(file);
                }
            }

            foreach (var dir in di.EnumerateDirectories())
            {
                ProcessDirectory(dir);
            }
        }

        private void ProcessFile(FileInfo fi)
        {
            if (!fi.Exists)
            {
                throw new ArgumentException(string.Format("The file {0} doesn't exist", fi.FullName));
            }

            using (var sr = fi.OpenText())
            {
                string currentString;
                var multilineComment = false;
                var str = false;

                while (null != (currentString = sr.ReadLine()))
                {
                    currentString = currentString.Trim();

                    if (currentString.Equals(string.Empty))
                    {
                        continue;
                    }
                    if (currentString.StartsWith("//"))
                    {
                        continue;
                    }
                    while (currentString.Contains("\""))
                    {
                        if (str)
                        {
                            currentString = currentString.Substring(0, currentString.IndexOf("\"", StringComparison.Ordinal) + 2);
                            str = false;
                        }
                        else
                        {
                            currentString = currentString.Substring(currentString.IndexOf("\"", StringComparison.Ordinal) + 2);
                            str = true;
                        }
                    }

                    currentString = currentString.Trim();

                    if (multilineComment)
                    {
                        if (currentString.Contains("*/"))
                        {
                            currentString = currentString.Substring(currentString.IndexOf("*/", StringComparison.Ordinal) + 2);
                            multilineComment = false;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    while (currentString.Contains("/*"))
                    {
                        if (currentString.Contains("*/"))
                        {
                            currentString = currentString.Remove(currentString.IndexOf("/*", StringComparison.Ordinal),
                                currentString.IndexOf("*/", StringComparison.Ordinal) + 2);
                        }
                        else
                        {
                            multilineComment = true;
                            break;
                        }
                    }

                    if (multilineComment)
                    {
                        currentString = currentString.Substring(0, currentString.IndexOf("/*", StringComparison.Ordinal));
                    }
                    currentString = currentString.Trim();

                    if (!currentString.Equals(string.Empty))
                    {
                        LinesCount++;
                    }
                }
            }
        }
    }
}
