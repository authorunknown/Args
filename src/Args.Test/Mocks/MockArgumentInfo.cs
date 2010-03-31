using System;

namespace Args.Test.Mocks
{
    public class MockArgumentInfo : IArgumentInfo
    {
        public string ShortName;
        public string LongName;
        public string Description;
        public Type Type;
        public bool IsRequired;

        #region IArgumentInfo Members

        string IArgumentInfo.ShortName
        {
            get { return ShortName; }
        }

        string IArgumentInfo.LongName
        {
            get { return LongName; }
        }

        string IArgumentInfo.Description
        {
            get { return Description; }
        }

        Type IArgumentInfo.Type
        {
            get { return Type; }
        }

        bool IArgumentInfo.IsRequired
        {
            get { return IsRequired; }
        }

        #endregion
    }
}
