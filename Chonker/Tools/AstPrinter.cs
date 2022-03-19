using Chonker.Expressions;

namespace Chonker.Tools;

public class AstPrinter : Expr.IVisitor<String>
{
    public string visitBinaryExpr(BinaryExpr expr)
    {
        throw new NotImplementedException();
    }

    public string visitGroupedExpr(GroupedExpr expr)
    {
        throw new NotImplementedException();
    }

    public string visitLiteralExpr(LiteralExpr expr)
    {
        throw new NotImplementedException();
    }

    public string visitUnaryExpr(UnaryExpr expr)
    {
        throw new NotImplementedException();
    }
}