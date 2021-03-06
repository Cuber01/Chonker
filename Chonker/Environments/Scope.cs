using Chonker.Interpreter;
using Chonker.Tokens;

namespace Chonker.Environments;

public class Scope
{
    private readonly Dictionary<string, (Type, object?)> variables = new Dictionary<string, (Type, object?)>();
    private readonly Scope? enclosing;


    public Scope(Scope? enclosing)
    {
        this.enclosing = enclosing;
    }
    
    public void define(Token name, Type type, object? value)
    {
        if (variables.Keys.Contains(name.lexeme))
        {
            throw new Error("Interpret","Tried to declare variable " + name.lexeme + ", but it already exists", "", name.line);
        }

        variables.Add(name.lexeme, (type, value));
    }
    
    public void define(string name, int line, Type type, object? value)
    {
        if (variables.Keys.Contains(name))
        {
            throw new Error("Interpret","Tried to declare function " + name + ", but it already exists", "", line);
        }

        variables.Add(name, (type, value));
    }

    public object getValue(Token name)
    {
        if (variables.ContainsKey(name.lexeme))
        {
            return variables[name.lexeme].Item2!;
        }
        
        if (enclosing != null) return enclosing.getValue(name);
        
        throw new Error("Interpreter", "Unknown variable '" + name + "'", $"[{name.lexeme}]", name.line);
    }

    public Type? getType(Token name)
    {
        if (variables.ContainsKey(name.lexeme))
        {
            return variables[name.lexeme].Item2!.GetType();
        }

        if (enclosing != null) return enclosing.getType(name);
        
        throw new Error("Interpreter", "Unknown variable '" + name + "'", $"[{name.lexeme}]", name.line);
    }
    
    public void assign(Token name, Object value)
    {
        
        if (variables.ContainsKey(name.lexeme))
        {
            // Type stays the same
            variables[name.lexeme] = (variables[name.lexeme].Item1, value);
            return;
        }
        
        if (enclosing != null) 
        {
            enclosing.assign(name, value);
            return;
        }

        throw new Error("Interpreter","Undefined variable '" + name.lexeme + "'" , $"[{name.lexeme}]", name.line);
    }

    #region Error


    
    #endregion
    
}