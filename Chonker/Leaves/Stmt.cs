using System.Diagnostics;
using Chonker.Tokens;

namespace Chonker.Leaves;

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
        TResult visitFunctionStmt(FunctionStmt stmt);
        TResult visitReturnStmt(ReturnStmt stmt);
        TResult visitBreakStmt(BreakStmt stmt);
        TResult visitSwitchStmt(SwitchStmt stmt);
    }
}

public class PrintStmt : Stmt
{
    public PrintStmt(Expr expression, bool writeline)
    {
        this.expression = expression;
        this.writeline = writeline;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitPrintStmt(this);
    }

    public readonly Expr expression;
    public readonly bool writeline;
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

public class SwitchStmt : Stmt
{
    public SwitchStmt(Dictionary<Expr, Stmt> cases, Stmt? defaultBranch)
    {
        this.cases = cases;
        this.defaultBranch = defaultBranch;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitSwitchStmt(this);
    }

    public readonly Dictionary<Expr, Stmt> cases; // Condition, body
    public readonly Stmt? defaultBranch;
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

public class FunctionStmt : Stmt
{
    public FunctionStmt(Token name, Type returnType, List<(Token, Type)> parameters, List<Stmt> body)
    {
        this.name = name;
        this.parameters = parameters;
        this.body = body;
        this.returnType = returnType;
    }

    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitFunctionStmt(this);
    }

    public readonly Token name;
    public readonly List<(Token, Type)> parameters;
    public readonly Type returnType;
    public readonly List<Stmt> body;
}


public class ReturnStmt : Stmt
{
    public ReturnStmt(Token keyword, Expr? value)
    {
        this.keyword = keyword;
        this.value = value;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitReturnStmt(this);
    }
    
    public readonly Expr? value;
    public readonly Token keyword;
}

public class BreakStmt : Stmt
{
    public BreakStmt(Token keyword)
    {
        this.keyword = keyword;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitBreakStmt(this);
    }

    public readonly Token keyword;
}


