namespace IbanCheckerOS.UnitTests;

/// <summary>
/// Shared IBAN samples (SWIFT registry examples) and helpers used across the test classes.
/// </summary>
internal static class TestData
{
    public const string NlValid = "NL91ABNA0417164300";
    public const string NlPrint = "NL91 ABNA 0417 1643 00";
    public const string DeValid = "DE89370400440532013000";
    public const string GbValid = "GB29NWBK60161331926819";
    public const string FrValid = "FR1420041010050500013M02606";
    public const string PtValid = "PT50000201231234567890154";
    public const string BeValid = "BE68539007547034";

    public static readonly IbanNetService Sut = new();

    public static IbanValidationResult Validate(string? iban, List<string>? allowed = null, List<string>? rejected = null)
    {
        Sut.ValidateIban(iban!, allowed!, rejected!, out var result);
        return result;
    }

    public static List<string> L(params string[] values) => values.ToList();
}
