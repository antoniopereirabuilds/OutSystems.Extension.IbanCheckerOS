using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Result of formatting a single IBAN.
    /// </summary>
    /// <remarks>Output of <c>FormatIban</c>. Exposed to ODC as the <c>IbanFormatResult</c> structure.</remarks>
    [OSStructure(Description = "Result of formatting a single IBAN.")]
    public struct IbanFormatResult
    {
        /// <summary>True when the IBAN is valid and was formatted.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True when the IBAN is valid and was formatted.")]
        public bool Success;

        /// <summary>Error code; empty on success. <c>InvalidFormat</c> for an unknown format (checked first), otherwise the <c>ValidateIban</c> codes except <c>CountryNotAccepted</c>.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Error code; empty on success. InvalidFormat for an unknown format, otherwise the same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The input after trimming, removing whitespace and dashes, and uppercasing.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>The formatted IBAN; empty on failure.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "The formatted IBAN; empty on failure.")]
        public string Value;

        /// <summary>Creates an empty (failed) result.</summary>
        public IbanFormatResult()
        {
            Success = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            NormalizedIban = string.Empty;
            Value = string.Empty;
        }
    }

    /// <summary>
    /// Format result for one item of a bulk format call.
    /// </summary>
    /// <remarks>One element of <see cref="IbanBulkFormatResult.Results"/> (<c>FormatIbanBulk</c>). Exposed to ODC as the <c>IbanBulkFormatItem</c> structure.</remarks>
    [OSStructure(Description = "Format result for one item of a bulk format call.")]
    public struct IbanBulkFormatItem
    {
        /// <summary>Zero-based position of the item in the input list.</summary>
        /// <remarks>Maps to ODC type <c>Integer</c>.</remarks>
        [OSStructureField(Description = "Zero-based position of the item in the input list.")]
        public int Index;

        /// <summary>The original input value, as received (a null item becomes an empty string).</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "The original input value, as received.")]
        public string Input;

        /// <summary>The input after trimming, removing whitespace and dashes, and uppercasing.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>True when the IBAN was formatted.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True when the IBAN is valid and was formatted.")]
        public bool Success;

        /// <summary>Error code; empty on success. The <c>ValidateIban</c> codes except <c>CountryNotAccepted</c>; an invalid format is reported at call level instead.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Error code; empty on success. Same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The formatted IBAN; empty on failure.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "The formatted IBAN; empty on failure.")]
        public string Value;

        /// <summary>Creates an empty item.</summary>
        public IbanBulkFormatItem()
        {
            Index = 0;
            Input = string.Empty;
            NormalizedIban = string.Empty;
            Success = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            Value = string.Empty;
        }
    }

    /// <summary>
    /// Result of a bulk format call.
    /// </summary>
    /// <remarks>Output of <c>FormatIbanBulk</c>. Exposed to ODC as the <c>IbanBulkFormatResult</c> structure.</remarks>
    [OSStructure(Description = "Result of a bulk IBAN format call.")]
    public struct IbanBulkFormatResult
    {
        /// <summary>False only when the whole call was rejected (<c>TooManyItems</c> or <c>InvalidFormat</c>); individual invalid IBANs do not set it to False.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "False only when the whole call was rejected (TooManyItems or InvalidFormat). Individual invalid IBANs do not set this to False.")]
        public bool Success;

        /// <summary>Call-level error code.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Call-level error code; empty on success. TooManyItems or InvalidFormat.")]
        public string ErrorCode;

        /// <summary>Call-level error message.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Call-level error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>Number of items received in the input list (set even when the call is rejected).</summary>
        /// <remarks>Maps to ODC type <c>Integer</c>.</remarks>
        [OSStructureField(Description = "Number of items received in the input list.")]
        public int TotalCount;

        /// <summary>Number of successfully formatted IBANs.</summary>
        /// <remarks>Maps to ODC type <c>Integer</c>.</remarks>
        [OSStructureField(Description = "Number of successfully formatted IBANs.")]
        public int SuccessCount;

        /// <summary>Number of IBANs that failed to format.</summary>
        /// <remarks>Maps to ODC type <c>Integer</c>.</remarks>
        [OSStructureField(Description = "Number of IBANs that failed to format.")]
        public int FailureCount;

        /// <summary>Per-item results in input order.</summary>
        /// <remarks>Maps to ODC type <c>IbanBulkFormatItem List</c>.</remarks>
        [OSStructureField(Description = "Per-item results in input order.")]
        public List<IbanBulkFormatItem> Results;

        /// <summary>Creates an empty successful result.</summary>
        public IbanBulkFormatResult()
        {
            Success = true;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            TotalCount = 0;
            SuccessCount = 0;
            FailureCount = 0;
            Results = new List<IbanBulkFormatItem>();
        }
    }
}
