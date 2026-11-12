namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.GetCountryInfo"/> and <see cref="IbanNetService.ListSupportedCountries"/>.
/// </summary>
public class CountriesTests
{
    private static IbanCountryResult Get(string? code)
    {
        TestData.Sut.GetCountryInfo(code!, out var result);
        return result;
    }

    [Fact]
    public void GetCountryInfo_Nl_ReturnsRegistryDefinition()
    {
        var r = Get("NL");

        Assert.True(r.Found);
        Assert.Equal("NL", r.Country.Code);
        Assert.Contains("Netherlands", r.Country.EnglishName);
        Assert.Equal(18, r.Country.IbanLength);
        Assert.Equal(TestData.NlValid, r.Country.IbanExample);
        Assert.Equal("ABNA0417164300", r.Country.BbanExample);
        Assert.True(r.Country.IsSepaMember);
        Assert.True(r.Country.HasBankIdentifier);
        Assert.False(r.Country.HasBranchIdentifier);
        Assert.Equal(new DateTime(2007, 4, 1), r.Country.EffectiveDate.Date);
        Assert.Equal(DateTimeKind.Utc, r.Country.EffectiveDate.Kind);
    }

    [Fact]
    public void GetCountryInfo_GbLowercaseWithSpaces_ReturnsBranchIdentifier()
    {
        var r = Get(" gb ");

        Assert.True(r.Found);
        Assert.Equal("GB", r.Country.Code);
        Assert.Equal(22, r.Country.IbanLength);
        Assert.True(r.Country.HasBranchIdentifier);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("XX")]
    [InlineData("US")]
    [InlineData("NLD")]
    public void GetCountryInfo_UnknownOrEmpty_ReturnsNotFound(string? code)
    {
        var r = Get(code);

        Assert.False(r.Found);
        Assert.Equal(string.Empty, r.Country.Code);
    }

    [Fact]
    public void ListSupportedCountries_ReturnsOrderedDistinctCountries()
    {
        TestData.Sut.ListSupportedCountries(out var countries);

        Assert.True(countries.Count >= 80, $"Expected at least 80 countries, got {countries.Count}");
        var codes = countries.Select(c => c.Code).ToList();
        Assert.Equal(codes.OrderBy(c => c, StringComparer.Ordinal), codes);
        Assert.Equal(codes.Count, codes.Distinct().Count());
        Assert.Contains("NL", codes);
        Assert.Contains("DE", codes);
        Assert.All(countries, c =>
        {
            Assert.Equal(2, c.Code.Length);
            Assert.True(c.IbanLength > 4);
            Assert.Equal(c.IbanLength, c.IbanExample.Length);
            Assert.NotEmpty(c.EnglishName);
        });
    }

    [Fact]
    public void ListSupportedCountries_EveryIbanExample_PassesValidation()
    {
        TestData.Sut.ListSupportedCountries(out var countries);

        Assert.All(countries, c => Assert.True(TestData.Validate(c.IbanExample).IsValid, $"{c.Code}: {c.IbanExample}"));
    }

    [Fact]
    public void ListSupportedCountries_ReturnedListMutation_DoesNotAffectLaterCalls()
    {
        TestData.Sut.ListSupportedCountries(out var first);
        int count = first.Count;
        first.Clear();

        TestData.Sut.ListSupportedCountries(out var second);

        Assert.Equal(count, second.Count);
    }
}
