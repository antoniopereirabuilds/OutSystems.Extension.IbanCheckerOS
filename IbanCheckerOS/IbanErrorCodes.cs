namespace IbanCheckerOS
{
    /// <summary>
    /// Error codes returned in the <c>ErrorCode</c> field of every result structure.
    /// An empty string means "no error".
    /// </summary>
    /// <remarks>
    /// Branch on these codes in ODC logic, never on <c>ErrorMessage</c>: messages are fixed English text
    /// and may be reworded. This class is not exposed to ODC Studio; ODC receives the values as <c>Text</c>.
    /// </remarks>
    public static class IbanErrorCodes
    {
        /// <summary>No error.</summary>
        /// <remarks>Value: empty string.</remarks>
        public const string None = "";

        /// <summary>The input was null, empty or contained only whitespace/dashes.</summary>
        /// <remarks>Returned by the validate, parse and format actions (per item in bulk calls) and by <c>GenerateTestIbans</c> when the country code is empty.</remarks>
        public const string Empty = "Empty";

        /// <summary>The IBAN length does not match the length defined for its country.</summary>
        /// <remarks>Returned by the validate, parse and format actions. <c>CountryCode</c> is still set in validation results.</remarks>
        public const string InvalidLength = "InvalidLength";

        /// <summary>The IBAN contains characters that are not allowed.</summary>
        /// <remarks>Only A-Z and 0-9 are allowed after normalization; also returned when the first two characters are not letters.</remarks>
        public const string IllegalCharacters = "IllegalCharacters";

        /// <summary>The IBAN does not match the BBAN structure defined for its country.</summary>
        /// <remarks>Also returned when the check digits (positions 3-4) are not numeric.</remarks>
        public const string InvalidStructure = "InvalidStructure";

        /// <summary>The IBAN check digits (mod-97) are incorrect.</summary>
        public const string InvalidCheckDigits = "InvalidCheckDigits";

        /// <summary>The country code is unknown or not part of the SWIFT IBAN registry.</summary>
        /// <remarks>Returned by the validate, parse and format actions and by <c>GenerateTestIbans</c>.</remarks>
        public const string UnknownCountry = "UnknownCountry";

        /// <summary>The IBAN is valid, but its country is not accepted by the allowed/rejected country rules.</summary>
        /// <remarks>Returned only by <c>ValidateIban</c> and <c>ValidateIbanBulk</c>, and only for IBANs that are otherwise valid.</remarks>
        public const string CountryNotAccepted = "CountryNotAccepted";

        /// <summary>The requested format is not one of Electronic, Print, Obfuscated or Hidden.</summary>
        /// <remarks>Returned by <c>FormatIban</c> (in the result) and <c>FormatIbanBulk</c> (call-level error).</remarks>
        public const string InvalidFormat = "InvalidFormat";

        /// <summary>The requested count is outside the allowed range.</summary>
        /// <remarks>Returned by <c>GenerateTestIbans</c> when <c>Count</c> is outside 1..1000.</remarks>
        public const string InvalidCount = "InvalidCount";

        /// <summary>A bulk call received more items than the per-call cap.</summary>
        /// <remarks>Call-level error of the bulk actions when the input has more than 5,000 items. No items are processed.</remarks>
        public const string TooManyItems = "TooManyItems";

        /// <summary>An unexpected error occurred while validating, parsing, formatting or generating.</summary>
        /// <remarks>Should not happen in normal operation. <c>ErrorMessage</c> describes the cause.</remarks>
        public const string UnexpectedError = "UnexpectedError";
    }
}
