namespace Chonker.Functions;

public interface Callable
{
    object call(Interpreter.Interpreter interpreter, List<Object> arguments);
    int arity();
    List<Type>? getParameterTypes();
}