using System;

namespace Args
{
    public class CommandLineArgument<T> : IArgumentInfo
    {
        private IArgParser _parser;

        public CommandLineArgument(IArgParser parser, string shortName, string longName, string description, bool isRequired)
        {
            _parser = parser;
            this.ShortName = shortName;
            this.LongName = longName;
            this.Description = description;
            this.IsRequired = isRequired;
        }

        public string ShortName { get; private set; }

        public string LongName { get; private set; }

        public string Description { get; private set; }

        public bool IsRequired { get; private set; }

        public T Value
        {
            get
            {
                if (!_parser.IsValid)
                    throw new InvalidOperationException("Unable to get the value of " + this.ShortName + ".  The command line arguments are not valid.");
                return _parser.GetValue<T>(this);
            }
        }

        public Type Type
        {
            get { return typeof(T); }
        }
    }
}
