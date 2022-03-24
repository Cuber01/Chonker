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
    private List<Stmt> statements = new List<Stmt>();
    private readonly List<Token> tokens;
    private int currentIndex;

    public bool hadError;
    
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }
    
    public List<Stmt> parse()
    {
        while (!isAtEnd()) 
        {
            statements.Add(declaration());
        }

        return statements; 
    }

    #region Statements

    private Stmt declaration()
    {
        try 
        {
            if (isMatchConsume(VAR))
            {
                return varDeclaration();
            }

            return statement();
        } 
        catch (Error error)
        {
            error.writeMessage();
            synchronize();
            
            return null!;
        }
    }
    
    private Stmt varDeclaration() 
    {
        Token type = consumeMultipleError("Expect variable type", STRING_KW, NUMBER_KW, BOOL_KW);
        Token name = consumeError(IDENTIFIER, "Expect variable name");

        Expr initializer = null!;
        if (isMatchConsume(EQUAL)) 
        {
            initializer = expression();
        }

        if (isMatchConsume(COMMA))
        {
            statements.Add(varDeclaration()); // WARNING: ADD STATEMENT IN LOOP
            return new VariableStmt(name, tokenToType(type), initializer);
        }

        consumeError(SEMICOLON, "Expect ';' after variable declaration");
        return new VariableStmt(name, tokenToType(type), initializer);
    }
    
    private Stmt statement()
    {
        if (isMatchConsume(PRINT)) return print();
        if (isMatchConsume(LEFT_BRACE)) return new BlockStmt(block());

        return expressionStatement();
    }
    
    private Stmt expressionStatement()
    {
        Expr expr = expression();
        consumeError(SEMICOLON, "Expect ';' after expression");
        return new ExpressionStmt(expr);
    }

    private List<Stmt> block()
    {
        List<Stmt> enclosedStmts = new List<Stmt>();

        while (currentToken().type != RIGHT_BRACE && !isAtEnd())
        {
            enclosedStmts.Add(declaration());
        }

        consumeError(RIGHT_BRACE, "Expect '}' after block");
        return enclosedStmts;
    }
    
    private Stmt print() 
    {
        Expr value = expression();
        consumeError(SEMICOLON, "Expect ';' after value");
        return new PrintStmt(value);
    }
    
    #endregion
    
    #region Expressions

    private Expr expression() 
    {
        return assignment();
    }

    private Expr assignment()
    {
        Expr expr = equality();

        if (isMatchConsume(EQUAL))
        {
            Token equals = previousToken();
            Expr value = assignment();

            if (expr is VariableExpr variableExpr)
            { 
                Token name = variableExpr.name;
                return new AssignExpr(name, value);
            }

            error(equals, "Invalid assignment target."); 
        }

        return expr;
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
            case IDENTIFIER: return new VariableExpr(previousToken());

            case LEFT_PAREN:
            {
                GroupedExpr expr = new GroupedExpr(expression());
                consumeError(RIGHT_PAREN, "Expect ')' after expression.");
                return expr;
            }
        }
        

        // Retract from advancing.
        retract();
        
        error(currentToken(), "Expect expression");

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

    private Token consumeError(TokenType type, string errorMessage)
    {
        if (currentToken().type == type)
        {
            advance();
            return previousToken();
        }

        error(currentToken(), errorMessage);
        
        return null!;
    }

    private Token consumeMultipleError(string errorMessage, params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (currentToken().type == type)
            {
                advance();
                return previousToken();
            }
        }
        
        error(currentToken(), errorMessage);
        
        return null!;
    }
    

    private bool isAtEnd() => currentIndex == tokens.Count - 1;
    private bool isAtBeginning() => currentIndex == 0;

    private Token currentToken() => tokens.ElementAt(currentIndex);

    private Token previousToken() => tokens.ElementAt(currentIndex - 1);

    private Type? tokenToType(Token token)
    {
        return token.type switch
        {
            NULL => null,
            NUMBER_KW => typeof(Double),
            STRING_KW => typeof(String),
            BOOL_KW => typeof(Boolean),
            _ => throw new Error("Interpreter", "Unknown variable type " + token.type, token.lexeme, token.line)
        };
    }
    
    
    #endregion

    #region Error

    private void error(Token token, string message)
    {
        hadError = true;
        
        string where;
        
        if (token.type == TokenType.EOF)
        {
            where = "at end.";
        } else 
        {
            where = $"at {token.lexeme}.";
        }
        
        throw new Error("Interpreter", message, where, token.line);
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