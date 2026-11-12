namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Tests for <see cref="IbanNetService.ValidateIbanBulk"/>: ordering, counts, OnlyInvalid, rules and the item cap.
/// </summary>
public class ValidateIbanBulkTests
{
    private static IbanBulkValidationResult Bulk(List<string>? ibans, bool onlyInvalid = false, List<string>? allowed = null, List<string>? rejected = null)
    {
        TestData.Sut.ValidateIbanBulk(ibans!, allowed!, rejected!, onlyInvalid, out var result);
        return result;
    }

    [Fact]
    public void ValidateIbanBulk_MixedInput_PreservesOrderAndCounts()
    {
        var input = TestData.L(TestData.NlValid, "NL92ABNA0417164300", " de89 3704 0044 0532 0130 00 ", "", "XX00");

        var r = Bulk(input);

        Assert.True(r.Success);
        Assert.Equal(string.Empty, r.ErrorCode);
        Assert.Equal(5, r.TotalCount);
        Assert.Equal(2, r.SuccessCount);
        Assert.Equal(3, r.FailureCount);
        Assert.Equal(5, r.Results.Count);
        Assert.Equal(new[] { 0, 1, 2, 3, 4 }, r.Results.Select(x => x.Index));
        Assert.Equal(input, r.Results.Select(x => x.Input));
        Assert.Equal(TestData.DeValid, r.Results[2].NormalizedIban);
        Assert.True(r.Results[2].IsValid);
        Assert.Equal(IbanErrorCodes.InvalidCheckDigits, r.Results[1].ErrorCode);
        Assert.Equal(IbanErrorCodes.Empty, r.Results[3].ErrorCode);
    }

    [Fact]
    public void ValidateIbanBulk_OnlyInvalid_ReturnsOnlyInvalidButFullCounts()
    {
        var input = TestData.L(TestData.NlValid, "bad", TestData.GbValid, "NL92ABNA0417164300");

        var r = Bulk(input, onlyInvalid: true);

        Assert.Equal(4, r.TotalCount);
        Assert.Equal(2, r.SuccessCount);
        Assert.Equal(2, r.FailureCount);
        Assert.Equal(new[] { 1, 3 }, r.Results.Select(x => x.Index));
        Assert.All(r.Results, x => Assert.False(x.IsValid));
    }

    [Fact]
    public void ValidateIbanBulk_NullItem_IsHandledAsEmpty()
    {
        var input = new List<string> { TestData.NlValid, null!, TestData.DeValid };

        var r = Bulk(input);

        Assert.Equal(3, r.TotalCount);
        Assert.Equal(2, r.SuccessCount);
        Assert.Equal(IbanErrorCodes.Empty, r.Results[1].ErrorCode);
        Assert.Equal(string.Empty, r.Results[1].Input);
    }

    [Fact]
    public void ValidateIbanBulk_NullOrEmptyList_ReturnsEmptySuccess()
    {
        foreach (var input in new List<string>?[] { null, new() })
        {
            var r = Bulk(input);
            Assert.True(r.Success);
            Assert.Equal(0, r.TotalCount);
            Assert.Empty(r.Results);
        }
    }

    [Fact]
    public void ValidateIbanBulk_WithCountryRules_AppliesRulesPerItem()
    {
        var r = Bulk(TestData.L(TestData.NlValid, TestData.DeValid, TestData.GbValid), allowed: TestData.L("NL", "GB"), rejected: TestData.L("GB"));

        Assert.True(r.Results[0].IsValid);
        Assert.Equal(IbanErrorCodes.CountryNotAccepted, r.Results[1].ErrorCode);
        Assert.Equal(IbanErrorCodes.CountryNotAccepted, r.Results[2].ErrorCode);
        Assert.Equal("DE", r.Results[1].CountryCode);
    }

    [Fact]
    public void ValidateIbanBulk_AtCap_IsProcessed()
    {
        var input = Enumerable.Repeat(TestData.NlValid, IbanNetService.MaxBulkItems).ToList();

        var r = Bulk(input);

        Assert.True(r.Success);
        Assert.Equal(IbanNetService.MaxBulkItems, r.SuccessCount);
        Assert.Equal(IbanNetService.MaxBulkItems, r.Results.Count);
    }

    [Fact]
    public void ValidateIbanBulk_OverCap_ReturnsTooManyItems()
    {
        var input = Enumerable.Repeat(TestData.NlValid, IbanNetService.MaxBulkItems + 1).ToList();

        var r = Bulk(input);

        Assert.False(r.Success);
        Assert.Equal(IbanErrorCodes.TooManyItems, r.ErrorCode);
        Assert.NotEmpty(r.ErrorMessage);
        Assert.Equal(IbanNetService.MaxBulkItems + 1, r.TotalCount);
        Assert.Equal(0, r.SuccessCount);
        Assert.Empty(r.Results);
    }
}
