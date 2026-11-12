using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Result of formatting a single IBAN.
    /// </summary>
    [OSStructure(Description = "Result of formatting a single IBAN.")]
    public struct IbanFormatResult
    {
        /// <summary>True when the IBAN is valid and was formatted.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and was formatted.")]
        public bool Success;

        /// <summary>Error code; empty on success.</summary>
        [OSStructureField(Description = "Error code; empty on success. InvalidFormat for an unknown format, otherwise the same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The input after normalization.</summary>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>The formatted IBAN; empty on failure.</summary>
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
    [OSStructure(Description = "Format result for one item of a bulk format call.")]
    public struct IbanBulkFormatItem
    {
        /// <summary>Zero-based position of the item in the input list.</summary>
        [OSStructureField(Description = "Zero-based position of the item in the input list.")]
        public int Index;

        /// <summary>The original input value.</summary>
        [OSStructureField(Description = "The original input value, as received.")]
        public string Input;

        /// <summary>The input after normalization.</summary>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>True when the IBAN was formatted.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and was formatted.")]
        public bool Success;

        /// <summary>Error code; empty on success.</summary>
        [OSStructureField(Description = "Error code; empty on success. Same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The formatted IBAN; empty on failure.</summary>
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
    [OSStructure(Description = "Result of a bulk IBAN format call.")]
    public struct IbanBulkFormatResult
    {
        /// <summary>False only when the whole call was rejected.</summary>
        [OSStructureField(Description = "False only when the whole call was rejected (TooManyItems or InvalidFormat). Individual invalid IBANs do not set this to False.")]
        public bool Success;

        /// <summary>Call-level error code.</summary>
        [OSStructureField(Description = "Call-level error code; empty on success. TooManyItems or InvalidFormat.")]
        public string ErrorCode;

        /// <summary>Call-level error message.</summary>
        [OSStructureField(Description = "Call-level error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>Number of items received.</summary>
        [OSStructureField(Description = "Number of items received in the input list.")]
        public int TotalCount;

        /// <summary>Number of successfully formatted IBANs.</summary>
        [OSStructureField(Description = "Number of successfully formatted IBANs.")]
        public int SuccessCount;

        /// <summary>Number of IBANs that failed to format.</summary>
        [OSStructureField(Description = "Number of IBANs that failed to format.")]
        public int FailureCount;

        /// <summary>Per-item results in input order.</summary>
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
