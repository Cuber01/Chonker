namespace Chonker.Functions;

public class Return : Exception 
{
    public readonly object? value;
    public readonly bool isEmpty;

    public Return(object? value, bool isEmpty)
    { 
        this.value = value;
        this.isEmpty = isEmpty;
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
