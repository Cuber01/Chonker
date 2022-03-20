
namespace Chonker;

public class Error : Exception
{
    private readonly string type;
    private readonly string message;
    private readonly string where;
    private readonly int line;
    
    public Error(string type, string message, string where, int line)
    {
        this.type = type;
        this.message = message;
        this.where = where;
        this.line = line;
    }

    public void writeMessage()
    {

        Console.Write(type + " error:\n");
        Console.Write($"[ {line} ] ");
        Console.Write(message);

        if (where.Length > 0)
        {
            Console.Write('.');
        }
        else
        {
            Console.Write(where + '.');
        }
        
        Console.Write("\n\n");
        
    }
}