using System.Xml;
using Chonker.Environments;
using Chonker.Leaves;

namespace Chonker.Functions;

public class Function : Callable
{
    private readonly FunctionStmt declaration;

    public Function(FunctionStmt declaration)
    {
        this.declaration = declaration;
    }

    public object? call(Interpreter.Interpreter interpreter, List<object> arguments)
    {
        Scope scope = new Scope(interpreter.globals);
        
        for (int i = 0; i < declaration.parameters.Count; i++)
        {
            scope.define(declaration.parameters.ElementAt(i).Item1.lexeme, declaration.parameters.ElementAt(i).Item1.line,
            arguments.ElementAt(i).GetType(), arguments.ElementAt(i));
        }

        try
        {
            interpreter.executeBlock(declaration.body, scope);
        } catch (Return returnValue)
        {
            if (returnValue.isEmpty) return new Empty();
            if (returnValue.value is null) return null;
            
            if (returnValue.value.GetType() != declaration.returnType)
            {
                throw new Error("Interpreter",
                    "Expected return type " + declaration.returnType + " but got " + returnValue.value!.GetType(),
                    "return", declaration.name.line);
            }
                
            return returnValue.value;
        }

        return null;
    }
    
    public String toString()
    {
        return "<fn " + declaration.name.lexeme + ">";
    }

    public int arity()
    {
        return declaration.parameters.Count;
    }

    public List<Type>? getParameterTypes()
    {
        if (declaration.parameters.Count == 0) return null;
        
        List<Type> rv = new List<Type>();

        foreach (var param in declaration.parameters)
        {
            rv.Add(param.Item2);
        }

        return rv;
    }
}
