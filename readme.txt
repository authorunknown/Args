Args - Small, simple command line argument parsing utility for .NET

Licensed under MIT license.

Sample usage:

	class Program
	{
		static int Main(string[] args)
		{
			Args arguments = new Args("sample program that will echo an integer");
			var numberArgument = arguments.Add<int>("n", "number", "some random number", true);

			arguments.Parse(args);

			if (!arguments.IsValid)
			{
				arguments.PrintUsage(System.Console.Error);
				return 1;
			}

			int n = numberArgument.Value;

			System.Console.WriteLine(n);
		}
	}

Results of valid arguments:
---------------------------
> SampleProgram.exe /n 3
3
> SampleProgram.exe -n 3
3
> SampleProgram.exe /number 3
3
> SampleProgram.exe -number 3
3

> SampleProgram.exe /n:3
3
> SampleProgram.exe -n:3
3
> SampleProgram.exe /number:3
3
> SampleProgram.exe -number:3
3

Results of invalid arguments:
-----------------------------
> SampleProgram.exe 3
SampleProgram - sample program that will echo an integer

usage: SampleProgram -n <int>

  n, number - int; required.  some random number

>SampleProgram.exe n foo
SampleProgram - sample program that will echo an integer

usage: SampleProgram -n <int>

  n, number - int; required.  some random number
