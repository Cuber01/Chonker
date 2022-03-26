namespace Chonker.Functions
{
    public static class NativeFunctions
    {
        public class Clock : Callable
        {
            public int arity()
            {
                return 0;
            }

            public object call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                return DateTime.Now;
            }

            public String toString()
            {
                return "<native fn>";
            }
        }
    }
}