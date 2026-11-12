using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Result of validating a single IBAN.
    /// </summary>
    [OSStructure(Description = "Result of validating a single IBAN against the SWIFT IBAN registry.")]
    public struct IbanValidationResult
    {
        /// <summary>True when the IBAN is valid and its country passes the allowed/rejected country rules.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and its country passes the allowed/rejected country rules.")]
        public bool IsValid;

        /// <summary>Error code; empty when valid.</summary>
        [OSStructureField(Description = "Error code; empty when valid. One of: Empty, InvalidLength, IllegalCharacters, InvalidStructure, InvalidCheckDigits, UnknownCountry, CountryNotAccepted, UnexpectedError.")]
        public string ErrorCode;

        /// <summary>Human-readable error message; empty when valid.</summary>
        [OSStructureField(Description = "Human-readable error message; empty when valid.")]
        public string ErrorMessage;

        /// <summary>ISO 3166-1 alpha-2 country code of the IBAN when it could be determined.</summary>
        [OSStructureField(Description = "ISO 3166-1 alpha-2 country code of the IBAN when it could be determined (also set for some invalid IBANs, e.g. wrong length); otherwise empty.")]
        public string CountryCode;

        /// <summary>The input after trimming, removing whitespace and dashes, and uppercasing.</summary>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>Creates an empty (invalid) result.</summary>
        public IbanValidationResult()
        {
            IsValid = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            CountryCode = string.Empty;
            NormalizedIban = string.Empty;
        }
    }

    /// <summary>
    /// Validation result for one item of a bulk validation call.
    /// </summary>
    [OSStructure(Description = "Validation result for one item of a bulk validation call.")]
    public struct IbanBulkValidationItem
    {
        /// <summary>Zero-based position of the item in the input list.</summary>
        [OSStructureField(Description = "Zero-based position of the item in the input list.")]
        public int Index;

        /// <summary>The original input value, as received.</summary>
        [OSStructureField(Description = "The original input value, as received.")]
        public string Input;

        /// <summary>The input after normalization.</summary>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>True when the IBAN is valid.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and its country passes the allowed/rejected country rules.")]
        public bool IsValid;

        /// <summary>Error code; empty when valid.</summary>
        [OSStructureField(Description = "Error code; empty when valid. Same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Human-readable error message; empty when valid.</summary>
        [OSStructureField(Description = "Human-readable error message; empty when valid.")]
        public string ErrorMessage;

        /// <summary>Country code of the IBAN when it could be determined.</summary>
        [OSStructureField(Description = "ISO 3166-1 alpha-2 country code of the IBAN when it could be determined; otherwise empty.")]
        public string CountryCode;

        /// <summary>Creates an empty item.</summary>
        public IbanBulkValidationItem()
        {
            Index = 0;
            Input = string.Empty;
            NormalizedIban = string.Empty;
            IsValid = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            CountryCode = string.Empty;
        }
    }

    /// <summary>
    /// Result of a bulk validation call.
    /// </summary>
    [OSStructure(Description = "Result of a bulk IBAN validation call.")]
    public struct IbanBulkValidationResult
    {
        /// <summary>False only when the whole call was rejected (e.g. TooManyItems).</summary>
        [OSStructureField(Description = "False only when the whole call was rejected (e.g. TooManyItems). Individual invalid IBANs do not set this to False.")]
        public bool Success;

        /// <summary>Call-level error code; empty on success.</summary>
        [OSStructureField(Description = "Call-level error code; empty on success. TooManyItems when the input exceeds the per-call cap.")]
        public string ErrorCode;

        /// <summary>Call-level error message; empty on success.</summary>
        [OSStructureField(Description = "Call-level error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>Number of items received.</summary>
        [OSStructureField(Description = "Number of items received in the input list.")]
        public int TotalCount;

        /// <summary>Number of valid IBANs.</summary>
        [OSStructureField(Description = "Number of valid IBANs (always computed over the full input, even when OnlyInvalid is True).")]
        public int SuccessCount;

        /// <summary>Number of invalid IBANs.</summary>
        [OSStructureField(Description = "Number of invalid IBANs (always computed over the full input, even when OnlyInvalid is True).")]
        public int FailureCount;

        /// <summary>Per-item results in input order.</summary>
        [OSStructureField(Description = "Per-item results in input order. Contains only invalid items when OnlyInvalid is True.")]
        public List<IbanBulkValidationItem> Results;

        /// <summary>Creates an empty successful result.</summary>
        public IbanBulkValidationResult()
        {
            Success = true;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            TotalCount = 0;
            SuccessCount = 0;
            FailureCount = 0;
            Results = new List<IbanBulkValidationItem>();
        }
    }
}
