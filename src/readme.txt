Args - Small, simple command line argument parsing utility for .NET

Licensed under MIT license.

Sample usage:

	class Program
	{
		static int Main(string[] args)
		{
			Args arguments = new Args("SampleProgram.exe");
			var numberArgument = arguments.Add<int>("n", "number", "some random number", true);

			arguments.Parse(args);

			if (!arguments.IsValid)
			{
				arguments.PrintUsage(System.Console.Error);
				return 1;
			}

			int n = numberArgument.Value;

			//do something
		}
	}

> SampleProgram.exe /n 3
> SampleProgram.exe -n 3
> SampleProgram.exe /n:3
> SampleProgram.exe -n:3