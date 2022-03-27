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

            public List<Type>? getParameterTypes()
            {
                return null;
            } 
        }
        
        public class Sleep : Callable
        {
            public int arity()
            {
                return 1;
            }

            public object? call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                double time = (double)arguments[0];
                int timeInt = Convert.ToInt32(arguments[0]);

                if (time == timeInt)
                {
                    Thread.Sleep(timeInt);
                }
                else
                {
                    throw new Error("Native Function",
                        "Function sleep expected an integer. Round your number with round() if needed", "", -1);
                }

                return null;
            }

            public String toString()
            {
                return "<native fn>";
            }

            public List<Type>? getParameterTypes()
            {
                return new List<Type>
                {
                    typeof(Double)
                };
            } 
        }
        
        public class Round : Callable
        {
            public int arity()
            {
                return 1;
            }

            public object call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                return Math.Round((Double)arguments[0]);
            }

            public String toString()
            {
                return "<native fn>";
            }

            public List<Type>? getParameterTypes()
            {
                return new List<Type>
                {
                    typeof(Double)
                };
            } 
        }
    }
}