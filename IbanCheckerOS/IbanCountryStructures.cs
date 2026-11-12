using System;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// IBAN definition of a country from the SWIFT IBAN registry.
    /// </summary>
    /// <remarks>Returned by <c>GetCountryInfo</c> (inside <see cref="IbanCountryResult"/>) and as a list by <c>ListSupportedCountries</c>. Exposed to ODC as the <c>IbanCountryInfo</c> structure.</remarks>
    [OSStructure(Description = "IBAN definition of a country from the SWIFT IBAN registry.")]
    public struct IbanCountryInfo
    {
        /// <summary>ISO 3166-1 alpha-2 country code.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "ISO 3166-1 alpha-2 country code, e.g. NL.")]
        public string Code;

        /// <summary>English country name as listed in the SWIFT IBAN registry, e.g. "Netherlands (The)".</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "English country name as listed in the SWIFT IBAN registry.")]
        public string EnglishName;

        /// <summary>Total IBAN length (electronic format) for the country, e.g. 18 for NL.</summary>
        /// <remarks>Maps to ODC type <c>Integer</c>.</remarks>
        [OSStructureField(Description = "Total IBAN length (electronic format) for the country.")]
        public int IbanLength;

        /// <summary>Example IBAN from the SWIFT registry, in electronic format.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Example IBAN from the SWIFT registry (electronic format).")]
        public string IbanExample;

        /// <summary>Example BBAN (the IBAN without country code and check digits) from the SWIFT registry.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Example BBAN from the SWIFT registry.")]
        public string BbanExample;

        /// <summary>True if the country is a SEPA member.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True if the country is a SEPA member.")]
        public bool IsSepaMember;

        /// <summary>True if the country's IBAN contains a bank identifier.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True if the country's IBAN contains a bank identifier.")]
        public bool HasBankIdentifier;

        /// <summary>True if the country's IBAN contains a branch identifier.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True if the country's IBAN contains a branch identifier.")]
        public bool HasBranchIdentifier;

        /// <summary>Date (UTC) the IBAN came into effect for this country.</summary>
        /// <remarks>Maps to ODC type <c>Date Time</c>.</remarks>
        [OSStructureField(Description = "Date (UTC) the IBAN came into effect for this country.")]
        public DateTime EffectiveDate;

        /// <summary>Creates an empty country info.</summary>
        public IbanCountryInfo()
        {
            Code = string.Empty;
            EnglishName = string.Empty;
            IbanLength = 0;
            IbanExample = string.Empty;
            BbanExample = string.Empty;
            IsSepaMember = false;
            HasBankIdentifier = false;
            HasBranchIdentifier = false;
            EffectiveDate = DateTime.MinValue;
        }
    }

    /// <summary>
    /// Result of a country lookup.
    /// </summary>
    /// <remarks>Output of <c>GetCountryInfo</c>. Has no error code: an empty, unknown or non-registry code returns <c>Found</c> = False. Exposed to ODC as the <c>IbanCountryResult</c> structure.</remarks>
    [OSStructure(Description = "Result of looking up a country in the SWIFT IBAN registry.")]
    public struct IbanCountryResult
    {
        /// <summary>True if the country exists in the registry.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True if the country code exists in the SWIFT IBAN registry.")]
        public bool Found;

        /// <summary>The country definition; empty when Found is False.</summary>
        /// <remarks>Maps to ODC type <c>IbanCountryInfo</c>.</remarks>
        [OSStructureField(Description = "The country definition; empty when Found is False.")]
        public IbanCountryInfo Country;

        /// <summary>Creates an empty (not found) result.</summary>
        public IbanCountryResult()
        {
            Found = false;
            Country = new IbanCountryInfo();
        }
    }
}
