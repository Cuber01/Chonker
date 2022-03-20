using Chonker.Expressions;
using Chonker.Tokens;
using static Chonker.Tokens.TokenType;

namespace Chonker.Interpreter;

/*

This is the interpreter. An interpreter gets expressions and executes them.
 
We basically visit and evaluate every expression into values/literals, and then use the values with the given operand. 
  
*/


public class Interpreter : Expr.IVisitor<Object>, Stmt.IVisitor<Object?>
{
    private Environments.Environment environment = new Environments.Environment();
    public bool hadError = false;
    
    public void interpret(List<Stmt> statements)
    { 
        try 
        {
            foreach (var stmt in statements)
            {
                execute(stmt);
            }
        } catch (InterpreterError error)
        {
            Console.WriteLine(error.Message);
        }
    }
    
    private Object evaluate(Expr expr)
    {
        return expr.accept(this);
    }

    #region Statements
    
    private void execute(Stmt stmt) {
        stmt.accept(this);
    }

    public object? visitPrintStmt(PrintStmt stmt)
    {
        object result = evaluate(stmt.expression);
        Console.WriteLine(stringify(result));
        return null;
    }

    public object? visitExpressionStmt(ExpressionStmt stmt)
    {
        evaluate(stmt.expression);
        return null;
    }

    public object? visitVariableStmt(VariableStmt stmt)
    {
        object? value = null;
        
        if (stmt.initializer is not null)
        {
            value = evaluate(stmt.initializer);
        }

        environment.define(stmt.name.lexeme, value);
        return null;
    }

    #endregion

    #region Expressions
    
    public object visitBinaryExpr(BinaryExpr expr)
    {
        object right = evaluate(expr.right);
        object left = evaluate(expr.left);

        switch (expr.operant.type) 
        {
            
            case MINUS:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left - (double)right; 
            case SLASH: 
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);

                if (right.Equals(0.0))
                {
                    error(expr.operant, "Attempt to divide by zero");
                }
                
                return (double)left / (double)right;
            case STAR: 
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left * (double)right;
            case PLUS:
            {
                if (left is Double && right is Double) 
                {
                    return (double)left + (double)right;
                } 

                if (left is String && right is String) 
                {
                    return (String)left + (String)right;
                }

                error(expr.operant, "Operands have to be either both numbers or strings.");
                break;
            }
            
            case GREATER:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left > (double)right;
            case GREATER_EQUAL:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left >= (double)right;
            case LESS:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left < (double)right;
            case LESS_EQUAL:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers", expr.operant);
                return (double)left <= (double)right;
            
            case BANG_EQUAL: 
                return !isEqual(left, right);
            case EQUAL_EQUAL: 
                return isEqual(left, right);
            
            case COMMA:
                return right;
        }

        return null!;
    }

    public object visitGroupedExpr(GroupedExpr expr)
    {
        return evaluate(expr.expression);
    }

    public object visitLiteralExpr(LiteralExpr expr)
    {
        return expr.value!;
    }

    public object visitUnaryExpr(UnaryExpr expr)
    {
        Object right = evaluate(expr.target);

        switch (expr.operant.type) {
            case BANG:
                return !isTruthy(right);
            case MINUS:
                return -(double)right;
        }

        // Unreachable.
        return null!;
    }

    public object visitAssignExpr(AssignExpr expr)
    {
        object value = evaluate(expr);
        environment.assign(expr.name, value);
        return value;
    }

    public object visitVariableExpr(VariableExpr expr)
    {
        return environment.getValue(expr.name);
    }

    #endregion

    #region Util
    
    private bool isTruthy(object? obj)
    {
        if (obj is null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }

    private bool isEqual(object? left, object? right)
    {
        if (left is null && right is null)
        {
            return true;
        } 
        
        if (left is null)
        {
            // Right is definitely not null if we're here.
            return false;
        }

        return left.Equals(right);
    }

    private string stringify(object obj)
    {
        string text = obj.ToString()!;

        if (text.EndsWith(".0"))
        {
            text = text.Substring(0, text.Length - 2);
        }

        return text;
    }
    
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
        
        Console.WriteLine("Interpreting error:");
        Console.WriteLine(fancyMessage);
                
        throw new InterpreterError(message);
    }
    
    private void checkOperands(object left, object right, Type typeLeft, Type typeRight, string errorMessage, Token token)
    {
        if (left.GetType() == typeLeft && right.GetType() == typeRight)
        {
            return;
        }

        error(token, errorMessage);
    }

    #endregion
    
}