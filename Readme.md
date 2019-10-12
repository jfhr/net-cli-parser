# net-cli-parser

Simple cross-platform command line parser for .NET applications.

```CS
using CliParser;

class SampleOptions
{
    [CliOption("--named-option", "-no")]
    public string NamedOption { get; set; }

    [CliOption("--flag", "-f")]
    public bool Flag { get; set; }
}

class Program 
{
    static void Main(string[] args)
    {
        var options = CliParser.Parse<SampleOptions>(args);
        // ...
    }
}
```
