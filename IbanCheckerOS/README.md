# IbanCheckerOS — OutSystems ODC External Library

> Validate, parse, format and generate IBANs in ODC apps using IbanNet 6.2.1 and the official SWIFT IBAN registry (88 countries).

IBAN inputs are normalized before use (trimmed, whitespace and dashes removed, uppercased). Invalid input never throws: the IBAN actions report problems through `ErrorCode` and `ErrorMessage`. See the [repository README](../README.md) for the full field-level reference and the error code table.

## Actions

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| ValidateIban | Validates one IBAN (length, characters, BBAN structure, mod-97) with optional country rules. | Iban: Text, AllowedCountries: Text List, RejectedCountries: Text List | Result: IbanValidationResult |
| ValidateIbanBulk | Validates up to 5,000 IBANs; counts always cover the full input. | Ibans: Text List, AllowedCountries: Text List, RejectedCountries: Text List, OnlyInvalid: Boolean | Result: IbanBulkValidationResult |
| ParseIban | Returns the formats, country, check digits, BBAN, bank/branch identifiers and SEPA membership of a valid IBAN. | Iban: Text | Result: IbanParseResult |
| ParseIbanBulk | Parses up to 5,000 IBANs. | Ibans: Text List | Result: IbanBulkParseResult |
| FormatIban | Formats a valid IBAN as Electronic, Print, Obfuscated or Hidden (case-insensitive; empty means Electronic). | Iban: Text, Format: Text | Result: IbanFormatResult |
| FormatIbanBulk | Formats up to 5,000 IBANs with one format. | Ibans: Text List, Format: Text | Result: IbanBulkFormatResult |
| GetCountryInfo | Returns the SWIFT registry definition of a country; `Found` is False for unknown codes. | CountryCode: Text | Result: IbanCountryResult |
| ListSupportedCountries | Lists all registry countries, ordered by code. | (none) | Countries: IbanCountryInfo List |
| GenerateTestIbans | **For testing only.** Generates 1 to 1000 random IBANs that pass validation. Not real accounts. | CountryCode: Text, Count: Integer | Result: IbanGenerateResult |

## Structures

| Structure | Description | Fields |
|-----------|-------------|--------|
| IbanValidationResult | Result of ValidateIban. | IsValid, ErrorCode, ErrorMessage, CountryCode, NormalizedIban |
| IbanBulkValidationItem | One item of ValidateIbanBulk. | Index, Input, NormalizedIban, IsValid, ErrorCode, ErrorMessage, CountryCode |
| IbanBulkValidationResult | Result of ValidateIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanInfo | Details extracted from a valid IBAN. | Electronic, Print, Obfuscated, CountryCode, CountryName, CheckDigits, Bban, BankIdentifier, BranchIdentifier, IsSepaMember |
| IbanParseResult | Result of ParseIban. | Success, ErrorCode, ErrorMessage, NormalizedIban, Info |
| IbanBulkParseItem | One item of ParseIbanBulk. | Index, Input, NormalizedIban, Success, ErrorCode, ErrorMessage, Info |
| IbanBulkParseResult | Result of ParseIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanFormatResult | Result of FormatIban. | Success, ErrorCode, ErrorMessage, NormalizedIban, Value |
| IbanBulkFormatItem | One item of FormatIbanBulk. | Index, Input, NormalizedIban, Success, ErrorCode, ErrorMessage, Value |
| IbanBulkFormatResult | Result of FormatIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanCountryInfo | IBAN definition of a country. | Code, EnglishName, IbanLength, IbanExample, BbanExample, IsSepaMember, HasBankIdentifier, HasBranchIdentifier, EffectiveDate |
| IbanCountryResult | Result of GetCountryInfo. | Found, Country |
| IbanGenerateResult | Result of GenerateTestIbans. | Success, ErrorCode, ErrorMessage, Ibans |

## Installation

1. Build: `./generate_upload_package.sh` (macOS/Linux) or `./generate_upload_package.ps1` (Windows). Both run `dotnet publish -c Release -r linux-x64 --self-contained false` and zip the `publish/` output into `IbanCheckerOS_Asset.zip`.
2. Alternatively, download `IbanCheckerOS_vX.Y.Z.zip` from the GitHub release (built by `.github/workflows/release.yml` on `ubuntu-26.04`).
3. Upload the ZIP in ODC Portal > External Libraries > Upload new library.

## Local Development

Requires the .NET 10 SDK.

```bash
dotnet build IbanCheckerOS.sln
dotnet test ../IbanCheckerOS.UnitTests/IbanCheckerOS.UnitTests.csproj
```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| IbanNet | 6.2.1 | IBAN logic and the embedded SWIFT IBAN registry. |
| OutSystems.ExternalLibraries.SDK | 1.7.0 | ODC External Library attributes. |

## Telemetry

None. The library does not log or store IBANs.

## Constraints

- Stateless: each call is independent. Static caches only speed up warm Lambda invocations and never change results.
- Bulk actions accept at most 5,000 items per call (call-level `TooManyItems`), to stay within the ODC 5.5 MB payload limit and ~95 s timeout.
- GenerateTestIbans accepts a Count from 1 to 1000 (`InvalidCount` otherwise).
- Validation checks format and checksum only; it does not confirm that the account exists.
- The SWIFT registry is embedded (no network calls); registry updates require a new library version.
