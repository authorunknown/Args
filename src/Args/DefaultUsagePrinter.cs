using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Args
{
    public class DefaultUsagePrinter : IUsagePrinter
    {
        private TextWriter writer;
        private IEnumerable<IArgumentInfo> args;

        #region IUsagePrinter Members

        public string Executable { get; set; }

        public string Description { get; set; }

        public void PrintUsage(System.IO.TextWriter writer, IEnumerable<IArgumentInfo> args)
        {
            this.writer = writer;
            this.args = args;
            if (!string.IsNullOrEmpty(Executable) && !string.IsNullOrEmpty(Description))
                PrintProgramLine();
            if (!string.IsNullOrEmpty(Executable))
                PrintUsageLine();
            PrintArgs();
        }

        private void PrintProgramLine()
        {
            writer.Write(Executable);
            writer.Write(" - ");
            writer.Write(Description);
            writer.WriteLine();
            writer.WriteLine();
        }

        private void PrintUsageLine()
        {
            writer.Write("usage: ");
            writer.Write(Executable);
            writer.Write(' ');
            foreach (IArgumentInfo arg in args)
            {
                if (!arg.IsRequired)
                    writer.Write('[');
                writer.Write('-');
                writer.Write(arg.ShortName);
                writer.Write(" <");
                writer.Write(GetTypeString(arg.Type));
                writer.Write('>');
                if (!arg.IsRequired)
                    writer.Write(']');
                writer.Write(' ');
            }
            writer.WriteLine();
            writer.WriteLine();
        }

        private void PrintArgs()
        {
            foreach (IArgumentInfo arg in args)
            {
                writer.Write(' ');
                writer.Write(' ');
                writer.Write(arg.ShortName);
                writer.Write(',');
                writer.Write(arg.LongName);
                writer.Write(" - ");
                writer.Write(GetTypeString(arg.Type));
                writer.Write("; ");
                if (arg.IsRequired)
                    writer.Write("required.  ");
                else
                    writer.Write("optional.  ");
                writer.Write(arg.Description);
                writer.WriteLine();
            }
        }

        private string GetTypeString(Type type)
        {
            if (typeof(string).Equals(type))
                return "string";
            else if (typeof(int).Equals(type))
                return "int";
            else if (typeof(double).Equals(type))
                return "double";
            else if (typeof(byte).Equals(type))
                return "byte";
            else if (typeof(short).Equals(type))
                return "short";
            else if (typeof(uint).Equals(type))
                return "uint";
            else if (typeof(long).Equals(type))
                return "long";
            else if (typeof(ulong).Equals(type))
                return "ulong";
            else if (typeof(decimal).Equals(type))
                return "decimal";
            else if (typeof(DateTime).Equals(type))
                return "date";
            else if (typeof(char).Equals(type))
                return "char";
            else if (typeof(float).Equals(type))
                return "float";
            else if (typeof(sbyte).Equals(type))
                return "sbyte";
            else if (typeof(ushort).Equals(type))
                return "ushort";
            else
                return type.Name;
        }

        #endregion
    }
}
