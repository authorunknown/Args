using System;
using NUnit.Framework;

namespace Args.Test
{
    public static class Error
    {
        /// <summary>
        /// Different from Assert.Throws in that this will pass for any exception that is assignable to T.
        /// Assert.Throws requires an exact match on the exception type.
        /// </summary>
        public static T Expect<T>(Action f) where T : Exception
        {
            T expectedException = null;
            Exception actualException = null;

            try
            {
                f();
            }
            catch (Exception ex)
            {
                actualException = ex;
                expectedException = ex as T;
            }

            if (actualException == null)
            {
                string message = string.Format(
                    "An exception of type {0} was expected, but no exception was thrown.",
                    typeof(T).FullName);
                Assert.Fail(message);
            }
            else if (expectedException == null)
            {
                string message = string.Format(
                    "An exception of type {0} was expected, but an exception of type {1} was thrown instead.",
                    typeof(T).FullName,
                    actualException.GetType().FullName);
                Assert.Fail(message);
            }

            return expectedException;
        }
    }
}
