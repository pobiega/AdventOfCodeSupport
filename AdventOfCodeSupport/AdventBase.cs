using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AdventOfCodeSupport;

/// <summary>
/// Registers self with AdventSolutions, and contains built-in
/// benchmarking support for BenchmarkDotNet.
/// </summary>
public abstract class AdventBase : IAoC
{
    /// <summary>
    /// Year of solution.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Day of solution i.e. 1-25.
    /// </summary>
    public int Day { get; }

    private string? _testInput;
    private string? _inputText;

    /// <summary>
    /// The entire text of the loaded input file.
    /// </summary>
    protected string InputText
    {
        get
        {
            if (_testInput is not null) return _testInput;
            if (_inputText is not null) return _inputText;
            try
            {
                var directory = new DirectoryInfo("../../../../").Name;
                var isBenchmark = directory.StartsWith("net");
                if (isBenchmark) Console.SetOut(TextWriter.Null);
                var relative = isBenchmark ? "../../../../../../../" : "../../../";
                _inputText = File.ReadAllText($"{relative}{Year}/Inputs/{Day.ToString("D2")}.txt");
            }
            catch (FileNotFoundException)
            {
                throw new Exception(@$"Input not found for {Year} - Day {Day.ToString("D2")}
Please ensure input is saved to ""Project Root/{Year}/Inputs/{Day.ToString("D2")}.txt""
If no input for the day, disable in the constructor with : base({Year}, {Day}, false)");
            }
            return _inputText;
        }
    }

    private string[]? _inputLines;

    /// <summary>
    /// The input file split on new lines, last empty line and trailing whitespace removed.
    /// </summary>
    protected string[] InputLines
    {
        get
        {
            if (_inputLines is not null) return _inputLines;
            _inputLines = InputText.Replace("\r", "").Trim().Split('\n');
            return _inputLines;
        }
    }

    /// <summary>
    /// Can be used for things like unit testing to pass information
    /// back to the test.
    /// </summary>
    public Dictionary<string, string> Bag { get; } = new();

    /// <summary>
    /// Registers self with AdventSolutions.
    /// </summary>
    public AdventBase()
    {
        var type = GetType();
        if (type.ToString().StartsWith("BenchmarkDotNet"))
        {
            type = type.BaseType;
        }
        var split = type!.FullName!.Split('.');
        var dayMatches = Regex.Matches(split[^1], @"0?(\d+)");
        var yearMatches = Regex.Matches(split[^2], @"^_(\d+)$");
        if (dayMatches.Count != 1 || dayMatches[0].Groups.Count != 2
            || yearMatches.Count != 1 || yearMatches[0].Groups.Count != 2)
        {
            throw new Exception($"Unable to automatically parse year/day from class: {type}");
        }
        Day = int.Parse(dayMatches[0].Groups[1].Value);
        Year = int.Parse(yearMatches[0].Groups[1].Value);
    }

    /// <summary>
    /// Registers self with AdventSolutions.
    /// </summary>
    /// <param name="year">Year of this AoC solution.</param>
    /// <param name="day">Day of this AoC solution i.e. 1-25.</param>
    [Obsolete("Constructor no longer required.")]
    protected AdventBase(int year, int day)
    {
        Year = year;
        Day = day;
    }

    /// <summary>
    /// Called from Part1().
    /// </summary>
    protected abstract void InternalPart1();

    /// <summary>
    /// Called from Part2().
    /// </summary>
    protected abstract void InternalPart2();

    /// <summary>
    /// Execute Part 1 of solution.
    /// </summary>
    /// <returns>Solution itself.</returns>
    [Benchmark]
    public IAoC Part1()
    {
        InternalPart1();
        return this;
    }

    /// <summary>
    /// Execute Part 2 of solution.
    /// </summary>
    /// <returns>Solution itself.</returns>
    [Benchmark]
    public IAoC Part2()
    {
        InternalPart2();
        return this;
    }

    /// <summary>
    /// Benchmark part 1 and 2 with BenchmarkDotNet.
    /// </summary>
    public void Benchmark()
    {
        BenchmarkRunner.Run(GetType());
    }

    /// <summary>
    /// Set a custom input to be used by InputText and InputLines instead of the
    /// automatically loaded day's input file.
    /// </summary>
    /// <param name="input">The custom input to test with.</param>
    public void SetTestInput(string? input)
    {
        _testInput = input;
    }
}
