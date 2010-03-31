using System;

namespace Args.Test.Mocks
{
    public class MockArgParser : IArgParser
    {
        public bool IsValid = true;
        public Func<IArgumentInfo, bool> IsMissing;
        public Func<IArgumentInfo, object> GetValue;

        #region IArgParser Members

        bool IArgParser.IsValid
        {
            get { return IsValid; }
        }

        bool IArgParser.IsMissing(IArgumentInfo arg)
        {
            if (IsMissing != null)
                return IsMissing(arg);
            return false;
        }

        T IArgParser.GetValue<T>(IArgumentInfo arg)
        {
            if (GetValue != null)
                return (T)GetValue(arg);
            return default(T);
        }

        #endregion
    }

}
