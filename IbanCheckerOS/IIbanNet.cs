using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Validates, parses, formats and generates IBANs using the IbanNet library and the SWIFT IBAN registry.
    /// </summary>
    [OSInterface(
        Name = "IbanCheckerOS",
        Description = "Validate, parse, format and generate IBANs (International Bank Account Numbers) using IbanNet and the official SWIFT IBAN registry. Inputs are normalized (trimmed, whitespace and dashes removed, uppercased). Invalid IBANs never throw: every action returns a result structure with an ErrorCode and ErrorMessage.",
        IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
    )]
    public interface IIbanNet
    {
        /// <summary>
        /// Validates a single IBAN, optionally restricting the accepted countries.
        /// </summary>
        /// <param name="Iban">The IBAN to validate. Spaces, dashes and lowercase are accepted.</param>
        /// <param name="AllowedCountries">Optional list of accepted ISO country codes. Empty means all countries.</param>
        /// <param name="RejectedCountries">Optional list of rejected ISO country codes.</param>
        /// <param name="Result">The validation result.</param>
        [OSAction(
            Description = "Validates an IBAN against the SWIFT IBAN registry (length, characters, BBAN structure, mod-97 check digits) and optional allowed/rejected country lists. Never throws for invalid input.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ValidateIban(
            [OSParameterAttribute(Description = "The IBAN to validate. Surrounding/inner whitespace, dashes and lowercase letters are accepted and normalized.")]
            string Iban,

            [OSParameterAttribute(Description = "Optional. ISO 3166-1 alpha-2 country codes to accept (e.g. NL, DE). Empty list accepts all registry countries.")]
            List<string> AllowedCountries,

            [OSParameterAttribute(Description = "Optional. ISO 3166-1 alpha-2 country codes to reject (e.g. GB). Applied in addition to AllowedCountries.")]
            List<string> RejectedCountries,

            [OSParameterAttribute(Description = "The validation result: IsValid, ErrorCode, ErrorMessage, CountryCode and NormalizedIban.")]
            out IbanValidationResult Result
        );

        /// <summary>
        /// Validates a list of IBANs in one call (max 5,000 items).
        /// </summary>
        /// <param name="Ibans">The IBANs to validate.</param>
        /// <param name="AllowedCountries">Optional list of accepted ISO country codes.</param>
        /// <param name="RejectedCountries">Optional list of rejected ISO country codes.</param>
        /// <param name="OnlyInvalid">When true, only invalid items are returned in Results.</param>
        /// <param name="Result">The bulk validation result.</param>
        [OSAction(
            Description = "Validates a list of IBANs (max 5,000 per call). Results keep the input order; one bad item never fails the batch. Counts always reflect the full input.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ValidateIbanBulk(
            [OSParameterAttribute(Description = "The IBANs to validate (max 5,000). Each item is normalized.")]
            List<string> Ibans,

            [OSParameterAttribute(Description = "Optional. ISO 3166-1 alpha-2 country codes to accept. Empty list accepts all registry countries.")]
            List<string> AllowedCountries,

            [OSParameterAttribute(Description = "Optional. ISO 3166-1 alpha-2 country codes to reject.")]
            List<string> RejectedCountries,

            [OSParameterAttribute(Description = "When True, Results contains only the invalid items (counts still reflect the full input).")]
            bool OnlyInvalid,

            [OSParameterAttribute(Description = "The bulk validation result with counts and per-item results.")]
            out IbanBulkValidationResult Result
        );

        /// <summary>
        /// Parses a single IBAN into its components.
        /// </summary>
        /// <param name="Iban">The IBAN to parse.</param>
        /// <param name="Result">The parse result.</param>
        [OSAction(
            Description = "Parses an IBAN and returns its components: formats, country, check digits, BBAN, bank and branch identifiers, SEPA membership. Never throws for invalid input.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ParseIban(
            [OSParameterAttribute(Description = "The IBAN to parse. Whitespace, dashes and lowercase letters are accepted and normalized.")]
            string Iban,

            [OSParameterAttribute(Description = "The parse result: Success, ErrorCode, ErrorMessage, NormalizedIban and Info.")]
            out IbanParseResult Result
        );

        /// <summary>
        /// Parses a list of IBANs in one call (max 5,000 items).
        /// </summary>
        /// <param name="Ibans">The IBANs to parse.</param>
        /// <param name="Result">The bulk parse result.</param>
        [OSAction(
            Description = "Parses a list of IBANs (max 5,000 per call). Results keep the input order; one bad item never fails the batch.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ParseIbanBulk(
            [OSParameterAttribute(Description = "The IBANs to parse (max 5,000). Each item is normalized.")]
            List<string> Ibans,

            [OSParameterAttribute(Description = "The bulk parse result with counts and per-item results.")]
            out IbanBulkParseResult Result
        );

        /// <summary>
        /// Formats a single IBAN.
        /// </summary>
        /// <param name="Iban">The IBAN to format.</param>
        /// <param name="Format">Electronic, Print, Obfuscated or Hidden (case-insensitive, default Electronic).</param>
        /// <param name="Result">The format result.</param>
        [OSAction(
            Description = "Formats a valid IBAN as Electronic (NL91ABNA0417164300), Print (NL91 ABNA 0417 1643 00), Obfuscated (XXXXXXXXXXXXXX4300) or Hidden (****).",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void FormatIban(
            [OSParameterAttribute(Description = "The IBAN to format. Whitespace, dashes and lowercase letters are accepted and normalized.")]
            string Iban,

            [OSParameterAttribute(Description = "Electronic, Print, Obfuscated or Hidden (case-insensitive). Empty defaults to Electronic.")]
            string Format,

            [OSParameterAttribute(Description = "The format result: Success, ErrorCode, ErrorMessage, NormalizedIban and Value.")]
            out IbanFormatResult Result
        );

        /// <summary>
        /// Formats a list of IBANs in one call (max 5,000 items).
        /// </summary>
        /// <param name="Ibans">The IBANs to format.</param>
        /// <param name="Format">Electronic, Print, Obfuscated or Hidden (case-insensitive, default Electronic).</param>
        /// <param name="Result">The bulk format result.</param>
        [OSAction(
            Description = "Formats a list of IBANs (max 5,000 per call) using one format. Results keep the input order; one bad item never fails the batch.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void FormatIbanBulk(
            [OSParameterAttribute(Description = "The IBANs to format (max 5,000). Each item is normalized.")]
            List<string> Ibans,

            [OSParameterAttribute(Description = "Electronic, Print, Obfuscated or Hidden (case-insensitive). Empty defaults to Electronic.")]
            string Format,

            [OSParameterAttribute(Description = "The bulk format result with counts and per-item results.")]
            out IbanBulkFormatResult Result
        );

        /// <summary>
        /// Gets the IBAN definition of a country.
        /// </summary>
        /// <param name="CountryCode">ISO 3166-1 alpha-2 country code.</param>
        /// <param name="Result">The country lookup result.</param>
        [OSAction(
            Description = "Returns the SWIFT IBAN registry definition of a country (length, examples, SEPA membership, bank/branch identifiers, effective date).",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void GetCountryInfo(
            [OSParameterAttribute(Description = "ISO 3166-1 alpha-2 country code (case-insensitive), e.g. NL.")]
            string CountryCode,

            [OSParameterAttribute(Description = "The lookup result: Found and Country.")]
            out IbanCountryResult Result
        );

        /// <summary>
        /// Lists every country in the SWIFT IBAN registry, ordered by code.
        /// </summary>
        /// <param name="Countries">The supported countries.</param>
        [OSAction(
            Description = "Lists all countries supported by the SWIFT IBAN registry, ordered by country code.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ListSupportedCountries(
            [OSParameterAttribute(Description = "The supported countries, ordered by country code.")]
            out List<IbanCountryInfo> Countries
        );

        /// <summary>
        /// Generates random IBANs for testing only.
        /// </summary>
        /// <param name="CountryCode">ISO 3166-1 alpha-2 country code.</param>
        /// <param name="Count">Number of IBANs to generate (1 to 1000).</param>
        /// <param name="Result">The generation result.</param>
        [OSAction(
            Description = "For testing only: generates random IBANs that pass validation for the given country. They are NOT real bank accounts and must never be used for payments. Count must be between 1 and 1000.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void GenerateTestIbans(
            [OSParameterAttribute(Description = "ISO 3166-1 alpha-2 country code (case-insensitive), e.g. DE.")]
            string CountryCode,

            [OSParameterAttribute(Description = "Number of IBANs to generate (1 to 1000).")]
            int Count,

            [OSParameterAttribute(Description = "The generation result: Success, ErrorCode, ErrorMessage and Ibans (for testing only).")]
            out IbanGenerateResult Result
        );
    }
}
