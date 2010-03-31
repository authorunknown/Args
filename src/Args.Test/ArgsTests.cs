using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Args.Test.Mocks;
using NUnit.Framework;

namespace Args.Test
{
    [TestFixture]
    [Category(TestCategories.Isolated)]
    public class ArgsTests
    {
        Args _args;
        MockUsagePrinter _usagePrinter;

        [SetUp]
        public void SetUp()
        {
            _usagePrinter = new MockUsagePrinter();
            _args = new Args(_usagePrinter);
        }

        [Test]
        public void PrintUsage_is_delegated_to_printer()
        {
            bool callWasDelegated = false;
            _usagePrinter.PrintUsage = (writer, args) => callWasDelegated = true;

            _args.PrintUsage(TextWriter.Null);

            Assert.That(callWasDelegated);
        }

        [Test]
        public void correct_argument_info_is_passed_to_usage_printer()
        {
            var arg1 = _args.Add<int>("i", "int", "int description", true);
            var arg2 = _args.Add<string>("s", "string", "string description", true);

            IEnumerable<IArgumentInfo> argsPrinted = null;
            _usagePrinter.PrintUsage = (writer, argInfo) =>
            {
                argsPrinted = argInfo;
            };

            _args.PrintUsage(TextWriter.Null);
            Assert.That(argsPrinted, !Is.Null);
            Assert.That(argsPrinted.Count(), Is.EqualTo(2));
            Assert.That(argsPrinted, Has.Member(arg1) & Has.Member(arg2));
        }

        [Test]
        public void constructor_with_usage_requires_value()
        {
            Error.Expect<ArgumentNullException>(() => new Args((IUsagePrinter)null));
        }

        [Test]
        public void string_assembly_constructor_sets_UsagePrinter()
        {
            Assert.That(new Args("foo", typeof(int).Assembly).UsagePrinter, !Is.Null);
        }

        [Test]
        public void short_form_syntax_is_parsed_correctly()
        {
            CommandLineArgument<int> p = _args.Add<int>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/p", "42" });
            Assert.That(p.Value, Is.EqualTo(42));

            _args.Parse(new[] { "-p", "0" });
            Assert.That(p.Value, Is.EqualTo(0));

            _args.Parse(new[] { "/p:3" });
            Assert.That(p.Value, Is.EqualTo(3));

            _args.Parse(new[] { "-p:5" });
            Assert.That(p.Value, Is.EqualTo(5));
        }

        [Test]
        public void long_form_syntax_is_parsed_correctly()
        {
            CommandLineArgument<int> cla = _args.Add<int>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/parm1", "42" });
            Assert.That(cla.Value, Is.EqualTo(42));

            _args.Parse(new[] { "-parm1", "0" });
            Assert.That(cla.Value, Is.EqualTo(0));

            _args.Parse(new[] { "/parm1:3" });
            Assert.That(cla.Value, Is.EqualTo(3));

            _args.Parse(new[] { "-parm1:5" });
            Assert.That(cla.Value, Is.EqualTo(5));
        }

        [Test]
        public void string_argments_are_parsed_correctly()
        {
            CommandLineArgument<string> p = _args.Add<string>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/p", "a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p", "a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "/p:a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p:a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));
        }

        [Test]
        public void quotes_are_trimmed_from_strings_when_TrimQuotes_true()
        {
            _args.TrimQuotes = true;
            CommandLineArgument<string> p = _args.Add<string>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/p", "\"a string\"" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p", "'a string'" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "/p:'a string'" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p:\"a string\"" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p:\"a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));

            _args.Parse(new[] { "-p:'a string" });
            Assert.That(p.Value, Is.EqualTo("a string"));
        }

        [Test]
        public void quotes_are_not_trimmed_from_strings_when_TrimQuotes_false()
        {
            _args.TrimQuotes = false;
            CommandLineArgument<string> p = _args.Add<string>("p", "parm1", "the one and only parameter", true);

            string value = "\"a string\"";
            _args.Parse(new[] { "/p", value });
            Assert.That(p.Value, Is.EqualTo(value));

            value = "'a string'";
            _args.Parse(new[] { "/p", value });
            Assert.That(p.Value, Is.EqualTo(value));

            _args.Parse(new[] { "/p:'a string'" });
            Assert.That(p.Value, Is.EqualTo("'a string'"));

            _args.Parse(new[] { "-p:\"a string\"" });
            Assert.That(p.Value, Is.EqualTo("\"a string\""));

            _args.Parse(new[] { "-p:\"a string" });
            Assert.That(p.Value, Is.EqualTo("\"a string"));

            _args.Parse(new[] { "-p:'a string" });
            Assert.That(p.Value, Is.EqualTo("'a string"));
        }

        [Test]
        public void IsValid_false_when_type_mismatch()
        {
            _args.Add<int>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/p", "a string" });

            Assert.That(_args.IsValid, Is.False);
        }

        [Test]
        public void IsValid_false_when_unrecognized_parameter()
        {
            _args.Add<int>("p", "parm1", "the one and only parameter", true);

            _args.Parse(new[] { "/k", "a string" });

            Assert.That(_args.IsValid, Is.False);
        }

        [Test]
        public void IsValid_false_when_required_parameter_missing()
        {
            _args.Add<DateTime>("p1", "parm1", "required paramater", true);
            _args.Add<int>("p2", "parm2", "optional parameter", false);

            _args.Parse(new[] { "p2", "10" });

            Assert.That(_args.IsValid, Is.False);
        }

        [Test]
        public void exception_is_thrown_when_shortName_is_duplicated()
        {
            _args.Add<int>("p", "parm1", "foo", true);
            ArgumentException ex = Error.Expect<ArgumentException>(() => _args.Add<int>("p", "parm2", "bar", false));
            Assert.That(ex.Message, Is.StringContaining("same name"));
        }

        [Test]
        public void exception_is_thrown_when_longName_is_duplicated()
        {
            _args.Add<int>("p1", "parm", "foo", true);
            ArgumentException ex = Error.Expect<ArgumentException>(() => _args.Add<int>("p2", "parm", "bar", false));
            Assert.That(ex.Message, Is.StringContaining("same name"));
        }

        [Test]
        public void exception_is_thrown_if_IsMissing_called_before_Parse()
        {
            var arg = _args.Add<int>("p", "p", "foo", true);
            bool isMissing;
            Error.Expect<InvalidOperationException>(() => isMissing = _args.IsMissing(arg));
        }

        [Test]
        public void IsValid_false_when_too_many_argments()
        {
            _args.Add<int>("i", "int", "int description", false);
            _args.Add<string>("s", "string", "string description", false);

            _args.Parse(new[] { "/s", "sval", "/i:1", "3" });
            Assert.That(_args.IsValid, Is.False);
        }

        [Test]
        public void IsValid_false_when_positional_parameter_is_passed()
        {
            _args.Add<int>("i", "int", "int description", false);
            _args.Add<string>("s", "string", "string description", false);

            _args.Parse(new[] { "sval", "/i:3" });
            Assert.That(_args.IsValid, Is.False);
        }

        [Test]
        public void GetValue_throws_InvalidOperationException_if_not_parsed()
        {
            var ex = Error.Expect<InvalidOperationException>(() => _args.GetValue<int>(new MockArgumentInfo()));
            Assert.That(ex.Message, Is.StringContaining("parse"));
        }
    }
}
