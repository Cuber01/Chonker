using Chonker.Interpreter;
using Chonker.Tokens;

namespace Chonker.Environments;

public class Scope
{
    private readonly Dictionary<string, object?> variables = new Dictionary<string, object?>();
    private readonly Scope? enclosing;

    public Scope(Scope? enclosing)
    {
        this.enclosing = enclosing;
    }
    
    public void define(Token name, object? value)
    {
        if (variables.Keys.Contains(name.lexeme))
        {
            throw new Error("Interpret","Tried to declare variable " + name.lexeme + ", but it already exists", "", name.line);
        }
        
        variables.Add(name.lexeme, value);
    }

    public object getValue(Token name)
    {
        if (variables.ContainsKey(name.lexeme))
        {
            return variables[name.lexeme]!;
        }
        
        if (enclosing != null) return enclosing.getValue(name);
        
        throw new Error("Interpreter", "Unknown variable '" + name + "'", $"at [{name.lexeme}]", name.line);
    }

    public Type getType(Token name)
    {
        if (variables.ContainsKey(name.lexeme))
        {
            return variables[name.lexeme]!.GetType();
        }

        if (enclosing != null) return enclosing.getType(name);
        
        throw new Error("Interpreter", "Unknown variable '" + name + "'", $"at [{name.lexeme}]", name.line);
    }
    
    public void assign(Token name, Object value)
    {
        
        if (variables.ContainsKey(name.lexeme))
        {
            variables[name.lexeme] = value;
            return;
        }
        
        if (enclosing != null) 
        {
            enclosing.assign(name, value);
            return;
        }

        throw new Error("Interpreter","Undefined variable '" + name.lexeme + "'" , $"at [{name.lexeme}]", name.line);
    }

    #region Error


    
    #endregion
    
}