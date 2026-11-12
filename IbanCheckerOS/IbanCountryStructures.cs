using System;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// IBAN definition of a country from the SWIFT IBAN registry.
    /// </summary>
    [OSStructure(Description = "IBAN definition of a country from the SWIFT IBAN registry.")]
    public struct IbanCountryInfo
    {
        /// <summary>ISO 3166-1 alpha-2 country code.</summary>
        [OSStructureField(Description = "ISO 3166-1 alpha-2 country code, e.g. NL.")]
        public string Code;

        /// <summary>English country name.</summary>
        [OSStructureField(Description = "English country name as listed in the SWIFT IBAN registry.")]
        public string EnglishName;

        /// <summary>Total IBAN length for the country.</summary>
        [OSStructureField(Description = "Total IBAN length (electronic format) for the country.")]
        public int IbanLength;

        /// <summary>Example IBAN.</summary>
        [OSStructureField(Description = "Example IBAN from the SWIFT registry (electronic format).")]
        public string IbanExample;

        /// <summary>Example BBAN.</summary>
        [OSStructureField(Description = "Example BBAN from the SWIFT registry.")]
        public string BbanExample;

        /// <summary>True if the country is a SEPA member.</summary>
        [OSStructureField(Description = "True if the country is a SEPA member.")]
        public bool IsSepaMember;

        /// <summary>True if the IBAN contains a bank identifier.</summary>
        [OSStructureField(Description = "True if the country's IBAN contains a bank identifier.")]
        public bool HasBankIdentifier;

        /// <summary>True if the IBAN contains a branch identifier.</summary>
        [OSStructureField(Description = "True if the country's IBAN contains a branch identifier.")]
        public bool HasBranchIdentifier;

        /// <summary>Date (UTC) the IBAN came into effect for this country.</summary>
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
    [OSStructure(Description = "Result of looking up a country in the SWIFT IBAN registry.")]
    public struct IbanCountryResult
    {
        /// <summary>True if the country exists in the registry.</summary>
        [OSStructureField(Description = "True if the country code exists in the SWIFT IBAN registry.")]
        public bool Found;

        /// <summary>The country definition; empty when Found is False.</summary>
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
