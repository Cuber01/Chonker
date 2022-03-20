using Chonker.Expressions;
using Chonker.Tokens;

using static Chonker.Tokens.TokenType;

namespace Chonker.Parsing;

/*
 
A parser transcribes tokens into expressions and statements.

This one recursively goes through all the rules and tries to apply the current token to them, uppon success
it advances to the next token.

Depending on the rule, the second token will usually have enough information to create an expression.


-- Example --

Input: 1 + 1

First token: Go through all rules and finally bump into primary, create literal expression and advance
Second token: Go through the rules and get into addition, create the right expression by calling the 'top' and return the final expression made of:
(first token made into an expression, current token, third token made into an expression on the fly)

To see what expressions are, go to Expressions/Expr.cs 

*/

public class Parser
{
    private readonly List<Token> tokens;
    private int currentIndex;

    public bool hadError = false;
    
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
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
            Expr right = comma();
            return new UnaryExpr(operant, right);
        }
        
        return comma();
    }

    private Expr comma()
    {
        Expr expr = primary();
            
        while (isMatchConsume(COMMA))
        {
            Token operant = previousToken();
            Expr right = primary();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr primary()
    {
        // This is basically a hack for not using matchConsume(). Should we find something, advance, nothing found? then retract from advancing.
        Token oldToken = currentToken();
        advance();
        
        switch (oldToken.type)
        {
            case TRUE:   return new LiteralExpr(true);  
            case FALSE:  return new LiteralExpr(false); 
            case NULL:   return new LiteralExpr(null);  
            case NUMBER: return new LiteralExpr(oldToken.literal); 
            case STRING: return new LiteralExpr(oldToken.literal); 

            case LEFT_PAREN:
            {
                GroupedExpr expr = new GroupedExpr(expression());
                consumeError(RIGHT_PAREN, "Expect ')' after expression.");
                return expr;
            }
        }
        
        

        // Retract from advancing.
        retract();
        
        error(currentToken(), "Expect expression.");

        return null!;
    }
    
    #endregion

    #region Util

    private bool isMatchConsume(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (tokens[currentIndex].type == type)
            {
                advance();
                return true;
            }
        }

        return false;
    }

    private void retract()
    {
        if(isAtBeginning()) return;
        currentIndex--;
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
    private bool isAtBeginning() => currentIndex == 0;

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