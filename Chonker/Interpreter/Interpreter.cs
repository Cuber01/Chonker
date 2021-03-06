using Chonker.Environments;
using Chonker.Functions;
using Chonker.Leaves;
using Chonker.Tokens;
using static Chonker.Tokens.TokenType;

namespace Chonker.Interpreter;

/*

This is the interpreter. An interpreter gets statements/expressions and executes them.
 
We basically visit and evaluate every statement and expression into values/literals, and then use the values with the given operand/other stuff. 
  
*/


public class Interpreter : Expr.IVisitor<Object>, Stmt.IVisitor<Object?>
{
    public readonly Scope globals = new Scope(null);
    private Scope scope;
    
    public bool hadError;

    public Interpreter()
    {
        scope = globals;
        
        globals.define("clock",  -1, typeof(Func<>) , new NativeFunctions.Clock());
        globals.define("count",  -1, typeof(Func<>) , new NativeFunctions.Count());
        globals.define("sleep",  -1, typeof(Func<>) , new NativeFunctions.Sleep());
        globals.define("round",  -1, typeof(Func<>) , new NativeFunctions.Round());
        globals.define("remove",    -1, typeof(Func<>) , new NativeFunctions.Remove());
        globals.define("add", -1, typeof(Func<>) , new NativeFunctions.Add());
    }
    
    public void interpret(List<Stmt> statements)
    { 
        try 
        {
            foreach (var stmt in statements)
            {
                execute(stmt);
            }
        } catch (Error error)
        {
            error.writeMessage();
            hadError = true;
        }

    }

    public object evaluate(Expr expr)
    {
        return expr.accept(this);
    }
    
    #region Execution

    public void execute(Stmt stmt) 
    {
        stmt.accept(this);
    }

    public void executeBlock(List<Stmt> statements, Scope newEnv)
    {
        Scope previous = this.scope;
        
        try 
        {
            this.scope = newEnv;

            foreach (Stmt statement in statements)
            {
                execute(statement);
            }
            
        } finally
        {
            this.scope = previous;
        }
    }

    #endregion

    #region Statements

    public object? visitPrintStmt(PrintStmt stmt)
    {
        object result = evaluate(stmt.expression);

        if (stmt.writeline)
        {
            Console.WriteLine(stringify(result));
        }
        else
        {
            Console.Write(stringify(result));
        }
        
        return null;
    }
    
    public object? visitBlockStmt(BlockStmt stmt)
    {
        executeBlock(stmt.statements, new Scope(scope));
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

        if ( !(value is null || value.GetType() == stmt.type) )
        {
            error(stmt.name, $"Cannot convert type {stmt.type} to {value.GetType()}");

            return null;
        }

        scope.define(stmt.name, stmt.type, value);
        return null;
    }
    
    public object? visitIfStmt(IfStmt stmt)
    {
        if (isTruthy(evaluate(stmt.condition))) 
        {
            execute(stmt.thenBranch);
        } 
        else if (stmt.elseBranch != null)
        {
            execute(stmt.elseBranch);
        }
        
        return null;
    }

    public object? visitWhileStmt(WhileStmt stmt)
    {
        while (isTruthy(evaluate(stmt.condition)))
        {
            try
            {
                execute(stmt.body);    
            } catch (Break br)
            {
                return null;
            }
            
        }
        
        return null;
    }

    public object? visitFunctionStmt(FunctionStmt stmt)
    {
        Function function = new Function(stmt);
        scope.define(stmt.name.lexeme, stmt.name.line, function.GetType(), function);
        
        return null;
    }

    public object visitReturnStmt(ReturnStmt stmt)
    {
        object? value = null;
        if (stmt.value == null)
        {
            throw new Return(value, stmt.keyword.line, true);            
        }
        else
        {
            value = evaluate(stmt.value);
        }
        
        throw new Return(value, stmt.keyword.line, false);
    }

    public object visitBreakStmt(BreakStmt stmt)
    {
        throw new Break(stmt.keyword.line);
    }

    public object? visitSwitchStmt(SwitchStmt stmt)
    {
        bool foundCase = false;

        foreach (var pair in stmt.cases)
        {
            if (isTruthy(evaluate(pair.Key))) 
            {
                execute(pair.Value);
                foundCase = true;
            } 
        }

        if (stmt.defaultBranch is not null && !foundCase)
        {
            execute(stmt.defaultBranch);
        }

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

                error(expr.operant, "Operands have to be either both numbers or strings");
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
        object value = evaluate(expr.value);
        
        Type varType = scope.getType(expr.name)!;
        if ( !(value.GetType() == varType) )
        {
            error(expr.name, $"Cannot convert type {varType} to {value.GetType()}");

            return null!;
        }
        
        scope.assign(expr.name, value);
        return value;
    }

    public object visitVariableExpr(VariableExpr expr)
    {
        return scope.getValue(expr.name);
    }
    
    public object visitLogicalExpr(LogicalExpr expr)
    {
        object left = evaluate(expr.left);

        if (expr.operant.type == TokenType.OR)
        {
            if (isTruthy(left)) return left;
        } else 
        {
            if (!isTruthy(left)) return left;
        }

        return evaluate(expr.right);
    }
    
    public object visitCallExpr(CallExpr expr)
    {
        object callee = evaluate(expr.callee);
        
        // Check if what we call is callable/function
        if (callee is not Callable)
        {
            throw new Error("Interpreter", "Can only call functions and classes",expr.paren.lexeme, expr.paren.line);
        }

        // Evaluate arguments
        List<object> arguments = new List<object>();
        
        foreach (Expr argument in expr.arguments)
        { 
            arguments.Add(evaluate(argument));
        }

        Callable function = (Callable)callee;

        // Check if the number of arguments is right
        if (arguments.Count != function.arity())
        {
            throw new Error("Interpreter", "Expected " + function.arity() + " arguments but got " + arguments.Count, expr.paren.lexeme, expr.paren.line);
        }
        
        // Check if the type of arguments is right
        List<Type>? paramTypes = function.getParameterTypes();
        if (paramTypes != null)
        {
            for (var i = 0; i < arguments.Count; i++)
            {
                Type got = arguments[i].GetType();
                Type expected = paramTypes[i];
            
                if (got != expected && expected != typeof(object))
                {
                    throw new Error("Interpreter", "Expected argument of type " + expected + " but got " + got,
                        expr.paren.lexeme, expr.paren.line);
                }
            }    
        }

        return function.call(this, arguments)!;
    }

    public object visitTernaryExpr(TernaryExpr expr)
    {
        bool condition = isTruthy(evaluate(expr.condition));

        if (condition)
        {
            return evaluate(expr.thenBranch);
        }
        else
        {
            return evaluate(expr.elseBranch);
        }
    }

    public object visitSubscriptionExpr(SubscriptionExpr expr)
    {
        object result = evaluate(expr.list);

        if (result is not List<object?> list)
        {
            return result;
        }

        int index = (int)Convert.ToDouble(evaluate(expr.index));
        
        if (index >= 0 && index < list.Count)
        {
            if (list.ElementAt(index) is not Expr)
            {
                return list.ElementAt(index)!;
            }
            
            return evaluate((Expr)list.ElementAt(index)!);    
        }
        else
        {
            throw new Error("Parser", "Index " + index + " is outside of the bounds of the given list",
                expr.bracket.lexeme, expr.bracket.line);
        }
    }

    public object visitListAssignExpr(ListAssignExpr expr)
    {
        object result = evaluate(expr.list);

        if (result is not List<object?> list)
        {
            return result;
        }

        int index = (int)Convert.ToDouble(evaluate(expr.index));
        
        if (index >= 0 && index < list.Count)
        {
            list[index] = expr.value;
            return list.ElementAt(index)!;
        }
        else
        {
            throw new Error("Parser", "Index " + index + " is outside of the bounds of the given list",
                expr.equals.lexeme, expr.equals.line);
        }
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

    private string stringify(object? obj)
    {
        if (obj is null)
        {
            return "null";
        }

        if (obj is List<object?>)
        {
            return "list";
        }
        
        var text = obj.ToString()!;

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
        string where;
        
        if (token.type == TokenType.EOF)
        {
            where = "end";
        } 
        else 
        {
            where = $"{token.lexeme}";
        }
        
        throw new Error("Interpreter", message, where, token.line);
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