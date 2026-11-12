namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.ParseIban"/> and <see cref="IbanNetService.ParseIbanBulk"/>.
/// </summary>
public class ParseIbanTests
{
    private static IbanParseResult Parse(string? iban)
    {
        TestData.Sut.ParseIban(iban!, out var result);
        return result;
    }

    #region ParseIban

    [Fact]
    public void ParseIban_NlIban_ReturnsAllComponents()
    {
        var r = Parse("nl91 abna 0417 1643 00");

        Assert.True(r.Success);
        Assert.Equal(string.Empty, r.ErrorCode);
        Assert.Equal(TestData.NlValid, r.NormalizedIban);
        Assert.Equal(TestData.NlValid, r.Info.Electronic);
        Assert.Equal(TestData.NlPrint, r.Info.Print);
        Assert.Equal("XXXXXXXXXXXXXX4300", r.Info.Obfuscated);
        Assert.Equal("NL", r.Info.CountryCode);
        Assert.Contains("Netherlands", r.Info.CountryName);
        Assert.Equal("91", r.Info.CheckDigits);
        Assert.Equal("ABNA0417164300", r.Info.Bban);
        Assert.Equal("ABNA", r.Info.BankIdentifier);
        Assert.Equal(string.Empty, r.Info.BranchIdentifier);
        Assert.True(r.Info.IsSepaMember);
    }

    [Fact]
    public void ParseIban_GbIban_ReturnsBranchIdentifier()
    {
        var r = Parse(TestData.GbValid);

        Assert.True(r.Success);
        Assert.Equal("NWBK", r.Info.BankIdentifier);
        Assert.Equal("601613", r.Info.BranchIdentifier);
        Assert.Equal("29", r.Info.CheckDigits);
    }

    [Theory]
    [InlineData(null, IbanErrorCodes.Empty)]
    [InlineData("", IbanErrorCodes.Empty)]
    [InlineData("NL91ABNA041716430", IbanErrorCodes.InvalidLength)]
    [InlineData("NL91ABNA041716430$", IbanErrorCodes.IllegalCharacters)]
    [InlineData("NL91ABNA04171643X0", IbanErrorCodes.InvalidStructure)]
    [InlineData("NL92ABNA0417164300", IbanErrorCodes.InvalidCheckDigits)]
    [InlineData("XX91ABNA0417164300", IbanErrorCodes.UnknownCountry)]
    public void ParseIban_InvalidInput_ReturnsErrorWithoutThrowing(string? input, string expectedCode)
    {
        var r = Parse(input);

        Assert.False(r.Success);
        Assert.Equal(expectedCode, r.ErrorCode);
        Assert.NotEmpty(r.ErrorMessage);
        Assert.Equal(string.Empty, r.Info.Electronic);
    }

    #endregion

    #region ParseIbanBulk

    [Fact]
    public void ParseIbanBulk_MixedInput_PreservesOrderAndCounts()
    {
        var input = TestData.L("bad", TestData.GbValid, " nl91-abna-0417-1643-00 ");

        TestData.Sut.ParseIbanBulk(input, out var r);

        Assert.True(r.Success);
        Assert.Equal(3, r.TotalCount);
        Assert.Equal(2, r.SuccessCount);
        Assert.Equal(1, r.FailureCount);
        Assert.Equal(new[] { 0, 1, 2 }, r.Results.Select(x => x.Index));
        Assert.Equal(input, r.Results.Select(x => x.Input));
        Assert.False(r.Results[0].Success);
        Assert.Equal("GB", r.Results[1].Info.CountryCode);
        Assert.Equal(TestData.NlValid, r.Results[2].NormalizedIban);
        Assert.Equal(TestData.NlPrint, r.Results[2].Info.Print);
    }

    [Fact]
    public void ParseIbanBulk_NullList_ReturnsEmptySuccess()
    {
        TestData.Sut.ParseIbanBulk(null!, out var r);

        Assert.True(r.Success);
        Assert.Equal(0, r.TotalCount);
        Assert.Empty(r.Results);
    }

    [Fact]
    public void ParseIbanBulk_OverCap_ReturnsTooManyItems()
    {
        var input = Enumerable.Repeat(TestData.NlValid, IbanNetService.MaxBulkItems + 1).ToList();

        TestData.Sut.ParseIbanBulk(input, out var r);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.TooManyItems, r.ErrorCode);
        Assert.Empty(r.Results);
    }

    #endregion
}
