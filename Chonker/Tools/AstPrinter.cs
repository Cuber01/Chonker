using System.Text;
using Chonker.Expressions;

namespace Chonker.Tools;

public class AstPrinter : Expr.IVisitor<String>
{
    private const string exprTypeColor = "\u001b[0;32m";
    private const string parenthesisColor = "\u001b[0;34m";
    private const string resetColor = "\u001b[0m";
    
    public string print(Expr expr)
    {
        return expr.accept(this);
    }
    
    public string visitBinaryExpr(BinaryExpr expr)
    {
        return build(expr.operant.lexeme, expr.left, expr.right);
    }

    public string visitGroupedExpr(GroupedExpr expr)
    {
        return build("group", expr.expression);
    }

    public string visitLiteralExpr(LiteralExpr expr)
    {
        return expr.value.ToString();
    }

    public string visitUnaryExpr(UnaryExpr expr)
    {
        return build(expr.operant.lexeme, expr.target);
    }
    
    private string build(string type, params Expr[] expressions)
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