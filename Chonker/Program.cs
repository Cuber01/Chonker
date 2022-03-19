using Chonker.Expressions;
using Chonker.Parsing;
using Chonker.Tokens;
using Chonker.Tools;
using FakeOS.Tools;

namespace Chonker 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner(FileReader.getFileString("./Tests/test.txt"));
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            Expr? expression = parser.parse();

            if (expression is null)
            {
                return;
            }

            Console.WriteLine(new AstPrinter().print(expression));
        }
    }
}