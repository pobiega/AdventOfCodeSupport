namespace PackageTests;

/// <summary>
/// Tests utilizing the pattern defaults
/// </summary>
public class PackageTests
{
    private readonly AdventSolutions _solutions = new();
    private readonly string _testHtmlCheckNoAnswers = File.ReadAllText("TestHtml/CheckNoAnswers.html");
    private readonly string _testHtmlCheckSingleAnswer = File.ReadAllText("TestHtml/CheckSingleAnswer.html");
    private readonly string _testHtmlSubmitIncorrect = File.ReadAllText("TestHtml/SubmitIncorrect.html");

    public static IEnumerable<object[]> YearDays = new List<object[]>
    {
        new object[] { 2022, 4 },
        new object[] { 2022, 12 },
        new object[] { 2021, 25 }
    };

    [Theory]
    [MemberData(nameof(YearDays))]
    public void DayTest_GetSpecificDay_ReturnsCorrectDay(int year, int day)
    {
        var result = _solutions.GetDay(year, day);
        Assert.True(result.Year == year && result.Day == day);
    }

    [Fact]
    public void DayTest_MostRecentDayNoYearSpecified_ReturnsLatestDayOfLatestYear()
    {
        var mostRecent = _solutions.GetMostRecentDay();
        Assert.True(mostRecent is { Year: 2022, Day: 12 });
    }

    [Fact]
    public void DayTest_MostRecentDayWithYear_ReturnsLatestDayOfSpecifiedYear()
    {
        var mostRecent = _solutions.GetMostRecentDay(2021);
        Assert.True(mostRecent is { Year: 2021, Day: 25 });
    }

    [Fact]
    public void InputTest_AllText_TextLoaded()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Contains("Test", day.GetBag()["Text"]);
    }

    [Fact]
    public void InputTest_CustomInput_TextLoaded()
    {
        var day = _solutions.GetDay(2022, 4);
        day.SetTestInput("""
            123
            456
            """);
        day.Part1();
        Assert.StartsWith("123", day.GetBag()["Text"]);
    }

    [Fact]
    public void InputTest_AllLines_LinesLoaded()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Equal("4", day.GetBag()["Lines"]);
    }

    [Fact]
    public void InputTest_AllLines_LeadingWhiteSpaceRetained()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Equal(" Test", day.GetBag()["FirstLine"]);
    }

    [Fact]
    public void InputTest_Blocks_NumberOfBlocks()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Equal("2", day.GetBag()["Blocks"]);
    }

    [Fact]
    public void InputTest_Blocks_FirstBlockLines()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Equal("2", day.GetBag()["Block1Lines"]);
    }

    [Fact]
    public void InputTest_Blocks_FirstBlockLeadingWhitespaceRetained()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.StartsWith(" Test", day.GetBag()["Block1Text"]);
    }

    [Fact]
    public void InputTest_Blocks_SecondBlockLines()
    {
        var day = _solutions.GetDay(2022, 4);
        day.Part1();
        Assert.Equal("1", day.GetBag()["Block2Lines"]);
    }

    [Fact]
    public void LoadDay_CheckDay12_ValidYearDay()
    {
        var day = _solutions.GetDay(2022, 12);
        Assert.IsType<TestDay12>(day);
        Assert.True(day is { Year: 2022, Day: 12 });
    }

    [Fact]
    public async Task TestHtml_CheckAnswerAgainstNoAnswer_NullResult()
    {
        var day = _solutions.GetDay(2022, 4);
        day.SetTestHtmlResults(_testHtmlSubmitIncorrect, _testHtmlCheckNoAnswers);
        var result = await day.CheckPart1Async();
        Assert.Null(result);
    }

    [Fact]
    public async Task TestHtml_CheckAnswerAgainstSingleAnswer_TrueResult()
    {
        var day = _solutions.GetDay(2022, 4);
        day.SetTestHtmlResults(_testHtmlSubmitIncorrect, _testHtmlCheckSingleAnswer);
        var result = await day.CheckPart1Async();
        Assert.True(result);
    }

    [Fact]
    public async Task TestHtml_SubmitAnswerWhenAlreadySubmitted_NullResult()
    {
        var day = _solutions.GetDay(2022, 4);
        day.SetTestHtmlResults(_testHtmlSubmitIncorrect, _testHtmlCheckSingleAnswer);
        var result = await day.SubmitPart1Async();
        Assert.Null(result);
    }

    [Fact]
    public async Task TestHtml_SubmitAnswer_FalseResult()
    {
        var day = _solutions.GetDay(2022, 4);
        day.SetTestHtmlResults(_testHtmlSubmitIncorrect, _testHtmlCheckNoAnswers);
        var result = await day.SubmitPart1Async();
        Assert.False(result);
    }
}
