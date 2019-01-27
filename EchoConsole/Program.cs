using System;
using System.Linq;

namespace EchoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool runSmoke = args.Contains("smoke");

            if (runSmoke)
            {
                Smoke smoke = new Smoke();
                smoke.Execute();
                return;
            }

            if (args.Any())
            {
                Console.WriteLine($"Given argument {args.First()}");
            }
        }
    }
}
