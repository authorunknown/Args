using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Args
{
    public interface IArgumentInfo
    {
        string ShortName { get; }
        string LongName { get; }
        string Description { get; }
        bool IsRequired { get; }
        Type Type { get; }
    }
}
