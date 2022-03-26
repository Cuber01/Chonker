

namespace Chonker.Environments;

public abstract class VariableType : Type
{
    public override string ToString()
    {
        switch (Name)
        {
            case "Double":  return "number";
            case "Boolean": return "bool";
            case "Void":    return "void";
            case "String":  return "string";
            
            default: throw new Error("Internal", "Unknown type entered in VariableType", "", -1);
        }
    }
}