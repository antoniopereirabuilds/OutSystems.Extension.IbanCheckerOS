using System.Collections.Generic;
using OutSystems.ExternalLibraries.SDK;

namespace IbanCheckerOS
{
    /// <summary>
    /// Result of generating test IBANs.
    /// </summary>
    /// <remarks>Output of <c>GenerateTestIbans</c>. The IBANs are not real bank accounts and must never be used for payments. Exposed to ODC as the <c>IbanGenerateResult</c> structure.</remarks>
    [OSStructure(Description = "Result of generating random, structurally valid IBANs for testing only.")]
    public struct IbanGenerateResult
    {
        /// <summary>True when the IBANs were generated.</summary>
        /// <remarks>Maps to ODC type <c>Boolean</c>.</remarks>
        [OSStructureField(Description = "True when the IBANs were generated.")]
        public bool Success;

        /// <summary>Error code; empty on success. One of <c>Empty</c>, <c>UnknownCountry</c>, <c>InvalidCount</c>, <c>UnexpectedError</c>.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Error code; empty on success. One of: Empty, UnknownCountry, InvalidCount, UnexpectedError.")]
        public string ErrorCode;

        /// <summary>Error message; empty on success.</summary>
        /// <remarks>Maps to ODC type <c>Text</c>.</remarks>
        [OSStructureField(Description = "Human-readable error message; empty on success.")]
        public string ErrorMessage;

        /// <summary>The generated IBANs in electronic format.</summary>
        /// <remarks>Maps to ODC type <c>Text List</c>.</remarks>
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
