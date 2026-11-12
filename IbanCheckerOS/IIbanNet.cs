using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Validates, parses, formats and generates IBANs using the IbanNet library and the SWIFT IBAN registry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Runs as an AWS Lambda function (linux-x64, framework-dependent .NET 10). Each call is independent:
    /// do not rely on in-memory state between invocations. The implementation caches read-only registry
    /// data in static fields, which only affects warm-start latency, never results.
    /// </para>
    /// <para>
    /// Exposed to ODC Studio as the <c>IbanCheckerOS</c> external library via
    /// <c>OutSystems.ExternalLibraries.SDK</c> attributes. Every IBAN input is normalized first: trimmed,
    /// all whitespace and dash characters removed, and uppercased (invariant culture).
    /// </para>
    /// <para>
    /// No action throws for bad input. The IBAN actions (validate, parse, format, generate) report
    /// problems through the <c>ErrorCode</c> (see <see cref="IbanErrorCodes"/>) and <c>ErrorMessage</c>
    /// fields of their result structure. <see cref="GetCountryInfo"/> reports an unknown country through
    /// <see cref="IbanCountryResult.Found"/>, and <see cref="ListSupportedCountries"/> has no failure mode.
    /// </para>
    /// </remarks>
    [OSInterface(
        Name = "IbanCheckerOS",
        Description = "Validate, parse, format and generate IBANs (International Bank Account Numbers) using IbanNet and the official SWIFT IBAN registry. Inputs are normalized (trimmed, whitespace and dashes removed, uppercased). Invalid input never throws: the IBAN actions return a result structure with an ErrorCode and ErrorMessage.",
        IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
    )]
    public interface IIbanNet
    {
        /// <summary>
        /// Validates a single IBAN, optionally restricting the accepted countries.
        /// </summary>
        /// <param name="Iban">The IBAN to validate. Spaces, dashes and lowercase are accepted and normalized. Null or empty returns <see cref="IbanErrorCodes.Empty"/>.</param>
        /// <param name="AllowedCountries">Optional list of accepted ISO 3166-1 alpha-2 country codes (case-insensitive, trimmed). Null or empty accepts all registry countries.</param>
        /// <param name="RejectedCountries">Optional list of rejected ISO 3166-1 alpha-2 country codes. Applied on top of <paramref name="AllowedCountries"/>, so a rejection always wins.</param>
        /// <param name="Result">The validation result: <c>IsValid</c>, <c>ErrorCode</c>, <c>ErrorMessage</c>, <c>CountryCode</c> and <c>NormalizedIban</c>.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>ValidateIban</c> in the <c>IbanCheckerOS</c> external library.
        /// Checks length, characters, BBAN structure and mod-97 check digits against the SWIFT IBAN registry.
        /// <see cref="IbanErrorCodes.CountryNotAccepted"/> is reported only for IBANs that are otherwise valid.
        /// Possible error codes: <c>Empty</c>, <c>InvalidLength</c>, <c>IllegalCharacters</c>, <c>InvalidStructure</c>,
        /// <c>InvalidCheckDigits</c>, <c>UnknownCountry</c>, <c>CountryNotAccepted</c>, <c>UnexpectedError</c>.
        /// </remarks>
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
        /// <param name="Ibans">The IBANs to validate (max <see cref="IbanNetService.MaxBulkItems"/>). Each item is normalized; a null item is treated as empty.</param>
        /// <param name="AllowedCountries">Optional list of accepted ISO 3166-1 alpha-2 country codes. Same semantics as in <see cref="ValidateIban"/>.</param>
        /// <param name="RejectedCountries">Optional list of rejected ISO 3166-1 alpha-2 country codes. Same semantics as in <see cref="ValidateIban"/>.</param>
        /// <param name="OnlyInvalid">When true, only invalid items are returned in <c>Results</c>. <c>TotalCount</c>, <c>SuccessCount</c> and <c>FailureCount</c> always cover the full input.</param>
        /// <param name="Result">The bulk validation result with call-level status, counts and per-item results in input order.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>ValidateIbanBulk</c> in the <c>IbanCheckerOS</c> external library.
        /// One bad item never fails the batch. The only call-level error is <see cref="IbanErrorCodes.TooManyItems"/>
        /// (more than 5,000 items), which sets <c>Success</c> to False and returns no items.
        /// </remarks>
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
        /// <param name="Iban">The IBAN to parse. Whitespace, dashes and lowercase are accepted and normalized.</param>
        /// <param name="Result">The parse result: <c>Success</c>, <c>ErrorCode</c>, <c>ErrorMessage</c>, <c>NormalizedIban</c> and <c>Info</c>.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>ParseIban</c> in the <c>IbanCheckerOS</c> external library.
        /// The IBAN is validated first without country rules, so the error codes are those of
        /// <see cref="ValidateIban"/> except <c>CountryNotAccepted</c>. <c>Info</c> is empty when <c>Success</c> is False.
        /// </remarks>
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
        /// <param name="Ibans">The IBANs to parse (max <see cref="IbanNetService.MaxBulkItems"/>). Each item is normalized; a null item is treated as empty.</param>
        /// <param name="Result">The bulk parse result with call-level status, counts and per-item results in input order.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>ParseIbanBulk</c> in the <c>IbanCheckerOS</c> external library.
        /// One bad item never fails the batch. The only call-level error is <see cref="IbanErrorCodes.TooManyItems"/>.
        /// </remarks>
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
        /// <param name="Iban">The IBAN to format. Whitespace, dashes and lowercase are accepted and normalized.</param>
        /// <param name="Format"><c>Electronic</c>, <c>Print</c>, <c>Obfuscated</c> or <c>Hidden</c> (case-insensitive, trimmed). Null or empty defaults to <c>Electronic</c>.</param>
        /// <param name="Result">The format result: <c>Success</c>, <c>ErrorCode</c>, <c>ErrorMessage</c>, <c>NormalizedIban</c> and <c>Value</c>.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>FormatIban</c> in the <c>IbanCheckerOS</c> external library.
        /// An unknown <paramref name="Format"/> returns <see cref="IbanErrorCodes.InvalidFormat"/> before the IBAN is checked.
        /// Otherwise the error codes are those of <see cref="ValidateIban"/> except <c>CountryNotAccepted</c>.
        /// Example for <c>NL91ABNA0417164300</c>: Print <c>NL91 ABNA 0417 1643 00</c>, Obfuscated <c>XXXXXXXXXXXXXX4300</c>, Hidden <c>****</c>.
        /// </remarks>
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
        /// <param name="Ibans">The IBANs to format (max <see cref="IbanNetService.MaxBulkItems"/>). Each item is normalized; a null item is treated as empty.</param>
        /// <param name="Format"><c>Electronic</c>, <c>Print</c>, <c>Obfuscated</c> or <c>Hidden</c> (case-insensitive, trimmed). Null or empty defaults to <c>Electronic</c>. Applied to every item.</param>
        /// <param name="Result">The bulk format result with call-level status, counts and per-item results in input order.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>FormatIbanBulk</c> in the <c>IbanCheckerOS</c> external library.
        /// Call-level errors (checked in this order): <see cref="IbanErrorCodes.TooManyItems"/>, then
        /// <see cref="IbanErrorCodes.InvalidFormat"/>. Either sets <c>Success</c> to False and returns no items.
        /// </remarks>
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
        /// Gets the IBAN definition of a country from the SWIFT IBAN registry.
        /// </summary>
        /// <param name="CountryCode">ISO 3166-1 alpha-2 country code (case-insensitive, trimmed), e.g. <c>NL</c>.</param>
        /// <param name="Result">The lookup result: <c>Found</c> and <c>Country</c>. <c>Found</c> is False (and <c>Country</c> empty) for empty, unknown or non-registry codes.</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>GetCountryInfo</c> in the <c>IbanCheckerOS</c> external library.
        /// This action has no <c>ErrorCode</c>; check <c>Found</c>.
        /// </remarks>
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
        /// Lists every country in the SWIFT IBAN registry, ordered by country code.
        /// </summary>
        /// <param name="Countries">The supported countries (88 with IbanNet 6.2.1), ordered by <c>Code</c> (ordinal).</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>ListSupportedCountries</c> in the <c>IbanCheckerOS</c> external library.
        /// Has no inputs and no failure mode. The list is built once per Lambda instance and copied on every call.
        /// </remarks>
        [OSAction(
            Description = "Lists all countries supported by the SWIFT IBAN registry, ordered by country code.",
            IconResourceName = "IbanCheckerOS.resources.IbanCheckerOS_icon.png"
        )]
        void ListSupportedCountries(
            [OSParameterAttribute(Description = "The supported countries, ordered by country code.")]
            out List<IbanCountryInfo> Countries
        );

        /// <summary>
        /// Generates random IBANs that pass validation. For testing only.
        /// </summary>
        /// <param name="CountryCode">ISO 3166-1 alpha-2 country code (case-insensitive, trimmed), e.g. <c>DE</c>. Must exist in the SWIFT IBAN registry.</param>
        /// <param name="Count">Number of IBANs to generate, from 1 to <see cref="IbanNetService.MaxGeneratedIbans"/> (1000).</param>
        /// <param name="Result">The generation result: <c>Success</c>, <c>ErrorCode</c>, <c>ErrorMessage</c> and <c>Ibans</c> (electronic format).</param>
        /// <remarks>
        /// ODC mapping: Server Action <c>GenerateTestIbans</c> in the <c>IbanCheckerOS</c> external library.
        /// The generated IBANs are NOT real bank accounts and must never be used for payments.
        /// Inputs are checked in this order: <c>Empty</c> (no country code), <c>UnknownCountry</c>, <c>InvalidCount</c>;
        /// <c>UnexpectedError</c> is returned if generation fails.
        /// </remarks>
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
