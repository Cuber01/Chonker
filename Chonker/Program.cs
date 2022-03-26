using Chonker.Leaves;
using Chonker.Parsing;
using Chonker.Scanning;
using Chonker.Tokens;
using Chonker.Tools;

namespace Chonker 
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Scanner scanner = new Scanner(FileReader.getFileString(args[0]));
            List<Token> tokens = scanner.scanTokens();

            if (scanner.hadError)
            {
                Environment.Exit(1);
            }
            
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();

            if (parser.hadError)
            {
                Environment.Exit(1);
            }

            // foreach (var stmt in statements)
            // {
            //     Console.WriteLine(new AstPrinter().print(stmt));    
            // }

            Interpreter.Interpreter interpreter = new Interpreter.Interpreter();
            interpreter.interpret(statements);

            if (interpreter.hadError)
            {
                Environment.Exit(1);
            }

            Environment.Exit(0);

        }
    }
}