namespace Chonker.Functions
{
    public static class NativeFunctions
    {
        public abstract class NativeFunction : Callable
        {
            public abstract object? call(Interpreter.Interpreter interpreter, List<object> arguments);

            public abstract int arity();

            public abstract List<Type>? getParameterTypes();

            public override string ToString()
            {
                return "<native function>";
            }
        }
        
        public class Clock : NativeFunction
        {
            public override int arity() => 1;

            public override object call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                return DateTime.Now;
            }

            public override List<Type>? getParameterTypes()
            {
                return null;
            } 
        }
        
        public class Sleep : NativeFunction
        {
            public override int arity() => 1;

            public override object? call(Interpreter.Interpreter interpreter, List<Object> arguments)
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

            public override List<Type>? getParameterTypes()
            {
                return new List<Type>
                {
                    typeof(Double)
                };
            } 
        }
        
        public class Round : NativeFunction
        {
            public override int arity() => 1;

            public override object call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                return Math.Round((Double)arguments[0]);
            }
            

            public override List<Type>? getParameterTypes()
            {
                return new List<Type>
                {
                    typeof(Double)
                };
            } 
        }
        
        public class Count : NativeFunction
        {
            public override int arity() => 1;

            public override object call(Interpreter.Interpreter interpreter, List<Object> arguments)
            {
                return Math.Round((Double)arguments[0]);
            }

            public override List<Type>? getParameterTypes()
            {
                return new List<Type>
                {
                    typeof(List<object?>)
                };
            } 
        }
    }
}