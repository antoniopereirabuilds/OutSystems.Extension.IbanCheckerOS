![Tests](https://github.com/antoniopereirabuilds/OutSystems.Extension.IbanCheckerOS/actions/workflows/test.yml/badge.svg) ![Release](https://github.com/antoniopereirabuilds/OutSystems.Extension.IbanCheckerOS/actions/workflows/release.yml/badge.svg)
# IbanCheckerOS

OutSystems Developer Cloud (ODC) External Library that validates, parses, formats and generates IBANs (International Bank Account Numbers). It wraps [IbanNet](https://www.nuget.org/packages/IbanNet) 6.2.1 and uses only the official **SWIFT IBAN registry** (88 countries). It appears in ODC Studio as the **IbanCheckerOS** library.

Two rules apply to every action:

- **Input normalization:** IBAN inputs are trimmed, all whitespace and dash characters (`-`, Unicode dashes) are removed, and the result is uppercased. The validate, parse and format results return the normalized value as `NormalizedIban`. Country codes (`CountryCode`, `AllowedCountries`, `RejectedCountries`) are trimmed and uppercased.
- **No exceptions for bad data:** invalid, null or empty inputs never throw. The validate, parse, format and generate actions return a result structure with `ErrorCode` and `ErrorMessage`. `GetCountryInfo` reports an unknown country with `Found` = False, and `ListSupportedCountries` has no failure mode.

## Actions

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| ValidateIban | Validates one IBAN, with optional country rules. | Iban, AllowedCountries, RejectedCountries | Result (IbanValidationResult) |
| ValidateIbanBulk | Validates up to 5,000 IBANs in one call. | Ibans, AllowedCountries, RejectedCountries, OnlyInvalid | Result (IbanBulkValidationResult) |
| ParseIban | Splits a valid IBAN into its components. | Iban | Result (IbanParseResult) |
| ParseIbanBulk | Parses up to 5,000 IBANs in one call. | Ibans | Result (IbanBulkParseResult) |
| FormatIban | Formats a valid IBAN as Electronic, Print, Obfuscated or Hidden. | Iban, Format | Result (IbanFormatResult) |
| FormatIbanBulk | Formats up to 5,000 IBANs with one format. | Ibans, Format | Result (IbanBulkFormatResult) |
| GetCountryInfo | Returns the registry definition of one country. | CountryCode | Result (IbanCountryResult) |
| ListSupportedCountries | Lists all registry countries, ordered by code. | (none) | Countries (IbanCountryInfo List) |
| GenerateTestIbans | **For testing only.** Generates random IBANs that pass validation. | CountryCode, Count | Result (IbanGenerateResult) |

### ValidateIban

Validates one IBAN: length, characters, BBAN structure and mod-97 check digits. It can also apply country rules.

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Iban | Text | The IBAN to validate. |
| Input | AllowedCountries | Text List | Optional. Only IBANs from these countries are accepted. Empty accepts all countries. |
| Input | RejectedCountries | Text List | Optional. IBANs from these countries are rejected. This applies on top of AllowedCountries, so a rejection always wins. |
| Output | Result | IbanValidationResult | The validation result. |

### ValidateIbanBulk

Validates up to 5,000 IBANs in one call.

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Ibans | Text List | The IBANs to validate (max 5,000). |
| Input | AllowedCountries | Text List | Optional. Same as in ValidateIban. |
| Input | RejectedCountries | Text List | Optional. Same as in ValidateIban. |
| Input | OnlyInvalid | Boolean | When True, Results contains only the invalid items. The counts still cover the full input. |
| Output | Result | IbanBulkValidationResult | Counts and per-item results. |

### ParseIban

Validates the IBAN (without country rules) and, if it is valid, returns its components in `Info`.

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Iban | Text | The IBAN to parse. |
| Output | Result | IbanParseResult | Success, error information, NormalizedIban and Info. |

### ParseIbanBulk

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Ibans | Text List | The IBANs to parse (max 5,000). |
| Output | Result | IbanBulkParseResult | Counts and per-item results. |

### FormatIban

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Iban | Text | The IBAN to format. |
| Input | Format | Text | `Electronic`, `Print`, `Obfuscated` or `Hidden`. Case-insensitive. Empty means `Electronic`. |
| Output | Result | IbanFormatResult | The formatted `Value`, or error information. |

An unknown `Format` returns `InvalidFormat` before the IBAN is checked. Example output for `NL91ABNA0417164300`:

| Format | Output |
|--------|--------|
| Electronic | `NL91ABNA0417164300` |
| Print | `NL91 ABNA 0417 1643 00` |
| Obfuscated | `XXXXXXXXXXXXXX4300` |
| Hidden | `****` |

### FormatIbanBulk

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | Ibans | Text List | The IBANs to format (max 5,000). |
| Input | Format | Text | Same as in FormatIban. An invalid format is a call-level `InvalidFormat` error. |
| Output | Result | IbanBulkFormatResult | Counts and per-item results. |

### GetCountryInfo

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | CountryCode | Text | ISO 3166-1 alpha-2 code. Case-insensitive and trimmed. |
| Output | Result | IbanCountryResult | Found and Country. Found is False for empty, unknown or non-registry codes. |

### ListSupportedCountries

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Output | Countries | IbanCountryInfo List | All countries in the SWIFT registry, ordered by code. |

### GenerateTestIbans

**For testing only.** Generates random IBANs that pass validation for a country. They are not real bank accounts and must never be used for payments.

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | CountryCode | Text | ISO 3166-1 alpha-2 code. Case-insensitive and trimmed. |
| Input | Count | Integer | Number of IBANs to generate (1 to 1000). |
| Output | Result | IbanGenerateResult | The generated IBANs, or error information. |

## Structures

| Structure | Description | Fields |
|-----------|-------------|--------|
| IbanValidationResult | Result of ValidateIban. | IsValid, ErrorCode, ErrorMessage, CountryCode, NormalizedIban |
| IbanBulkValidationItem | One item of a bulk validation. | Index, Input, NormalizedIban, IsValid, ErrorCode, ErrorMessage, CountryCode |
| IbanBulkValidationResult | Result of ValidateIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanInfo | Details extracted from a valid IBAN. | Electronic, Print, Obfuscated, CountryCode, CountryName, CheckDigits, Bban, BankIdentifier, BranchIdentifier, IsSepaMember |
| IbanParseResult | Result of ParseIban. | Success, ErrorCode, ErrorMessage, NormalizedIban, Info |
| IbanBulkParseItem | One item of a bulk parse. | Index, Input, NormalizedIban, Success, ErrorCode, ErrorMessage, Info |
| IbanBulkParseResult | Result of ParseIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanFormatResult | Result of FormatIban. | Success, ErrorCode, ErrorMessage, NormalizedIban, Value |
| IbanBulkFormatItem | One item of a bulk format. | Index, Input, NormalizedIban, Success, ErrorCode, ErrorMessage, Value |
| IbanBulkFormatResult | Result of FormatIbanBulk. | Success, ErrorCode, ErrorMessage, TotalCount, SuccessCount, FailureCount, Results |
| IbanCountryInfo | IBAN definition of a country. | Code, EnglishName, IbanLength, IbanExample, BbanExample, IsSepaMember, HasBankIdentifier, HasBranchIdentifier, EffectiveDate |
| IbanCountryResult | Result of GetCountryInfo. | Found, Country |
| IbanGenerateResult | Result of GenerateTestIbans. | Success, ErrorCode, ErrorMessage, Ibans |

### IbanValidationResult

| Field | Type | Description |
|-------|------|-------------|
| IsValid | Boolean | True when the IBAN is valid and its country passes the country rules. |
| ErrorCode | Text | Error code. Empty when the IBAN is valid. |
| ErrorMessage | Text | English error message. Empty when the IBAN is valid. |
| CountryCode | Text | ISO country code, when it can be determined. Some invalid IBANs have one too, for example a wrong length for a known country. |
| NormalizedIban | Text | The normalized input. |

### IbanBulkValidationItem

| Field | Type | Description |
|-------|------|-------------|
| Index | Integer | Zero-based position in the input list. |
| Input | Text | The original input value. A null item becomes an empty string. |
| NormalizedIban | Text | The normalized input. |
| IsValid | Boolean | Same meaning as in IbanValidationResult. |
| ErrorCode | Text | Same meaning as in IbanValidationResult. |
| ErrorMessage | Text | Same meaning as in IbanValidationResult. |
| CountryCode | Text | Same meaning as in IbanValidationResult. |

### IbanBulkValidationResult, IbanBulkParseResult, IbanBulkFormatResult

| Field | Type | Description |
|-------|------|-------------|
| Success | Boolean | False only when the whole call is rejected (`TooManyItems`, or `InvalidFormat` for format calls). Invalid items do not set it to False. |
| ErrorCode | Text | Call-level error code. Empty on success. |
| ErrorMessage | Text | Call-level error message. Empty on success. |
| TotalCount | Integer | Number of input items. Set even when the call is rejected. |
| SuccessCount | Integer | Number of items that succeeded, counted over the full input. |
| FailureCount | Integer | Number of items that failed, counted over the full input. |
| Results | List | IbanBulkValidationItem, IbanBulkParseItem or IbanBulkFormatItem, in input order. Empty when the call is rejected. |

### IbanInfo

| Field | Type | Description |
|-------|------|-------------|
| Electronic | Text | Electronic format, for example `NL91ABNA0417164300`. |
| Print | Text | Print format, for example `NL91 ABNA 0417 1643 00`. |
| Obfuscated | Text | Obfuscated format, for example `XXXXXXXXXXXXXX4300`. |
| CountryCode | Text | ISO 3166-1 alpha-2 code. |
| CountryName | Text | English name from the SWIFT registry, for example "Netherlands (The)". |
| CheckDigits | Text | The two check digits. |
| Bban | Text | The BBAN (the part after the country code and check digits). |
| BankIdentifier | Text | The bank identifier. Empty if the country does not define one. |
| BranchIdentifier | Text | The branch identifier. Empty if the country does not define one. |
| IsSepaMember | Boolean | True if the country is a SEPA member. |

### IbanParseResult, IbanBulkParseItem

| Field | Type | Description |
|-------|------|-------------|
| Index | Integer | Bulk item only. Zero-based position in the input list. |
| Input | Text | Bulk item only. The original input value. |
| NormalizedIban | Text | The normalized input. |
| Success | Boolean | True when the IBAN is valid and was parsed. |
| ErrorCode | Text | Empty on success. Same codes as ValidateIban, except `CountryNotAccepted`. |
| ErrorMessage | Text | English error message. Empty on success. |
| Info | IbanInfo | The parsed details. Empty when Success is False. |

### IbanFormatResult, IbanBulkFormatItem

| Field | Type | Description |
|-------|------|-------------|
| Index | Integer | Bulk item only. Zero-based position in the input list. |
| Input | Text | Bulk item only. The original input value. |
| NormalizedIban | Text | The normalized input. |
| Success | Boolean | True when the IBAN is valid and was formatted. |
| ErrorCode | Text | Empty on success. `InvalidFormat` (single action only), otherwise the ValidateIban codes except `CountryNotAccepted`. |
| ErrorMessage | Text | English error message. Empty on success. |
| Value | Text | The formatted IBAN. Empty on failure. |

### IbanCountryInfo

| Field | Type | Description |
|-------|------|-------------|
| Code | Text | ISO 3166-1 alpha-2 code. |
| EnglishName | Text | English name from the SWIFT registry. |
| IbanLength | Integer | Total IBAN length. |
| IbanExample | Text | Example IBAN from the registry. |
| BbanExample | Text | Example BBAN from the registry. |
| IsSepaMember | Boolean | SEPA membership. |
| HasBankIdentifier | Boolean | True if the IBAN contains a bank identifier. |
| HasBranchIdentifier | Boolean | True if the IBAN contains a branch identifier. |
| EffectiveDate | Date Time | Date (UTC) the IBAN came into effect for the country. |

### IbanCountryResult

| Field | Type | Description |
|-------|------|-------------|
| Found | Boolean | True if the country is in the registry. |
| Country | IbanCountryInfo | The country definition. Empty when Found is False. |

### IbanGenerateResult

| Field | Type | Description |
|-------|------|-------------|
| Success | Boolean | True when the IBANs were generated. |
| ErrorCode | Text | `Empty`, `UnknownCountry`, `InvalidCount` or `UnexpectedError`. Empty on success. |
| ErrorMessage | Text | Error message. Empty on success. |
| Ibans | Text List | The generated IBANs, in electronic format. **For testing only.** |

## Error codes

| ErrorCode | Meaning |
|-----------|---------|
| *(empty)* | No error. |
| `Empty` | The input was null, empty, or only whitespace and dashes. |
| `InvalidLength` | The length does not match the country's IBAN length. |
| `IllegalCharacters` | The input contains characters other than A-Z and 0-9, including in the country-code position. |
| `InvalidStructure` | The input does not match the country's BBAN pattern, or the check digits are not numeric. |
| `InvalidCheckDigits` | The mod-97 check digits are wrong. |
| `UnknownCountry` | The country code is not in the SWIFT IBAN registry. |
| `CountryNotAccepted` | The IBAN is valid, but the AllowedCountries/RejectedCountries rules exclude its country (ValidateIban and ValidateIbanBulk only). |
| `InvalidFormat` | The format is not Electronic, Print, Obfuscated or Hidden (FormatIban and FormatIbanBulk). |
| `InvalidCount` | The count is outside 1..1000 (GenerateTestIbans). |
| `TooManyItems` | A bulk call received more than 5,000 items. This is a call-level error. |
| `UnexpectedError` | An unexpected internal error. It should not happen. |

## Usage Notes

- Store IBANs in the `NormalizedIban` (electronic) form, and show them to users in `Print` or `Obfuscated` form.
- `Index` in bulk results is zero-based, so it matches ODC list indexes.
- Error messages are fixed English text and do not depend on the culture. Branch on `ErrorCode` in your logic, not on `ErrorMessage`.
- `CountryNotAccepted` is reported only for IBANs that are otherwise valid. Structural errors are reported first.

## Installation

1. Get the upload package, either:
   - download `IbanCheckerOS_vX.Y.Z.zip` from the GitHub release, or
   - build it locally (see below), which produces `IbanCheckerOS/IbanCheckerOS_Asset.zip`.
2. In ODC Portal, go to **External Libraries** > **Upload new library** and upload the ZIP.
3. Publish the library, then add **IbanCheckerOS** as a dependency of your app in ODC Studio.

The package contains the output of `dotnet publish -c Release -r linux-x64 --self-contained false`.

## Local Development

Requires the .NET 10 SDK.

```bash
cd IbanCheckerOS
dotnet build IbanCheckerOS.sln
dotnet test ../IbanCheckerOS.UnitTests/IbanCheckerOS.UnitTests.csproj
./generate_upload_package.sh      # or generate_upload_package.ps1 on Windows
```

## Continuous Integration

| Workflow | Trigger | What it does |
|----------|---------|--------------|
| `.github/workflows/test.yml` | Push or pull request to `main`/`master`, manual run | Restores, runs the unit tests with coverage, uploads `coverage-report` (kept 30 days). |
| `.github/workflows/release.yml` | Push of a `vX.Y.Z` tag | Runs the tests, publishes for linux-x64, and creates a GitHub release with `IbanCheckerOS_vX.Y.Z.zip`. |

Both workflows run on `ubuntu-26.04` with .NET `10.0.x` and use `actions/checkout@v7`, `actions/setup-dotnet@v6` and `irongut/CodeCoverageSummary@v1.3.0`. The test workflow also uses `actions/upload-artifact@v7`, and the release workflow uses `softprops/action-gh-release@v3`.

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| IbanNet | 6.2.1 | IBAN validation, parsing, formatting, generation and the SWIFT IBAN registry. |
| OutSystems.ExternalLibraries.SDK | 1.7.0 | ODC attributes (`OSInterface`, `OSAction`, `OSStructure`, ...). |
| xunit.v3 | 3.2.2 | Unit tests (test project only). |
| Microsoft.NET.Test.Sdk | 18.12.0 | Test host (test project only). |
| coverlet.collector | 10.1.0 | Code coverage in CI (test project only). |

## Constraints

- **Stateless:** each call is independent. Static caches (registry, validators, country list) only speed up warm invocations and never change results.
- **Bulk cap:** bulk actions accept at most 5,000 items per call (call-level `TooManyItems`). This keeps calls within ODC's 5.5 MB payload limit and ~95 s timeout. Split larger lists into batches.
- **Generation cap:** GenerateTestIbans produces 1 to 1000 IBANs per call.
- **Format and checksum only:** validation does not confirm that the bank account exists or is open.
- **Offline registry:** the SWIFT registry is embedded in IbanNet 6.2.1 (88 countries). No external calls are made; registry updates require a new library version.
- **Test IBANs:** GenerateTestIbans output is not a real bank account and must never be used for payments.
- **Telemetry:** none. The library does not log or store any IBAN.

## Technical Details

- **Library:** IbanNet v6.2.1 (SWIFT IBAN registry, `IbanRegistry.Default` / `SwiftRegistryProvider`)
- **Target:** .NET 10.0, linux-x64 (framework-dependent)
- **ODC SDK:** OutSystems.ExternalLibraries.SDK v1.7.0
- **Tests:** xUnit v3 (185 tests)
- **License:** Apache-2.0 (same as IbanNet). See [LICENSE](LICENSE), [NOTICE](NOTICE) and [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## Credits & License

This is an unofficial OutSystems wrapper around [IbanNet](https://github.com/skwasjer/IbanNet) by Martijn Bodeman. All IBAN validation, parsing, formatting and generation logic comes from IbanNet. This project only adds the ODC action surface, input normalization, bulk handling and result structures.

- Wrapper code: Apache License 2.0. See [LICENSE](LICENSE) and [NOTICE](NOTICE).
- IbanNet: Apache License 2.0. OutSystems.ExternalLibraries.SDK: BSD 3-Clause. Both are redistributed unmodified in the upload zip. See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
- Not affiliated with or endorsed by the IbanNet author, OutSystems or SWIFT.
