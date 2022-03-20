using Chonker.Tokens;

namespace Chonker.Expressions;

public abstract class Stmt
{
    public abstract TResult accept<TResult>(IVisitor<TResult> visitor);

    public interface IVisitor<TResult>
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

    public Expr expression;
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

    public Expr expression;
}

public class VariableStmt : Stmt
{
    public VariableStmt(Token name, Expr initializer)
    {
        this.initializer = initializer;
        this.name = name;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitVariableStmt(this);
    }

    public Token name;
    public Expr? initializer;
}