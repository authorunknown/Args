using System;
using System.Collections.Generic;
using System.IO;

namespace Args.Test.Mocks
{
    public class MockUsagePrinter : IUsagePrinter
    {
        public Action<TextWriter, IEnumerable<IArgumentInfo>> PrintUsage;

        #region IUsagePrinter Members

        public string Executable { get; set; }

        public string Description { get; set; }

        void IUsagePrinter.PrintUsage(System.IO.TextWriter writer, IEnumerable<IArgumentInfo> args)
        {
            if (PrintUsage != null)
                PrintUsage(writer, args);
        }

        #endregion
    }
}
