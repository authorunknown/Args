using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Args
{
    public class Args : IArgParser
    {
        private List<IArgumentInfo> _args;
        private Dictionary<IArgumentInfo, object> _valueMap;

        private Args()
        {
            _args = new List<IArgumentInfo>();
        }

        public Args(string description)
            : this(description, Assembly.GetEntryAssembly())
        {
        }

        public Args(string description, Assembly programAssembly)
            : this()
        {
            UsagePrinter = new DefaultUsagePrinter();
            UsagePrinter.Description = description;
            UsagePrinter.Executable = programAssembly.GetName().Name;
        }

        public Args(IUsagePrinter usagePrinter)
            : this()
        {
            if (usagePrinter == null)
                throw new ArgumentNullException("usagePrinter");
            UsagePrinter = usagePrinter;
        }

        public void PrintUsage(TextWriter writer)
        {
            UsagePrinter.PrintUsage(writer, _args);
        }

        public CommandLineArgument<PType> Add<PType>(string shortName, string longName, string description, bool isRequired)
        {
            for (int i = 0; i < _args.Count; i++)
            {
                IArgumentInfo info = _args[i];
                if (info.ShortName == shortName)
                    throw new ArgumentException("An argument with the same name already exists: " + shortName);
                if (info.LongName == longName)
                    throw new ArgumentException("An argument with the same name already exists: " + longName);
            }

            var arg = new CommandLineArgument<PType>(this, shortName, longName, description, isRequired);
            _args.Add(arg);
            return arg;
        }

        public void Parse(string[] p)
        {
            _valueMap = new Dictionary<IArgumentInfo, object>();

            IsValid = true;
            IArgumentInfo[] args = _args.ToArray();
            int k = 0;
            while (k < p.Length)
            {
                string token = p[k];
                while (string.IsNullOrEmpty(token) && k < p.Length)
                {
                    k++;
                    token = p[k];
                }
                if (string.IsNullOrEmpty(token) || (token[0] != '-' && token[0] != '/'))
                {
                    IsValid = false;
                    return;
                }

                string tokenName;
                string tokenValue;

                int colonIndex = token.IndexOf(':');
                if (colonIndex > 0)
                {
                    tokenName = token.Substring(1, colonIndex - 1);
                    tokenValue = token.Substring(colonIndex + 1);
                }
                else
                {
                    tokenName = token.Substring(1);
                    k++;
                    if (k >= p.Length)
                    {
                        IsValid = false;
                        return;
                    }
                    else
                        tokenValue = p[k];
                }

                IArgumentInfo matchingArgument = null;
                for (int i = 0; i < args.Length; i++)
                {
                    IArgumentInfo arg = args[i];
                    if (arg.ShortName == tokenName || arg.LongName == tokenName)
                    {
                        matchingArgument = arg;
                        break;
                    }
                }
                if (matchingArgument == null || _valueMap.ContainsKey(matchingArgument))
                {
                    IsValid = false;
                    return;
                }

                object actualValue;
                if (TryGetActualValue(tokenValue, matchingArgument, out actualValue))
                {
                    _valueMap.Add(matchingArgument, actualValue);
                }
                else
                {
                    IsValid = false;
                    return;
                }
                k++;
            }

            for (int i = 0; i < args.Length; i++)
            {
                IArgumentInfo arg = args[i];
                if (arg.IsRequired && !_valueMap.ContainsKey(arg))
                {
                    IsValid = false;
                    return;
                }
            }
        }

        private bool TryGetActualValue(string tokenValue, IArgumentInfo matchingArgument, out object actualValue)
        {
            actualValue = null;
            try
            {
                if (typeof(int).Equals(matchingArgument.Type))
                    actualValue = int.Parse(tokenValue);
                else if (typeof(short).Equals(matchingArgument.Type))
                    actualValue = short.Parse(tokenValue);
                else if (typeof(long).Equals(matchingArgument.Type))
                    actualValue = long.Parse(tokenValue);
                else if (typeof(DateTime).Equals(matchingArgument.Type))
                    actualValue = DateTime.Parse(tokenValue);
                else if (typeof(string).Equals(matchingArgument.Type))
                {
                    if (this.TrimQuotes)
                        actualValue = tokenValue.Trim('\'', '"');
                    else
                        actualValue = tokenValue;
                }
                else
                    actualValue = Convert.ChangeType(tokenValue, matchingArgument.Type);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IUsagePrinter UsagePrinter { get; set; }

        public bool TrimQuotes { get; set; }

        #region IArgParser Members

        public bool IsValid { get; private set; }

        public bool IsMissing(IArgumentInfo arg)
        {
            if (_valueMap == null)
                throw new InvalidOperationException();
            return _valueMap.ContainsKey(arg);
        }

        public T GetValue<T>(IArgumentInfo arg)
        {
            if (_valueMap == null)
                throw new InvalidOperationException("Unable to get argument value.  Command line arguments have not been parsed.");
            if (_valueMap.ContainsKey(arg))
                return (T)_valueMap[arg];
            else
                return default(T);
        }

        #endregion
    }
}
