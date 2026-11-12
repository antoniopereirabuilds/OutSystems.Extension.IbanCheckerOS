namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.FormatIban"/> and <see cref="IbanNetService.FormatIbanBulk"/>.
/// </summary>
public class FormatIbanTests
{
    private static IbanFormatResult Format(string? iban, string? format)
    {
        TestData.Sut.FormatIban(iban!, format!, out var result);
        return result;
    }

    #region FormatIban

    [Theory]
    [InlineData("Electronic", TestData.NlValid)]
    [InlineData("Print", TestData.NlPrint)]
    [InlineData("Obfuscated", "XXXXXXXXXXXXXX4300")]
    [InlineData("Hidden", "****")]
    [InlineData("print", TestData.NlPrint)]
    [InlineData(" OBFUSCATED ", "XXXXXXXXXXXXXX4300")]
    [InlineData("hIdDeN", "****")]
    [InlineData("", TestData.NlValid)]
    [InlineData(null, TestData.NlValid)]
    public void FormatIban_ValidIban_ReturnsFormattedValue(string? format, string expected)
    {
        var r = Format(" nl91-abna 0417 1643 00 ", format);

        Assert.True(r.Success, r.ErrorMessage);
        Assert.Equal(expected, r.Value);
        Assert.Equal(TestData.NlValid, r.NormalizedIban);
    }

    [Theory]
    [InlineData("Pretty")]
    [InlineData("P")]
    [InlineData("1")]
    public void FormatIban_InvalidFormat_ReturnsInvalidFormat(string format)
    {
        var r = Format(TestData.NlValid, format);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.InvalidFormat, r.ErrorCode);
        Assert.Contains(format, r.ErrorMessage);
        Assert.Equal(string.Empty, r.Value);
        Assert.Equal(TestData.NlValid, r.NormalizedIban);
    }

    [Theory]
    [InlineData(null, IbanErrorCodes.Empty)]
    [InlineData("NL92ABNA0417164300", IbanErrorCodes.InvalidCheckDigits)]
    [InlineData("XX91ABNA0417164300", IbanErrorCodes.UnknownCountry)]
    public void FormatIban_InvalidIban_ReturnsValidationError(string? iban, string expectedCode)
    {
        var r = Format(iban, "Print");

        Assert.False(r.Success);
        Assert.Equal(expectedCode, r.ErrorCode);
        Assert.Equal(string.Empty, r.Value);
    }

    #endregion

    #region FormatIbanBulk

    [Fact]
    public void FormatIbanBulk_MixedInput_PreservesOrderAndCounts()
    {
        var input = TestData.L(TestData.GbValid, "nope", "nl91abna0417164300");

        TestData.Sut.FormatIbanBulk(input, "Print", out var r);

        Assert.True(r.Success);
        Assert.Equal(3, r.TotalCount);
        Assert.Equal(2, r.SuccessCount);
        Assert.Equal(1, r.FailureCount);
        Assert.Equal(new[] { 0, 1, 2 }, r.Results.Select(x => x.Index));
        Assert.Equal("GB29 NWBK 6016 1331 9268 19", r.Results[0].Value);
        Assert.False(r.Results[1].Success);
        Assert.Equal("NOPE", r.Results[1].NormalizedIban);
        Assert.Equal(TestData.NlPrint, r.Results[2].Value);
    }

    [Fact]
    public void FormatIbanBulk_InvalidFormat_ReturnsTopLevelError()
    {
        TestData.Sut.FormatIbanBulk(TestData.L(TestData.NlValid), "Fancy", out var r);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.InvalidFormat, r.ErrorCode);
        Assert.Equal(1, r.TotalCount);
        Assert.Empty(r.Results);
    }

    [Fact]
    public void FormatIbanBulk_OverCap_ReturnsTooManyItems()
    {
        var input = Enumerable.Repeat(TestData.NlValid, IbanNetService.MaxBulkItems + 1).ToList();

        TestData.Sut.FormatIbanBulk(input, "Electronic", out var r);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.TooManyItems, r.ErrorCode);
        Assert.Empty(r.Results);
    }

    #endregion
}
