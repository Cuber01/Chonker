using Chonker.Expressions;
using Chonker.Tokens;

using static Chonker.Tokens.TokenType;

namespace Chonker.Parsing;

public class Parser
{
    private readonly List<Token> tokens;
    private int currentIndex;

    public bool hadError = false;
    
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;

        expression();
    }
    
    public Expr? parse() 
    {
        try {
            return expression();
        } catch (ParseError) {
            return null;
        }
    }


    #region Main

    private Expr expression() 
    {
        return equality();
    }
    
    private Expr equality()
    {
        Expr expr = comparison();

        while (isMatchConsume(EQUAL_EQUAL, BANG_EQUAL))
        {
            Token operant = previousToken();
            Expr right = comparison();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr comparison()
    {
        Expr expr = additionSubtraction();
            
        while (isMatchConsume(LESS, GREATER, GREATER_EQUAL, LESS_EQUAL))
        {
            Token operant = previousToken();
            Expr right = additionSubtraction();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr additionSubtraction()
    {
        Expr expr = multiplicationDivision();
            
        while (isMatchConsume(MINUS, PLUS))
        {
            Token operant = previousToken();
            Expr right = multiplicationDivision();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr multiplicationDivision()
    {
        Expr expr = unary();
            
        while (isMatchConsume(STAR, SLASH))
        {
            Token operant = previousToken();
            Expr right = unary();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr unary()
    {
        if (isMatchConsume(BANG, MINUS))
        {
            Token operant = previousToken();
            Expr right = primary();
            return new UnaryExpr(operant, right);
        }
        
        return primary();
    }
    
    private Expr primary()
    {
        switch (currentToken().type)
        {
            case TRUE:   return new LiteralExpr(true);  
            case FALSE:  return new LiteralExpr(false); 
            case NULL:   return new LiteralExpr(null);  
            case NUMBER: return new LiteralExpr(currentToken().literal); 
            case STRING: return new LiteralExpr(currentToken().literal); 

            case LEFT_PAREN:
            {
                consumeError(RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupedExpr(expression());
            }
        }

        error(currentToken(), "Expect expression.");

        return null!;
    }
    
    #endregion

    #region Util

    private bool isMatchConsume(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (check(type))
            {
                advance();
                return true;
            }
        }

        return false;
    }
    
    private bool check(TokenType type) {
        if (isAtEnd()) return false;
        return peek().type == type;
    }
    
    private Token peek()
    {
        return tokens.ElementAt(currentIndex);
    }

    private void advance()
    {
        if(isAtEnd()) return;
        currentIndex++;
    }

    private void consumeError(TokenType type, string errorMessage)
    {
        if (currentToken().type == type)
        {
            advance();
            return;
        }

        error(currentToken(), errorMessage);
    }
    

    private bool isAtEnd() => currentIndex == tokens.Count - 1;

    private Token currentToken() => tokens.ElementAt(currentIndex);

    private Token previousToken() => tokens.ElementAt(currentIndex - 1);

    #endregion

    #region Error

    private void error(Token token, string message)
    {
        hadError = true;
        
        string fancyMessage;

        if (token.type == TokenType.EOF)
        {
            fancyMessage = $"[{token.line}] {message} at end.";
        } else 
        {
            fancyMessage = $"[{token.line}] {message} at {token.lexeme}.";
        }

        Console.WriteLine("Parsing error:");
        Console.WriteLine(fancyMessage);
        
        throw new ParseError(fancyMessage);
        
    }
    
    private void synchronize() 
    {
        advance();
    
        while (!isAtEnd()) {
            if (previousToken().type == SEMICOLON) return;
    
            switch (currentToken().type) 
            {
                case CLASS:
                case FUNCTION:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }
    
            advance();
        }
    }


    #endregion
}