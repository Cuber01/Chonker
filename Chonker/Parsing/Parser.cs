using Chonker.Leaves;
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
    private readonly List<Stmt> statements = new List<Stmt>();
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
            if (isMatch(VAR))
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
        Token type = consumeMultipleError("Expect variable type", STRING_KW, NUMBER_KW, BOOL_KW, LIST_KW);
        Token name = consumeError(IDENTIFIER, "Expect variable name");

        // TODO if initializer is null impplement a default value depending on type
        Expr initializer = null!;
        if (isMatch(EQUAL)) 
        {
            initializer = expression();
        }
        else
        {
            initializer = new LiteralExpr(defaultInitializer(type.type));
        }

        if (isMatch(COMMA))
        {
            statements.Add(varDeclaration()); // WARNING: ADD STATEMENT IN LOOP
            return new VariableStmt(name, tokenToType(type)!, initializer);
        }

        consumeError(SEMICOLON, "Expect ';' after variable declaration");
        return new VariableStmt(name, tokenToType(type)!, initializer);
    }
    
    private Stmt statement()
    {
        if (isMatch(PRINT))      return print(false);
        if (isMatch(PUTS))       return print(true);
        if (isMatch(IF))         return ifStatement();
        if (isMatch(WHILE))      return whileStatement();
        if (isMatch(FOR))        return forStatement();
        if (isMatch(SWITCH))     return switchStatement();
        if (isMatch(FUNCTION))   return function("function");
        if (isMatch(RETURN))     return returnStatement();
        if (isMatch(BREAK))      return breakStatement();
        if (isMatch(LEFT_BRACE)) return new BlockStmt(block());
        

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
    
    private Stmt print(bool writeline) 
    {
        Expr value = expression();
        consumeError(SEMICOLON, "Expect ';' after value");
        return new PrintStmt(value, writeline);
    }

    private Stmt ifStatement()
    {
        consumeError(LEFT_PAREN, "Expect '(' after 'if'");
        Expr condition = expression();
        consumeError(RIGHT_PAREN, "Expect ')' after if condition"); 

        Stmt thenBranch = statement();
        
        Stmt? elseBranch = null;
        if (isMatch(ELSE))
        {
            elseBranch = statement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }
    
    private Stmt whileStatement() 
    {
        consumeError(LEFT_PAREN, "Expect '(' after 'while'.");
        Expr condition = expression();
        consumeError(RIGHT_PAREN, "Expect ')' after condition.");
        Stmt body = statement();

        return new WhileStmt(condition, body);
    }
    
    private Stmt forStatement() 
    {
        consumeError(LEFT_PAREN, "Expect '(' after 'for'");

        // Initializer
        Stmt? initializer;
        if (isMatch(SEMICOLON)) 
        {
            initializer = null;
        } 
        else if (isMatch(VAR))
        {
            initializer = varDeclaration();
        } 
        else
        {
            initializer = expressionStatement();
        }
        
        // Condition
        Expr? condition = null;
        if (currentToken().type != SEMICOLON)
        {
            condition = expression();
        }
        consumeError(SEMICOLON, "Expect ';' after loop condition");
        
        // Increment
        Expr? increment = null;
        if (currentToken().type != SEMICOLON)
        {
            increment = expression();
        }
        consumeError(RIGHT_PAREN, "Expect ')' after for clauses");
        
        // Body
        Stmt body = statement();
        
        // Assembling the pieces
        if (increment != null)
        {
            body = new BlockStmt(
                new List<Stmt>
                {
                    body,
                    // If there's an increment expression, we append it to the body so it gets executed after it
                    new ExpressionStmt(increment)
                });
        }

        // If there's no condition, we assume it as always true
        condition ??= new LiteralExpr(true);
        body = new WhileStmt(condition, body);
        
        
        if (initializer != null)
        {
            // If there's an initializer, we append it to the start of the body so it runs before it
            body = new BlockStmt( new List<Stmt> {initializer, body});
        }

        return body;
    }

    private Stmt switchStatement()
    {
        IfStmt? currentIf = null;
        Stmt? defaultBranch = null;
        List<IfStmt> cases = new List<IfStmt>();

        consumeError(LEFT_PAREN, "Expect '(' after 'switch'");
        Expr expr = expression();
        consumeError(RIGHT_PAREN, "Expect ')' after expression in switch");
        
        consumeError(LEFT_BRACE, "Expect '{' after switch()");

        while (currentToken().type != RIGHT_BRACE && !isAtEnd())
        {
            Token caseOrDefault = consumeMultipleError("Expect either 'case' or 'default' in switch body", CASE, DEFAULT);

            // Case
            if (caseOrDefault.type is CASE)
            {
                Expr condition = new BinaryExpr(expression(),
                    new Token(EQUAL_EQUAL, "==", null, -1),
                    expr);

                consumeError(COLON, "Expect ':' after case/default condition");

                Stmt caseBody = statement();
                cases.Add(new IfStmt(condition, caseBody, currentIf));
            }
            else // Default
            {
                // Check if there are 2 default statements
                if (defaultBranch != null)
                {
                    throw new Error("Parser", "One switch statement can't have more than one 'default' branch", 
                        caseOrDefault.lexeme, caseOrDefault.line);
                }
                
                consumeError(COLON, "Expect ':' after case/default condition");
                
                defaultBranch = statement();
            }
        }
        
        consumeError(RIGHT_BRACE, "Expect '}' after switch body");


        for (var i = 0; i < cases.Count; i++)
        {
            
            var _case = cases[i];
            if (i == 0 && defaultBranch != null)
            {
                IfStmt def = new IfStmt(new LiteralExpr(false), null!, defaultBranch);

                currentIf = new IfStmt(_case.condition, _case.thenBranch, def);
            }
            else
            {
                currentIf = _case;
            }
            
        }

        // TODO return literally nothing if null
        return currentIf!;
    }

    private FunctionStmt function(string kind)
    {
        Type returnType = tokenToType(consumeMultipleError("Expect return type", NUMBER_KW, STRING_KW, BOOL_KW, VOID))!;
        
        Token name = consumeError(IDENTIFIER, "Expect " + kind + " name");
        consumeError(LEFT_PAREN, "Expect '(' after " + kind + " name");
        
        List<(Token, Type)> parameters = new List<(Token, Type)>();
        if (currentToken().type != RIGHT_PAREN)
        {
            do
            {
                Type paramType = tokenToType(consumeMultipleError("Expect parameter type", BOOL_KW, STRING_KW, NUMBER_KW))!;
                Token paramName = consumeError(IDENTIFIER, "Expect parameter name");
                
                parameters.Add((paramName, paramType));
            } while (isMatch(COMMA));
        }
        
        consumeError(RIGHT_PAREN, "Expect ')' after parameters");
        
        consumeError(LEFT_BRACE, "Expect '{' before " + kind + " body");
        List<Stmt> body = block();

        if (returnType != typeof(void))
        {
            bool foundReturn = false;
            
            foreach (var stmt in body)
            {
                if (stmt is not ReturnStmt) continue;
                
                foundReturn = true;
                break;
            }

            if (!foundReturn)
            {
                throw new Error("Interpreter", "Expected return statement in " + kind + " of type " + returnType,
                    name.lexeme, name.line);
            }
        }

        return new FunctionStmt(name, returnType, parameters, body);
    }
    
    private Stmt returnStatement()
    {
        Token keyword = previousToken();
        Expr? value = null;
        
        if (currentToken().type != SEMICOLON)
        {
            value = expression();
        }

        consumeError(SEMICOLON, "Expect ';' after return value");
        return new ReturnStmt(keyword, value);
    }

    private Stmt breakStatement()
    {
        Token keyword = previousToken();
        consumeError(SEMICOLON, "Expect ';' after break statement");
        return new BreakStmt(keyword);
    }

    #endregion
    
    #region Expressions

    private Expr expression() 
    {
        return assignment();
    }

    private Expr assignment()
    {
        Expr expr = ternary();

        if (isMatch(EQUAL))
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

    private Expr ternary()
    {
        Expr expr = or();

        if (isMatch(QUESTION_MARK))
        {
            Expr thenBranch = or();
            consumeError(COLON, "Expect ':' after first branch of ternary operator");
            Expr elseBranch = or();
            expr = new TernaryExpr(expr, thenBranch, elseBranch);
        }

        return expr;
    }

    private Expr or()
    {
        Expr expr = and();

        while (isMatch(OR)) 
        {
            Token operant = previousToken();
            Expr right = and();
            expr = new LogicalExpr(expr, operant, right);
        }

        return expr;
    }
    
    private Expr and() 
    {
        Expr expr = equality();

        while (isMatch(AND))
        {
            Token operant = previousToken();
            Expr right = equality();
            expr = new LogicalExpr(expr, operant, right);
        }

        return expr;
    }
    
    private Expr equality()
    {
        Expr expr = comparison();

        while (isMatch(EQUAL_EQUAL, BANG_EQUAL))
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
            
        while (isMatch(LESS, GREATER, GREATER_EQUAL, LESS_EQUAL))
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
            
        while (isMatch(MINUS, PLUS))
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
            
        while (isMatch(STAR, SLASH))
        {
            Token operant = previousToken();
            Expr right = unary();
            expr = new BinaryExpr(expr, operant, right);
        }
        
        return expr;
    }
    
    private Expr unary()
    {
        if (isMatch(BANG, MINUS))
        {
            Token operant = previousToken();
            Expr right = primary();
            return new UnaryExpr(operant, right);
        }
        
        return subscription();
    }

    private Expr subscription()
    {
        Expr expr = call();

        if (isMatch(LEFT_BRACKET))
        {
            expr = finishSubscription(expr);
        }

        return expr;
    }

    private Expr finishSubscription(Expr expr)
    {
        Expr index = expression();
        Token bracket = consumeError(RIGHT_BRACKET, "Expect ] after list subscription.");

        if (isMatch(LEFT_BRACKET))
        {
            expr = finishSubscription(expr);
        }
        
        return new SubscriptionExpr(expr, index, bracket);
    }
    
    private Expr call() 
    {
        Expr expr = primary();

        while (true)
        { 
            if (isMatch(LEFT_PAREN))
            {
                expr = finishCall(expr);
            } else
            {
                break;
            }
        }

        return expr;
    }
    
    private Expr finishCall(Expr callee)
    {
        List<Expr> arguments = new List<Expr>();
        
        if (currentToken().type != RIGHT_PAREN)
        {
            do 
            {
                arguments.Add(expression());
            } while (isMatch(COMMA));
        }

        Token paren = consumeError(RIGHT_PAREN, "Expect ')' after arguments");

        return new CallExpr(callee, paren, arguments);
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

            case LEFT_BRACKET:
            {
                return list();
            }
        }
        
        // Retract from advancing.
        retract();
        
        error(currentToken(), "Expect expression");

        return null!;
    }

    private LiteralExpr list()
    {
        List<object?> value = new List<object?>();
        bool expectValue = true;
        
        while (!isMatch(RIGHT_BRACKET))
        {
            switch (currentToken().type)
            {
                case COMMA:
                {
                    if(expectValue) throw new Error("Parser", "Expect value in list declaration", currentToken().lexeme, currentToken().line);

                    advance();
                    
                    expectValue = true;
                    break;
                }

                default:
                    addToValue(expression());

                    expectValue = false;
                    break;
                    // throw new Error("Parser", "Tried to add a wrong token type to a list", currentToken().lexeme,
                    //     currentToken().line);
            }
            

        }
        
        void addToValue(Expr expr)
        {
            if(!expectValue) throw new Error("Parser", "Expect ',' after value in list declaration", currentToken().lexeme, currentToken().line);

            expectValue = false;
            value.Add(expr);
        }

        return new LiteralExpr(value);
    }
    
    #endregion

    #region Util

    private bool isMatch(params TokenType[] types)
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

    #region Variable Utils

    private Type? tokenToType(Token token)
    {
        return token.type switch
        {
            NULL => null,
            NUMBER_KW => typeof(Double),
            STRING_KW => typeof(String),
            BOOL_KW => typeof(Boolean),
            LIST_KW => typeof(List<object?>),
            VOID => typeof(void),
            _ => throw new Error("Interpreter", "Unknown variable type " + token.type, token.lexeme, token.line)
        };
    }

    private object defaultInitializer(TokenType type)
    {
        switch (type)
        {
            case NUMBER_KW: return 0.0;
            case STRING_KW: return "";
            case BOOL_KW:   return false;
            case LIST_KW:   return new List<object>();
            
            default: throw new Error("Internal", "Unknown type " + type, "", -1);
        }
    }
    
    #endregion

    #endregion

    #region Error

    private void error(Token token, string message)
    {
        hadError = true;
        
        string where;
        
        if (token.type == TokenType.EOF)
        {
            where = "end";
        } else 
        {
            where = $"{token.lexeme}";
        }
        
        throw new Error("Parser", message, where, token.line);
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
                case PUTS:
                case BREAK:
                case RETURN:
                    return;
            }
    
            advance();
        }
    }


    #endregion
}