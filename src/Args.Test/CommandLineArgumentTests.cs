using System;
using Args.Test.Mocks;
using NUnit.Framework;

namespace Args.Test
{
    [TestFixture]
    [Category(TestCategories.Isolated)]
    public class CommandLineArgumentTests
    {
        MockArgParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new MockArgParser();
        }

        [Test]
        public void exception_is_thrown_when_argments_are_not_valid()
        {
            parser.IsValid = false;

            CommandLineArgument<int> arg = new CommandLineArgument<int>(parser, "shortName", "longName", "description", true);

            int value;
            Error.Expect<InvalidOperationException>(() =>
            {
                value = arg.Value;
            });
        }

        [Test]
        public void value_is_take_from_the_parser()
        {
            CommandLineArgument<int> arg = new CommandLineArgument<int>(parser, "shortName", "longName", "description", true);

            parser.GetValue = (a) =>
            {
                if (a == arg)
                    return 7;
                return -1;
            };

            Assert.That(arg.Value, Is.EqualTo(7));
        }
    }
}
