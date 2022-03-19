using System;
using Chonker.Tokens;
using FakeOS.Tools;

namespace Chonker // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner(FileReader.getFileString("./Tests/test.txt"));
            List<Token> tokens = scanner.scanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
                Console.WriteLine();
            }
        }
    }
}