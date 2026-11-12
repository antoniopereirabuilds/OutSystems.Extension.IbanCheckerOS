namespace IbanCheckerOS
{
    /// <summary>
    /// Error codes returned in the <c>ErrorCode</c> field of every result structure.
    /// An empty string means "no error".
    /// </summary>
    public static class IbanErrorCodes
    {
        /// <summary>No error.</summary>
        public const string None = "";

        /// <summary>The input was null, empty or contained only whitespace/dashes.</summary>
        public const string Empty = "Empty";

        /// <summary>The IBAN length does not match the length defined for its country.</summary>
        public const string InvalidLength = "InvalidLength";

        /// <summary>The IBAN contains characters that are not allowed.</summary>
        public const string IllegalCharacters = "IllegalCharacters";

        /// <summary>The IBAN does not match the BBAN structure defined for its country.</summary>
        public const string InvalidStructure = "InvalidStructure";

        /// <summary>The IBAN check digits (mod-97) are incorrect.</summary>
        public const string InvalidCheckDigits = "InvalidCheckDigits";

        /// <summary>The country code is unknown or not part of the SWIFT IBAN registry.</summary>
        public const string UnknownCountry = "UnknownCountry";

        /// <summary>The IBAN is valid, but its country is not accepted by the allowed/rejected country rules.</summary>
        public const string CountryNotAccepted = "CountryNotAccepted";

        /// <summary>The requested format is not one of Electronic, Print, Obfuscated or Hidden.</summary>
        public const string InvalidFormat = "InvalidFormat";

        /// <summary>The requested count is outside the allowed range.</summary>
        public const string InvalidCount = "InvalidCount";

        /// <summary>A bulk call received more items than the per-call cap.</summary>
        public const string TooManyItems = "TooManyItems";

        /// <summary>An unexpected error occurred while validating, parsing, formatting or generating.</summary>
        public const string UnexpectedError = "UnexpectedError";
    }
}
