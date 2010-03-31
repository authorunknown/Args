using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Args
{
    public interface IUsagePrinter
    {
        string Executable { get; set; }
        string Description { get; set; }
        void PrintUsage(TextWriter writer, IEnumerable<IArgumentInfo> args);
    }
}
