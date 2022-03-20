namespace Chonker.Expressions;

public abstract class Stmt
{
    public abstract TResult accept<TResult>(IVisitor<TResult> visitor);

    public interface IVisitor<TResult>
    {
        TResult visitPrintStmt(PrintStmt stmt);
        TResult visitExpressionStmt(ExpressionStmt stmt);

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