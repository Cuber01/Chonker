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
        TResult visitBlockStmt(BlockStmt stmt);
        TResult visitIfStmt(IfStmt stmt);
        TResult visitWhileStmt(WhileStmt stmt);
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

public class BlockStmt : Stmt
{
    public BlockStmt(List<Stmt> statements)
    {
        this.statements = statements;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitBlockStmt(this);
    }

    public readonly List<Stmt> statements;
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

public class IfStmt : Stmt
{
    public IfStmt(Expr condition, Stmt thenBranch, Stmt? elseBranch)
    {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitIfStmt(this);
    }

    public readonly Expr condition;
    public readonly Stmt thenBranch;
    public readonly Stmt? elseBranch;
}

public class WhileStmt : Stmt
{
    public WhileStmt(Expr condition, Stmt body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitWhileStmt(this);
    }

    public readonly Expr condition;
    public readonly Stmt body;
}


public class VariableStmt : Stmt
{
    public VariableStmt(Token name, Type type, Expr initializer)
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
    public readonly Type type;
    
    public readonly Expr? initializer;
}