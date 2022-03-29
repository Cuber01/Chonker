
namespace Chonker.Tokens
{
    public enum TokenType 
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,
        COLON, QUESTION_MARK, LEFT_BRACKET, RIGHT_BRACKET,
        
        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        IDENTIFIER, STRING, NUMBER,

        // Keywords.
        AND, CLASS, ELSE, FALSE, FUNCTION, FOR, IF, NULL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE, BREAK,
        SWITCH, CASE, DEFAULT, STRING_KW, NUMBER_KW, BOOL_KW,
        VOID, PUTS,

        EOF
    }
}