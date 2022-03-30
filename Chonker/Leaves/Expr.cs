using Chonker.Tokens;

namespace Chonker.Leaves;

/*
 
Expression is a chunk of tokens ready for executing. 

Literal expression is just a value. The rest of expressions is made out of literal expressions and operands.

We also use the visitor pattern here to access expression fields easily.

To see how expressions are executed, go to Interpreter/Interpreter.cs 
  
*/

public abstract class Expr
{
    public abstract TResult accept<TResult>(IVisitor<TResult> visitor);

    public interface IVisitor<out TResult>
    {
        TResult visitBinaryExpr(BinaryExpr expr);
        TResult visitGroupedExpr(GroupedExpr expr);
        TResult visitLiteralExpr(LiteralExpr expr);
        TResult visitUnaryExpr(UnaryExpr expr);
        TResult visitAssignExpr(AssignExpr expr);
        TResult visitVariableExpr(VariableExpr expr);
        TResult visitLogicalExpr(LogicalExpr expr);
        TResult visitCallExpr(CallExpr expr);
        TResult visitTernaryExpr(TernaryExpr expr);
        TResult visitSubscriptionExpr(SubscriptionExpr expr);
    }
}


public class BinaryExpr : Expr
{
    public BinaryExpr(Expr left, Token operant, Expr right)
    {
        this.left = left;
        this.operant = operant;
        this.right = right;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitBinaryExpr(this);
    }
    
    public readonly Expr left;
    public readonly Token operant;
    public readonly Expr right;
    
}

public class CallExpr : Expr
{
    public CallExpr(Expr callee, Token paren, List<Expr> arguments)
    {
        this.callee = callee;
        this.paren = paren;
        this.arguments = arguments;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitCallExpr(this);
    }
    
    public readonly Expr callee;
    public readonly Token paren;
    public readonly List<Expr> arguments;
    
}

public class GroupedExpr : Expr
{
    public GroupedExpr(Expr expression)
    {
        this.expression = expression;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitGroupedExpr(this);
    }
    
    public readonly Expr expression;
}

public class LogicalExpr : Expr
{
    public LogicalExpr(Expr left, Token operant, Expr right)
    {
        this.left = left;
        this.operant = operant;
        this.right = right;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitLogicalExpr(this);
    }
    
    public readonly Expr left;
    public readonly Token operant;
    public readonly Expr right;
}

public class TernaryExpr : Expr
{
    public TernaryExpr(Expr condition, Expr thenBranch, Expr elseBranch)
    {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitTernaryExpr(this);
    }

    public readonly Expr condition;
    public readonly Expr thenBranch;
    public readonly Expr elseBranch;
}

public class UnaryExpr : Expr
{
    public UnaryExpr(Token operant, Expr target)
    {
        this.target = target;
        this.operant = operant;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitUnaryExpr(this);
    }
    
    public readonly Expr target;
    public readonly Token operant;
}

public class VariableExpr : Expr
{
    public VariableExpr(Token name)
    {
        this.name = name;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitVariableExpr(this);
    }

    public readonly Token name;
}

public class AssignExpr : Expr
{
    public AssignExpr(Token name, Expr value)
    {
        this.name = name;
        this.value = value;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitAssignExpr(this);
    }


    public readonly Token name;
    public readonly Expr value;
}

public class SubscriptionExpr : Expr
{
    public SubscriptionExpr(Expr list, Expr index, Token bracket)
    {
        this.list = list;
        this.index = index;
        this.bracket = bracket;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitSubscriptionExpr(this);
    }

    public readonly Expr list;
    public readonly Expr index;
    public readonly Token bracket;
}

public class LiteralExpr : Expr
{
    public LiteralExpr(object? value)
    {
        this.value = value;
    }
    
    public override TResult accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.visitLiteralExpr(this);
    }
    
    public readonly object? value;
}
