using Chonker.Expressions;
using Chonker.Parsing;
using Chonker.Tokens;

using FakeOS.Tools;

namespace Chonker 
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Scanner scanner = new Scanner(FileReader.getFileString("./Tests/test.txt"));
            List<Token> tokens = scanner.scanTokens();

            if (scanner.hadError)
            {
                return;
            }
            
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();

            if (parser.hadError)
            {
                return;
            }
            
            Interpreter.Interpreter interpreter = new Interpreter.Interpreter();
            interpreter.interpret(statements);

        }
    }
}