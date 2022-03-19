using System;
using Chonker.Expressions;
using Chonker.Tokens;
using Chonker.Tools;
using FakeOS.Tools;

namespace Chonker // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Expr expression = new BinaryExpr(
                new UnaryExpr(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new LiteralExpr(new Token(TokenType.NUMBER, "123", null,1))),
                new Token(TokenType.STAR, "*", null, 1),
                new GroupedExpr(
                    new LiteralExpr(new Token(TokenType.NUMBER, "45.67", null,1))));

            Console.WriteLine(new AstPrinter().print(expression));
        }
    }
}