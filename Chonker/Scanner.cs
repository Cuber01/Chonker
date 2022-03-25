using Chonker.Tokens;
using static Chonker.Tokens.TokenType;

/*

This is the scanner, the first step in the journey of our language implementation.
It's essentially just a script that transcribes text into Tokens, algorithmically it's not much, so let's just talk about what a token is.

See Tokens/Token.cs
 
*/

namespace Chonker
{
    public class Scanner
    {
        public bool hadError = false;
        
        private readonly string source;
        public readonly List<Token> tokens = new List<Token>();
        
        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "class",       CLASS       },
            { "else",        ELSE        },
            { "false",       FALSE       },
            { "for",         FOR         },
            { "function",    FUNCTION    },
            { "if",          IF          },
            { "null",        NULL        },
            { "print",       PRINT       },
            { "return",      RETURN      },
            { "super",       SUPER       },
            { "this",        THIS        },
            { "true",        TRUE        },
            { "var",         VAR         },
            { "while",       WHILE       },
            { "break",       BREAK       },
            { "string",      STRING_KW   },
            { "number",      NUMBER_KW   },
            { "bool",        BOOL_KW     },
            { "switch",      SWITCH      },
            { "case",        CASE        },
            { "default",     DEFAULT     },
        };

        private int start;
        private int current;
        private int line = 1;

        public Scanner(string source) 
        {
            this.source = source;
        }
        
        public List<Token> scanTokens() 
        {
            
            while (!isAtEnd()) 
            {
                // We are at the beginning of the next lexeme.
                start = current;
                
                try
                {
                    scanToken();
                }
                catch (Error error)
                {
                    error.writeMessage();
                    hadError = true;
                }

            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }
        
        
        private void scanToken() 
        {
            char c = advance();
            switch (c) {
                case '(': addToken(LEFT_PAREN); break;
                case ')': addToken(RIGHT_PAREN); break;
                case '{': addToken(LEFT_BRACE); break;
                case '}': addToken(RIGHT_BRACE); break;
                case ',': addToken(COMMA); break;
                case ':': addToken(COLON); break;
                case '.': addToken(DOT); break;
                case '-': addToken(MINUS); break;
                case '+': addToken(PLUS); break;
                case ';': addToken(SEMICOLON); break;
                case '*': addToken(STAR); break;
                
                case '!':
                    addToken(match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    addToken(match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    addToken(match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '&':
                    if (match('&'))
                    {
                        addToken(AND);
                    }
                    else
                    {
                        throw new Error("Scanner", "Another '&' expected after single '&'", "", line);
                    }
                    break;
                
                case '|':
                    if (match('|'))
                    {
                        addToken(OR);
                    }
                    else
                    {
                        throw new Error("Scanner", "Another '|' expected after single '|'", "", line);
                    }
                    break;

                
                case '"': handleString(); break;
                
                case ' ' : break;
                case '\r': break;
                case '\t': break;
                case '\n':
                    line++;
                    break;
                
                case '/':
                    if (match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd())
                        {
                            advance();
                        }
                    } else if (match('*'))
                    {
                        while (!isAtEnd())
                        {
                            // If we don't find a '*', we can continue safely
                            if (advance() != '*') continue;
                            
                            // If we find */, stop the comment
                            if (match('/'))
                            {
                                break;
                            }

                        }
                    } else {
                        addToken(SLASH);
                    }
                    break;
                

                default:
                    if (isDigit(c)) 
                    {
                        handleNumber();
                    } else if (isAlpha(c)) 
                    {
                        handleIdentifier();
                    } else
                    {
                        throw new Error("Scanner", "Unexpected character '" + current + "'", "", line); 
                    }
                    break;
            }
        }
        
        private char advance() 
        {
            return source[current++];
        }
        
        private char peek() 
        {
            
            if (isAtEnd())
            {
                return '\0';
            }
            
            return source[current];
        }
        
        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private void addToken(TokenType type, Object? literal = null) 
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal!, line));
        }
        
        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            
            if (source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }
        
        private void handleNumber()
        {

            while (isDigit(peek()))
            {
                advance();
            }
            
            if (peek() == '.' && isDigit(peekNext()))
            {

                // Consume the "."
                advance();

                while (isDigit(peek()))
                {
                    advance();
                }
            }
            
            addToken(NUMBER, Convert.ToDouble(source.Substring(start, current - start)));
        }
        
        private void handleString()
        {
            while (peek() != '"' && !isAtEnd()) {
                
                if (peek() == '\n')
                {
                    line++;
                }
                
                advance();
            }

            if (isAtEnd()) 
            {
                throw new Error("Scanner","Unterminated string", "at " + current, line);
            }

            // The closing "
            advance();

            // Trim the surrounding quotes.
            string value = source.Substring(start + 1, (current - start) - 2);
            addToken(STRING, value);
        }
        
        private void handleIdentifier() 
        {

            while (isAlphaNumeric(peek()))
            {
                advance();
            }
            
            string text = source.Substring(start, current - start);

            if (!keywords.TryGetValue(text, out var type))
            { 
                // TODO
               addToken(IDENTIFIER);
               return;
            }

            addToken(type);
        }
        
        private bool isDigit(char c) 
        {
            return c is >= '0' and <= '9';
        } 
        
        private bool isAlpha(char c) 
        {
            
            if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_')
            {
                return true;
            } else 
            {
                return false;
            }
            
        }

        private bool isAlphaNumeric(char c) 
        {
            return isAlpha(c) || isDigit(c);
        }

        private bool isAtEnd() 
        {
            return current >= source.Length;
        }
        
    }
}