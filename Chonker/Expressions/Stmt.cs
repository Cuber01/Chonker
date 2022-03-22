using Chonker.Tokens;

namespace Chonker.Expressions;

public abstract class Stmt
{
    public abstract TResult accept<TResult>(IVisitor<TResult> visitor);

    public interface IVisitor<out TResult>
    {
        TResult visitPrintStmt(PrintStmt stmt);
        TResult visitExpressionStmt(ExpressionStmt stmt);
        TResult visitVariableStmt(VariableStmt stmt);
    }
}

public class PrintStmt : Stmt
{
    public PrintStmt(Expr expression)
    {
        this.expression = expression;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitPrintStmt(this);
    }

    public readonly Expr expression;
}

public class ExpressionStmt : Stmt
{
    public ExpressionStmt(Expr expression)
    {
        this.expression = expression;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitExpressionStmt(this);
    }

    public readonly Expr expression;
}

public class VariableStmt : Stmt
{
    public VariableStmt(Token name, Type? type, Expr initializer)
    {
        this.initializer = initializer;
        this.name = name;
        this.type = type;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitVariableStmt(this);
    }

    public readonly Token name;
    public readonly Type? type;
    
    public readonly Expr? initializer;
}