using Chonker.Expressions;
using Chonker.Tokens;
using static Chonker.Tokens.TokenType;

namespace Chonker.Interpreter;

public class Interpreter : Expr.IVisitor<Object>
{
    public bool hadError = false;
    
    public void interpret(Expr expression)
    { 
        try 
        {
            Object value = evaluate(expression);
            Console.WriteLine(stringify(value));
        } catch (InterpreterError error)
        {
            Console.WriteLine(error.Message);
        }
    }
    
    private Object evaluate(Expr expr)
    {
        return expr.accept(this);
    }

    #region Main
    
    public object visitBinaryExpr(BinaryExpr expr)
    {
        object right = evaluate(expr.right);
        object left = evaluate(expr.left);

        switch (expr.operant.type) 
        {
            
            case MINUS:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left - (double)right; 
            case SLASH: 
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left / (double)right;
            case STAR: 
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
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
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left > (double)right;
            case GREATER_EQUAL:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left >= (double)right;
            case LESS:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left < (double)right;
            case LESS_EQUAL:
                checkOperands(left, right, typeof(Double), typeof(Double), "Both operands should be numbers.", expr.operant);
                return (double)left <= (double)right;
            
            case BANG_EQUAL: 
                return !isEqual(left, right);
            case EQUAL_EQUAL: 
                return isEqual(left, right);
        }

        return null;
    }

    public object visitGroupedExpr(GroupedExpr expr)
    {
        return evaluate(expr.expression);
    }

    public object visitLiteralExpr(LiteralExpr expr)
    {
        return expr.value;
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
        return null;
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

        return left.Equals(right);
    }

    private string stringify(object obj)
    {
        string text = obj.ToString()!;

        if (text!.EndsWith(".0"))
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