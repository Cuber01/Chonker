using Chonker.Tokens;

namespace Chonker.Interpreter;

public class InterpreterError : Exception
{
    private string message;
    
    public InterpreterError(string message)
    {
        this.message = message;
    }

    public override string ToString()
    {
        return $"Runtime Error:\n {message}";
    }
}
