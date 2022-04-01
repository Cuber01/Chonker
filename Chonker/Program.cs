using System.Net.NetworkInformation;
using Chonker.Functions;
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
            if (args.Length == 0)
            {
                runRepl(); 
            }
            
            if (args[0] == "--help")
            {
                help(); 
            }
            
            if (args[0] == "--source")
            {
                if (args.Length >= 2)
                {
                    runFromString(args[1], true);    
                }
                {
                    Console.WriteLine("Expect string after --source.");
                    Environment.Exit(1);
                }
            }
            
            string source = FileReader.getFileString(args[0]);
            runFromString(source, true); // Exits
            Environment.Exit(0);
        }

        private static void runRepl()
        {
            while (true)
            {
                Console.Write("> ");
                string? source = Console.ReadLine();
                
                if (String.IsNullOrWhiteSpace(source))
                {
                    break;
                }
                
                
                if (!source.EndsWith(';'))
                {
                    source += ';';
                }
                
                runFromStringRepl(source);
            }

        }

        private static void runFromStringRepl(string source)
        {
            Scanner scanner = new Scanner(source);
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
            try
            {
                try 
                {
                    foreach (var stmt in statements)
                    {
                        if (stmt is ExpressionStmt exprStmt)
                        {
                            Console.WriteLine(interpreter.evaluate(exprStmt.expression));
                        }
                        
                        interpreter.execute(stmt);
                    }
                } catch (Error error)
                {
                    error.writeMessage();
                }

            }
            catch (Return ret)
            {
                Error e = new Error("Interpreter", "Unexpected return statement", "", ret.line);
                e.writeMessage();
                
                interpreter.hadError = true;
            }
            catch (Break br)
            {
                Error e = new Error("Interpreter", "Unexpected break statement", "", br.line);
                e.writeMessage();

                interpreter.hadError = true;
            }

        }
        
        private static void runFromString(string source, bool exitOnError)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            if (scanner.hadError)
            {
                if(exitOnError) Environment.Exit(1); else return;
            }
            
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();

            if (parser.hadError)
            {
                if(exitOnError) Environment.Exit(1); else return;
            }

            Interpreter.Interpreter interpreter = new Interpreter.Interpreter();
            try
            {
                interpreter.interpret(statements);
            }
            catch (Return ret)
            {
                Error e = new Error("Interpreter", "Unexpected return statement", "", ret.line);
                e.writeMessage();
                
                interpreter.hadError = true;
            }
            catch (Break br)
            {
                Error e = new Error("Interpreter", "Unexpected break statement", "", br.line);
                e.writeMessage();

                interpreter.hadError = true;
            }

            if (interpreter.hadError)
            {
                if(exitOnError) Environment.Exit(1);
            }
        }

        private static void help()
        {
            Console.WriteLine("Chonker Programming Language");
            Console.WriteLine("https://github.com/Cuber01/Chonker\n");
                
            Console.WriteLine("Optional Args:");
            Console.WriteLine("--help              - display this help prompt");
            Console.WriteLine("--source '[string]' - run given string as source");
            Console.WriteLine("[path to file]      - run source from file\n");
                
            Console.WriteLine("If there are no args provided, a REPL will launch.");
        }
    }
}