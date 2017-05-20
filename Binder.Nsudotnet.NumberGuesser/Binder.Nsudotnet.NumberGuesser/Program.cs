using System;
using System.Globalization;

namespace Binder.Nsudotnet.NumberGuesser
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine("Please enter your name");

            var name = Console.ReadLine();

            var random = new Random();

            var number = random.Next(0, 100);

            Console.WriteLine("Now try to guess the number");

            var time = DateTime.Now;
            var i = 0;
            var triesCount = 0;
            var history = new int[1000];

            while (true)
            {
                var s = Console.ReadLine();

                if (null == s)
                {
                    continue;
                }

                int tryNumber;

                if (int.TryParse(s, out tryNumber))
                {
                    history[triesCount] = tryNumber;
                    triesCount++;

                    if (tryNumber == number)
                    {
                        Console.WriteLine("Congratulations! You guessed it right!");

                        Console.WriteLine("History:");

                        for (var j = 0; j < triesCount - 1; j++)
                        {
                            Console.WriteLine("#{0}\t{1}\t{2}", j, history[j], (history[j] > number) ? ">" : "<");
                        }

                        Console.WriteLine("#{0}\t{1}\t{2}", triesCount, history[triesCount - 1], "=");

                        Console.WriteLine("Minutes spent: {0}", (DateTime.Now.Subtract(time)).TotalMinutes.ToString(CultureInfo.CurrentCulture));

                        break;
                    }
                    
                    Console.WriteLine(tryNumber > number ? "Lower!" : "MOAR!");

                    i++;

                    if (i % 4 == 0)
                    {
                        Console.WriteLine(Abuse(number, tryNumber, name));
                    }
                }
                else if (s.Equals("q"))
                {
                    Console.WriteLine("Giving up already? Okey, see you, take care, bye!");

                    break;
                }
                else
                {
                    Console.WriteLine("Less talking, more guessing! Again!");
                }
            }

            Console.ReadKey();
        }

        private static string Abuse(int number, int tryNumber, string name)
        {
            var random = new Random();

            string[] abuses = { string.Format("{0}, you are a whole new level of stupid...", name), 
                                  string.Format("A quantum supercomputer calculating for a thousand years could not even approach a number of tries you need, {0}...", name),
                                  string.Format("How come does it take so long, {0}? Are you stupid?!", name),
                                  string.Format("Oh, of course, {0}, silly me! And here I was expecting something that makes sence...", tryNumber),
                                  string.Format("{0}? Really? Oh, let me give you a nice piece of advice..\n{1}", tryNumber, (tryNumber < number) ? ("MOOOOAAARRR!!!") : ("LOOOOWWWWEEERRR!!!"))
                              };

            return abuses[random.Next(0, abuses.Length)];
        }
    }
}
