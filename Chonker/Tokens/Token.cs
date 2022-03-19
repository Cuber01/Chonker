/*
 
Token is any 'singular' piece of syntax. It's a recognizable element while writing code.  

Examples of tokens are:
* keywords (private, int)
* special characters, or sequences of characters (*, ||)
* user defined keywords (variable, function names)
* literals ("hello", 1)
 
In Chonker, a token has:

1) type - what it represents (See TokenType.cs for a full list)
2) lexeme - a string containing how it looks in raw text
3) value - a value it evaluates to
4) line - line it appears in

Next, let's see the parser at Parsing/Parser  

*/

namespace Chonker.Tokens
{
    public class Token
    {
       
        public readonly TokenType type;
        public readonly object? literal;
        public readonly string lexeme;
        public readonly int line;

        internal Token(TokenType type, string lexeme, object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString() 
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}