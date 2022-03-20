using Chonker.Interpreter;
using Chonker.Tokens;

namespace Chonker.Environments;

public class Environment
{
    private Dictionary<string, object?> variables = new Dictionary<string, object?>();

    public void define(string name, object? value)
    {
        if (variables.Keys.Contains(name))
        {
            throw new InterpreterError($"Interpret error:\n Tried to declare variable " + name + ", but it already exists.");
        }
        
        variables.Add(name, value);
    }

    public object getValue(Token name)
    {
        if (variables.ContainsKey(name.lexeme))
        {
            return variables[name.lexeme]!;
        }
        
        throw new InterpreterError($"Interpret error:\n [{name.line}] Unknown variable '" + name + $"' at [{name.lexeme}].");
    }
    
    public void assign(Token name, Object value) {
        
        if (variables.ContainsKey(name.lexeme))
        {
            variables.Add(name.lexeme, value);
            return;
        }

        throw new InterpreterError($"Interpret error:\n  [{name.line}] Undefined variable '" + name.lexeme + $"' at [{name.lexeme}].");
    }

    #region Error


    
    #endregion
    
}