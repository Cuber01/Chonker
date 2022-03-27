using Chonker.Tokens;

namespace Chonker.Leaves;

public class Break : Exception
{
    public Break(Token keyword)
    {
        this.keyword = keyword;
    }

    public readonly Token keyword;
}