using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Details extracted from a valid IBAN.
    /// </summary>
    [OSStructure(Description = "Details extracted from a valid IBAN.")]
    public struct IbanInfo
    {
        /// <summary>Electronic format, e.g. NL91ABNA0417164300.</summary>
        [OSStructureField(Description = "Electronic format (no spaces), e.g. NL91ABNA0417164300.")]
        public string Electronic;

        /// <summary>Print format, e.g. NL91 ABNA 0417 1643 00.</summary>
        [OSStructureField(Description = "Print format (groups of 4), e.g. NL91 ABNA 0417 1643 00.")]
        public string Print;

        /// <summary>Obfuscated format, e.g. XXXXXXXXXXXXXX4300.</summary>
        [OSStructureField(Description = "Obfuscated format showing only the last 4 characters, e.g. XXXXXXXXXXXXXX4300.")]
        public string Obfuscated;

        /// <summary>ISO 3166-1 alpha-2 country code.</summary>
        [OSStructureField(Description = "ISO 3166-1 alpha-2 country code, e.g. NL.")]
        public string CountryCode;

        /// <summary>English country name from the SWIFT registry.</summary>
        [OSStructureField(Description = "English country name as listed in the SWIFT IBAN registry.")]
        public string CountryName;

        /// <summary>The two IBAN check digits.</summary>
        [OSStructureField(Description = "The two IBAN check digits (positions 3-4).")]
        public string CheckDigits;

        /// <summary>The Basic Bank Account Number.</summary>
        [OSStructureField(Description = "The BBAN (Basic Bank Account Number) part of the IBAN.")]
        public string Bban;

        /// <summary>The bank identifier, or empty if the country defines none.</summary>
        [OSStructureField(Description = "The bank identifier, or empty if the country does not define one.")]
        public string BankIdentifier;

        /// <summary>The branch identifier, or empty if the country defines none.</summary>
        [OSStructureField(Description = "The branch identifier, or empty if the country does not define one.")]
        public string BranchIdentifier;

        /// <summary>True if the country is a SEPA member.</summary>
        [OSStructureField(Description = "True if the IBAN country is a SEPA member.")]
        public bool IsSepaMember;

        /// <summary>Creates an empty info.</summary>
        public IbanInfo()
        {
            Electronic = string.Empty;
            Print = string.Empty;
            Obfuscated = string.Empty;
            CountryCode = string.Empty;
            CountryName = string.Empty;
            CheckDigits = string.Empty;
            Bban = string.Empty;
            BankIdentifier = string.Empty;
            BranchIdentifier = string.Empty;
            IsSepaMember = false;
        }
    }

    /// <summary>
    /// Result of parsing a single IBAN.
    /// </summary>
    [OSStructure(Description = "Result of parsing a single IBAN.")]
    public struct IbanParseResult
    {
        /// <summary>True when the IBAN is valid and was parsed.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and was parsed.")]
        public bool Success;

        /// <summary>Error code; empty on success.</summary>
        [OSStructureField(Description = "Error code; empty on success. Same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The input after normalization.</summary>
        [OSStructureField(Description = "The input after trimming, removing whitespace and dashes, and uppercasing.")]
        public string NormalizedIban;

        /// <summary>Parsed IBAN details; empty when Success is False.</summary>
        [OSStructureField(Description = "Parsed IBAN details; empty when Success is False.")]
        public IbanInfo Info;

        /// <summary>Creates an empty (failed) result.</summary>
        public IbanParseResult()
        {
            Success = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            NormalizedIban = string.Empty;
            Info = new IbanInfo();
        }
    }

    /// <summary>
    /// Parse result for one item of a bulk parse call.
    /// </summary>
    [OSStructure(Description = "Parse result for one item of a bulk parse call.")]
    public struct IbanBulkParseItem
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

        /// <summary>True when the IBAN was parsed.</summary>
        [OSStructureField(Description = "True when the IBAN is valid and was parsed.")]
        public bool Success;

        /// <summary>Error code; empty on success.</summary>
        [OSStructureField(Description = "Error code; empty on success. Same codes as ValidateIban.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>Parsed IBAN details.</summary>
        [OSStructureField(Description = "Parsed IBAN details; empty when Success is False.")]
        public IbanInfo Info;

        /// <summary>Creates an empty item.</summary>
        public IbanBulkParseItem()
        {
            Index = 0;
            Input = string.Empty;
            NormalizedIban = string.Empty;
            Success = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            Info = new IbanInfo();
        }
    }

    /// <summary>
    /// Result of a bulk parse call.
    /// </summary>
    [OSStructure(Description = "Result of a bulk IBAN parse call.")]
    public struct IbanBulkParseResult
    {
        /// <summary>False only when the whole call was rejected.</summary>
        [OSStructureField(Description = "False only when the whole call was rejected (e.g. TooManyItems). Individual invalid IBANs do not set this to False.")]
        public bool Success;

        /// <summary>Call-level error code.</summary>
        [OSStructureField(Description = "Call-level error code; empty on success. TooManyItems when the input exceeds the per-call cap.")]
        public string ErrorCode;

        /// <summary>Call-level error message.</summary>
        [OSStructureField(Description = "Call-level error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>Number of items received.</summary>
        [OSStructureField(Description = "Number of items received in the input list.")]
        public int TotalCount;

        /// <summary>Number of successfully parsed IBANs.</summary>
        [OSStructureField(Description = "Number of successfully parsed IBANs.")]
        public int SuccessCount;

        /// <summary>Number of IBANs that failed to parse.</summary>
        [OSStructureField(Description = "Number of IBANs that failed to parse.")]
        public int FailureCount;

        /// <summary>Per-item results in input order.</summary>
        [OSStructureField(Description = "Per-item results in input order.")]
        public List<IbanBulkParseItem> Results;

        /// <summary>Creates an empty successful result.</summary>
        public IbanBulkParseResult()
        {
            Success = true;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            TotalCount = 0;
            SuccessCount = 0;
            FailureCount = 0;
            Results = new List<IbanBulkParseItem>();
        }
    }
}
