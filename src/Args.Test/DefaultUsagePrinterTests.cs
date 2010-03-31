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
    public class DefaultUsagePrinterTests
    {
        private const string ProgramDescription = "this message will self destruct in 10 seconds";
        private const string ExecutableName = "my-program";

        List<IArgumentInfo> _args;
        DefaultUsagePrinter _printer;

        [SetUp]
        public void SetUp()
        {
            _args = new List<IArgumentInfo>();
            _printer = new DefaultUsagePrinter();
            _printer.Description = ProgramDescription;
            _printer.Executable = ExecutableName;
        }

        [Test]
        public void PrintUsage_displays_one_parameter_indented_on_line_separate_from_description()
        {
            _args.Add(CreateArgInfo<int>("p1", "parm1", "the one and only parameter", true));

            string[] lines = GetLines();

            Assert.That(lines.Length, Is.GreaterThanOrEqualTo(2));
            string descriptionLine = lines[0];
            string parmLine = lines
                .Skip(1)
                .Where(line => line.Contains("parm1"))
                .First();

            //string should 
            //  -start with whitespace (assumption is that said whitespace is indentation)
            //  -both short name and long name are present
            //  -description is present
            Assert.That(parmLine, Is.StringMatching(@"^\s+") &
                                  Is.StringContaining("p1") &
                                  Is.StringContaining("parm1") &
                                  Is.StringContaining("the one and only parameter"));
        }

        [Test]
        public void PrintUsage_displays_sample_usage_with_required_indicators()
        {
            _args.Add(CreateArgInfo<int>("p1", "parm1", "optional", false));
            _args.Add(CreateArgInfo<string>("p2", "parm2", "required", true));

            string usage = GetUsage();

            Assert.That(usage, Is.StringContaining(ExecutableName + " [-p1 <int>] -p2 <string>") |
                               Is.StringContaining(ExecutableName + " -p2 <string> [-p1 <int>]"));
        }

        [Test]
        public void first_line_contains_program_description_and_name()
        {
            string[] usage = GetLines();

            Assert.That(usage[0], Is.StringContaining(ProgramDescription) & Is.StringContaining(ExecutableName));
        }

        [Test]
        public void correct_message_is_printed_for_multiple_arguments()
        {
            string ExpectedUsage = string.Join(Environment.NewLine, new[]
            {
                "program.exe - <program description>",
                "",
                "usage: program.exe [-p1 <int>] -p2 <string> ",
                "",
                "  p1,parm1 - int; optional.  <p1 description>",
                "  p2,parm2 - string; required.  <p2 description>",
                ""
            });
            _printer.Description = "<program description>";
            _printer.Executable = "program.exe";

            _args.Add(CreateArgInfo<int>("p1", "parm1", "<p1 description>", false));
            _args.Add(CreateArgInfo<string>("p2", "parm2", "<p2 description>", true));

            Assert.That(GetUsage(), Is.EqualTo(ExpectedUsage));
        }

        private MockArgumentInfo CreateArgInfo<T1>(string shortName, string longName, string description, bool isRequired)
        {
            return new MockArgumentInfo
            {
                ShortName = shortName,
                LongName = longName,
                Description = description,
                IsRequired = isRequired,
                Type = typeof(T1)
            };
        }

        private string GetUsage()
        {
            StringWriter writer = new StringWriter();
            _printer.PrintUsage(writer, _args);
            return writer.ToString();
        }

        private string[] GetLines()
        {
            return GetUsage().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
