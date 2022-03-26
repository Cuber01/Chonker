namespace Chonker.Functions;

public class Return : Exception 
{
    public readonly object? value;
    public readonly bool isEmpty;
    public readonly int line;

    public Return(object? value, int line, bool isEmpty)
    { 
        this.value = value;
        this.isEmpty = isEmpty;
        this.line = line;
    }
}

public class Empty
{
    // Class used to represent emptiness aka void.

    public override string ToString()
    {
        return "void";
    }
}
