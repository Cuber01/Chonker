using Chonker.Tokens;

namespace Chonker.Expressions;

/*
 
Expression is a chunk of tokens ready for executing. 

Literal expression is just a value. The rest of expressions is made out of literal expressions and operands.

We also use the visitor pattern here to access expression fields easily.

To see how expressions are executed, go to Interpreter/Interpreter.cs 
  
*/

public abstract class Expr
{
    public abstract TResult accept<TResult>(IVisitor<TResult> visitor);

    public interface IVisitor<TResult>
    {
        TResult visitBinaryExpr(BinaryExpr expr);
        TResult visitGroupedExpr(GroupedExpr expr);
        TResult visitLiteralExpr(LiteralExpr expr);
        TResult visitUnaryExpr(UnaryExpr expr);
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
    
    public Expr left;
    public Token operant;
    public Expr right;
    
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
    
    public Expr expression;
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
    
    public Expr target;
    public Token operant;
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
    
    public object? value;
}
