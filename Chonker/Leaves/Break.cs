using Chonker.Tokens;

namespace Chonker.Leaves;

public class Break : Exception
{
    public Break(int line)
    {
        this.line = line;
    }

    public readonly int line;
}