using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Result of generating test IBANs.
    /// </summary>
    [OSStructure(Description = "Result of generating random, structurally valid IBANs for testing only.")]
    public struct IbanGenerateResult
    {
        /// <summary>True when the IBANs were generated.</summary>
        [OSStructureField(Description = "True when the IBANs were generated.")]
        public bool Success;

        /// <summary>Error code; empty on success.</summary>
        [OSStructureField(Description = "Error code; empty on success. One of: Empty, UnknownCountry, InvalidCount, UnexpectedError.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The generated IBANs in electronic format.</summary>
        [OSStructureField(Description = "The generated IBANs in electronic format. For testing only: they pass validation but are not real bank accounts.")]
        public List<string> Ibans;

        /// <summary>Creates an empty (failed) result.</summary>
        public IbanGenerateResult()
        {
            Success = false;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            Ibans = new List<string>();
        }
    }
}
