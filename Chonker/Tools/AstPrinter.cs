using System.Text;
using Chonker.Leaves;

namespace Chonker.Tools;

public class AstPrinter : Expr.IVisitor<String?>, Stmt.IVisitor<String?>
{
    private const string exprTypeColor = "\u001b[0;32m";
    private const string parenthesisColor = "\u001b[0;34m";
    private const string resetColor = "\u001b[0m";
    
    public string print(Stmt stmt)
    {
       return stmt.accept(this)!;
    }

    #region Statements

    public string visitPrintStmt(PrintStmt stmt)
    {
        return build("print", stmt.expression);
    }

    public string? visitExpressionStmt(ExpressionStmt stmt)
    {
        return stmt.expression.accept(this);
    }

    public string visitVariableStmt(VariableStmt stmt)
    {
        return build("var " + stmt.name.lexeme, stmt.initializer!);
    }

    public string visitBlockStmt(BlockStmt stmt)
    {
        foreach (var statement in stmt.statements)
        {
            print(statement);
        }

        return "";
    }

    public string visitIfStmt(IfStmt stmt)
    {
        return build("if", stmt.condition);
    }

    public string visitWhileStmt(WhileStmt stmt)
    {
        return build("while", stmt.condition);
    }

    public string? visitFunctionStmt(FunctionStmt stmt)
    {
        return build("function", new LiteralExpr(stmt.name));
    }

    public string? visitReturnStmt(ReturnStmt stmt)
    {
        return build("return", stmt.value);
    }

    public string? visitBreakStmt(BreakStmt stmt)
    {
        return build("break");
    }

    #endregion

    #region Expressions

    public string visitBinaryExpr(BinaryExpr expr)
    {
        return build(expr.operant.lexeme, expr.left, expr.right);
    }

    public string visitGroupedExpr(GroupedExpr expr)
    {
        return build("group", expr.expression);
    }

    public string? visitLiteralExpr(LiteralExpr expr)
    {
        if (expr.value is null)
        {
            return "null";
        }
        
        return expr.value.ToString();
    }

    public string visitUnaryExpr(UnaryExpr expr)
    {
        return build(expr.operant.lexeme, expr.target);
    }

    public string visitAssignExpr(AssignExpr expr)
    {
        return build(expr.name.lexeme, expr.value); 
    }

    public string visitVariableExpr(VariableExpr expr)
    {
        return build(expr.name.lexeme); 
    }

    public string? visitLogicalExpr(LogicalExpr expr)
    {
        return build(expr.operant.lexeme, expr.left, expr.right);
    }

    public string visitCallExpr(CallExpr expr)
    {
        return build("call", expr.callee);
    }

    public string? visitTernaryExpr(TernaryExpr expr)
    {
        return build("?", expr.condition, expr.thenBranch, expr.thenBranch);
    }

    #endregion
    
    
    private string build(string type, params Expr?[] expressions)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(parenthesisColor);
        builder.Append('(');
        builder.Append(resetColor);
        
        builder.Append(exprTypeColor);
        builder.Append(type);
        builder.Append(resetColor);
        
        foreach (Expr expr in expressions) 
        {
            builder.Append(' ');
            builder.Append(expr.accept(this));
        }
        
        builder.Append(parenthesisColor);
        builder.Append(')');
        builder.Append(resetColor);
        
        return builder.ToString();
    }
}