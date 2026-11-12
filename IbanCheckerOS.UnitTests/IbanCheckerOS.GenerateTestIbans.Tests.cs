namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.GenerateTestIbans"/>: validity across every registry country,
/// count limits and error handling.
/// </summary>
public class GenerateTestIbansTests
{
    private static IbanGenerateResult Generate(string? code, int count)
    {
        TestData.Sut.GenerateTestIbans(code!, count, out var result);
        return result;
    }

    public static TheoryData<string> AllCountryCodes()
    {
        TestData.Sut.ListSupportedCountries(out var countries);
        var data = new TheoryData<string>();
        foreach (var c in countries)
        {
            data.Add(c.Code);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllCountryCodes))]
    public void GenerateTestIbans_EveryCountry_GeneratesValidIbans(string code)
    {
        var r = Generate(code, 25);

        Assert.True(r.Success, r.ErrorMessage);
        Assert.Equal(string.Empty, r.ErrorCode);
        Assert.Equal(25, r.Ibans.Count);
        Assert.All(r.Ibans, iban =>
        {
            var v = TestData.Validate(iban);
            Assert.True(v.IsValid, $"{iban}: {v.ErrorCode}");
            Assert.Equal(code, v.CountryCode);
            Assert.Equal(iban, v.NormalizedIban);
        });
    }

    [Fact]
    public void GenerateTestIbans_LowercaseCode_IsNormalized()
    {
        var r = Generate(" de ", 3);

        Assert.True(r.Success);
        Assert.All(r.Ibans, i => Assert.StartsWith("DE", i));
    }

    [Fact]
    public void GenerateTestIbans_MaxCount_GeneratesThatMany()
    {
        var r = Generate("NL", IbanNetService.MaxGeneratedIbans);

        Assert.True(r.Success);
        Assert.Equal(IbanNetService.MaxGeneratedIbans, r.Ibans.Count);
        Assert.True(r.Ibans.Distinct().Count() > 1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    public void GenerateTestIbans_CountOutOfRange_ReturnsInvalidCount(int count)
    {
        var r = Generate("NL", count);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.InvalidCount, r.ErrorCode);
        Assert.Empty(r.Ibans);
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("US")]
    public void GenerateTestIbans_UnknownCountry_ReturnsUnknownCountry(string code)
    {
        var r = Generate(code, 1);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.UnknownCountry, r.ErrorCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void GenerateTestIbans_EmptyCountry_ReturnsEmpty(string? code)
    {
        var r = Generate(code, 1);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.Empty, r.ErrorCode);
    }
}
