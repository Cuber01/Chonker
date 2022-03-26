namespace Chonker.Functions;

public class Return : Exception 
{
    public readonly object? value;
    public bool isEmpty;

    public Return(object? value, bool isEmpty)
    { 
        this.value = value;
        this.isEmpty = isEmpty;
    }
}

public class Empty
{
    // Class used to represent emptiness aka void.
    // If anything touches it, it will throw an error.
}
