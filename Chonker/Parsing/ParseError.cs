namespace Chonker.Parsing;

public class ParseError : Exception
{
    private string message;
    
    public ParseError(string message)
    {
        this.message = message;
    }

    public override string ToString()
    {
        return $"Parse Error:\n {message}";
    }
}