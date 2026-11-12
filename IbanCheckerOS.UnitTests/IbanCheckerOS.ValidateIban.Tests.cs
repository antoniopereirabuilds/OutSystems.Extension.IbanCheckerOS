namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.ValidateIban"/>: valid IBANs, every error code,
/// input normalization and the allowed/rejected country rules.
/// </summary>
public class ValidateIbanTests
{
    #region Valid IBANs

    [Theory]
    [InlineData(TestData.NlValid, "NL")]
    [InlineData(TestData.DeValid, "DE")]
    [InlineData(TestData.GbValid, "GB")]
    [InlineData(TestData.FrValid, "FR")]
    [InlineData(TestData.PtValid, "PT")]
    [InlineData(TestData.BeValid, "BE")]
    public void ValidateIban_ValidIban_ReturnsValid(string iban, string country)
    {
        var r = TestData.Validate(iban);

        Assert.True(r.IsValid);
        Assert.Equal(string.Empty, r.ErrorCode);
        Assert.Equal(string.Empty, r.ErrorMessage);
        Assert.Equal(country, r.CountryCode);
        Assert.Equal(iban, r.NormalizedIban);
    }

    #endregion

    #region Normalization

    [Theory]
    [InlineData("NL91 ABNA 0417 1643 00")]
    [InlineData("  NL91ABNA0417164300  ")]
    [InlineData("nl91abna0417164300")]
    [InlineData("NL91-ABNA-0417-1643-00")]
    [InlineData(" nl91 - abna\t0417\n1643 00 ")]
    [InlineData("NL91 ABNA–0417164300")]
    public void ValidateIban_UnnormalizedInput_IsNormalizedAndValid(string input)
    {
        var r = TestData.Validate(input);

        Assert.True(r.IsValid, r.ErrorMessage);
        Assert.Equal(TestData.NlValid, r.NormalizedIban);
    }

    [Fact]
    public void ValidateIban_InvalidInput_StillReturnsNormalizedIban()
    {
        var r = TestData.Validate(" nl92-abna 0417 1643 00 ");

        Assert.False(r.IsValid);
        Assert.Equal("NL92ABNA0417164300", r.NormalizedIban);
    }

    #endregion

    #region Error codes

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" - - ")]
    public void ValidateIban_NullOrEmpty_ReturnsEmptyError(string? input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.Empty, r.ErrorCode);
        Assert.NotEmpty(r.ErrorMessage);
        Assert.Equal(string.Empty, r.NormalizedIban);
    }

    [Theory]
    [InlineData("NL91ABNA041716430")]
    [InlineData("NL91ABNA04171643000")]
    public void ValidateIban_WrongLength_ReturnsInvalidLength(string input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.InvalidLength, r.ErrorCode);
        Assert.Equal("NL", r.CountryCode);
        Assert.Contains("18", r.ErrorMessage);
    }

    [Theory]
    [InlineData("NL91ABNA041716430$")]
    [InlineData("NL91ABNA0417.64300")]
    [InlineData("1291ABNA0417164300")]
    public void ValidateIban_IllegalCharacters_ReturnsIllegalCharacters(string input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.IllegalCharacters, r.ErrorCode);
        Assert.NotEmpty(r.ErrorMessage);
    }

    [Theory]
    [InlineData("NL91ABNA04171643X0")]
    [InlineData("NL911BNA0417164300")]
    public void ValidateIban_WrongStructure_ReturnsInvalidStructure(string input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.InvalidStructure, r.ErrorCode);
        Assert.Equal("NL", r.CountryCode);
    }

    [Theory]
    [InlineData("NL92ABNA0417164300")]
    [InlineData("DE88370400440532013000")]
    [InlineData("NL91ABNA0417164301")]
    public void ValidateIban_WrongCheckDigits_ReturnsInvalidCheckDigits(string input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.InvalidCheckDigits, r.ErrorCode);
    }

    [Theory]
    [InlineData("XX91ABNA0417164300")]
    [InlineData("US64SVBKUS6S3300958879")]
    public void ValidateIban_UnknownCountry_ReturnsUnknownCountry(string input)
    {
        var r = TestData.Validate(input);

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.UnknownCountry, r.ErrorCode);
        Assert.Equal(string.Empty, r.CountryCode);
    }

    #endregion

    #region Country rules

    [Fact]
    public void ValidateIban_CountryInAllowedList_ReturnsValid()
    {
        var r = TestData.Validate(TestData.NlValid, TestData.L("DE", "NL"));

        Assert.True(r.IsValid);
    }

    [Fact]
    public void ValidateIban_CountryNotInAllowedList_ReturnsCountryNotAccepted()
    {
        var r = TestData.Validate(TestData.NlValid, TestData.L("DE", "FR"));

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.CountryNotAccepted, r.ErrorCode);
        Assert.Equal("NL", r.CountryCode);
        Assert.Contains("NL", r.ErrorMessage);
    }

    [Fact]
    public void ValidateIban_AllowedListNormalized_IsCaseAndWhitespaceInsensitive()
    {
        var r = TestData.Validate(TestData.NlValid, TestData.L(" nl ", "", "  "));

        Assert.True(r.IsValid);
    }

    [Fact]
    public void ValidateIban_CountryInRejectedList_ReturnsCountryNotAccepted()
    {
        var r = TestData.Validate(TestData.GbValid, rejected: TestData.L("gb"));

        Assert.False(r.IsValid);
        Assert.Equal(IbanErrorCodes.CountryNotAccepted, r.ErrorCode);
    }

    [Fact]
    public void ValidateIban_CountryNotInRejectedList_ReturnsValid()
    {
        var r = TestData.Validate(TestData.DeValid, rejected: TestData.L("GB"));

        Assert.True(r.IsValid);
    }

    [Fact]
    public void ValidateIban_AllowedAndRejectedBoth_RejectWins()
    {
        var allowed = TestData.L("NL", "DE");
        var rejected = TestData.L("NL");

        Assert.False(TestData.Validate(TestData.NlValid, allowed, rejected).IsValid);
        Assert.Equal(IbanErrorCodes.CountryNotAccepted, TestData.Validate(TestData.NlValid, allowed, rejected).ErrorCode);
        Assert.True(TestData.Validate(TestData.DeValid, allowed, rejected).IsValid);
    }

    [Fact]
    public void ValidateIban_EmptyRuleLists_AcceptAllCountries()
    {
        var r = TestData.Validate(TestData.GbValid, new List<string>(), new List<string>());

        Assert.True(r.IsValid);
    }

    [Fact]
    public void ValidateIban_InvalidIbanWithRules_ReportsStructuralErrorFirst()
    {
        var r = TestData.Validate("NL92ABNA0417164300", TestData.L("DE"));

        Assert.Equal(IbanErrorCodes.InvalidCheckDigits, r.ErrorCode);
    }

    [Fact]
    public void ValidateIban_ManyDistinctRuleSets_DoNotFail()
    {
        var countries = new List<IbanCountryInfo>();
        TestData.Sut.ListSupportedCountries(out countries);

        foreach (var c in countries.Take(150))
        {
            var r = TestData.Validate(TestData.NlValid, TestData.L(c.Code));
            Assert.Equal(c.Code == "NL", r.IsValid);
        }
    }

    #endregion
}
