![Tests](https://github.com/antoniopereirabuilds/OutSystems.Extension.IbanCheckerOS/actions/workflows/test.yml/badge.svg) ![Release](https://github.com/antoniopereirabuilds/OutSystems.Extension.IbanCheckerOS/actions/workflows/release.yml/badge.svg)
# IbanCheckerOS

OutSystems Developer Cloud (ODC) External Library that validates, parses, formats and generates IBANs (International Bank Account Numbers). It wraps [IbanNet](https://www.nuget.org/packages/IbanNet) 6.2.1 and uses only the official **SWIFT IBAN registry** (88 countries). It appears in ODC Studio as the **IbanCheckerOS** library.

Two rules apply to every action:

- **Input normalization:** inputs are trimmed, all whitespace and dash characters (`-`, Unicode dashes) are removed, and the result is uppercased. The normalized value is always returned as `NormalizedIban`.
- **No exceptions for bad data:** invalid, null or empty inputs never throw. Every action returns a result structure with `ErrorCode` and `ErrorMessage`.

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
| `CountryNotAccepted` | The IBAN is valid, but the AllowedCountries/RejectedCountries rules exclude its country. |
| `InvalidFormat` | The format is not Electronic, Print, Obfuscated or Hidden (FormatIban and FormatIbanBulk). |
| `InvalidCount` | The count is outside 1..1000 (GenerateTestIbans). |
| `TooManyItems` | A bulk call received more than 5,000 items. This is a call-level error. |
| `UnexpectedError` | An unexpected internal error. It should not happen. |

## Structures

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
| Input | Text | The original input value. |
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
| TotalCount | Integer | Number of input items. |
| SuccessCount | Integer | Number of items that succeeded, counted over the full input. |
| FailureCount | Integer | Number of items that failed, counted over the full input. |
| Results | List | IbanBulkValidationItem, IbanBulkParseItem or IbanBulkFormatItem, in input order. |

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

These contain `Success`, `ErrorCode`, `ErrorMessage`, `NormalizedIban` and `Info` (IbanInfo). The bulk item also has `Index` and `Input`.

### IbanFormatResult, IbanBulkFormatItem

These contain `Success`, `ErrorCode`, `ErrorMessage`, `NormalizedIban` and `Value` (the formatted IBAN). The bulk item also has `Index` and `Input`.

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
| ErrorCode | Text | `Empty`, `UnknownCountry`, `InvalidCount` or `UnexpectedError`. |
| ErrorMessage | Text | Error message. |
| Ibans | Text List | The generated IBANs, in electronic format. **For testing only.** |

## Actions

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

Example output for `NL91ABNA0417164300`:

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
| Output | Result | IbanCountryResult | Found and Country. |

### ListSupportedCountries

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Output | Countries | IbanCountryInfo List | All countries in the SWIFT registry, ordered by code. |

### GenerateTestIbans

**For testing only.** Generates random IBANs that pass validation for a country. They are not real bank accounts and must never be used for payments.

| Direction | Parameter | Type | Description |
|-----------|-----------|------|-------------|
| Input | CountryCode | Text | ISO 3166-1 alpha-2 code. |
| Input | Count | Integer | Number of IBANs to generate (1 to 1000). |
| Output | Result | IbanGenerateResult | The generated IBANs, or error information. |

## Usage Notes

- Store IBANs in the `NormalizedIban` (electronic) form, and show them to users in `Print` or `Obfuscated` form.
- `Index` in bulk results is zero-based, so it matches ODC list indexes.
- Error messages are fixed English text and do not depend on the culture. Branch on `ErrorCode` in your logic, not on `ErrorMessage`.
- Bulk calls are capped at 5,000 items to stay within ODC's 5.5 MB payload limit and ~95 s timeout. Split larger lists into batches.
- `CountryNotAccepted` is reported only for IBANs that are otherwise valid. Structural errors are reported first.
- Validation checks the format and the checksum only. It does not confirm that the bank account exists.

## Build and Package

```bash
cd IbanCheckerOS
dotnet build IbanCheckerOS.sln
dotnet test ../IbanCheckerOS.UnitTests/IbanCheckerOS.UnitTests.csproj
./generate_upload_package.sh      # or generate_upload_package.ps1 on Windows
```

This produces `IbanCheckerOS/IbanCheckerOS_Asset.zip`, which contains the contents of `dotnet publish -c Release -r linux-x64 --self-contained false`. To upload it, go to ODC Portal > External Libraries > Upload new library.

Pushing a `vX.Y.Z` tag runs `.github/workflows/release.yml`, which tests the code, packages it, and creates a GitHub release with `IbanCheckerOS_vX.Y.Z.zip`.

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
