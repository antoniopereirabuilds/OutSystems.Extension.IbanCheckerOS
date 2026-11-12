## What it does

IbanCheckerOS checks, parses and formats International Bank Account Numbers (IBANs) inside your ODC app, using the official SWIFT IBAN registry. It checks length, characters, country structure and check digits, and can restrict accepted countries. No external API or key is needed.

## Actions

| Action | What it does | Key inputs | Key outputs |
|--------|-------------|-----------|-------------|
| ValidateIban | Checks one IBAN, with optional country rules | Iban: Text | Result: IbanValidationResult |
| ValidateIbanBulk | Checks up to 5,000 IBANs in one call | Ibans: Text List, OnlyInvalid: Boolean | Result: IbanBulkValidationResult |
| ParseIban | Splits a valid IBAN into its parts | Iban: Text | Result: IbanParseResult |
| ParseIbanBulk | Parses up to 5,000 IBANs in one call | Ibans: Text List | Result: IbanBulkParseResult |
| FormatIban | Returns Electronic, Print, Obfuscated or Hidden text | Iban: Text, Format: Text | Result: IbanFormatResult |
| FormatIbanBulk | Formats up to 5,000 IBANs with one format | Ibans: Text List, Format: Text | Result: IbanBulkFormatResult |
| GetCountryInfo | Returns the IBAN rules of one country | CountryCode: Text | Result: IbanCountryResult |
| ListSupportedCountries | Lists all supported countries by code | (none) | Countries: IbanCountryInfo List |
| GenerateTestIbans | Creates random valid IBANs for testing only | CountryCode: Text, Count: Integer | Result: IbanGenerateResult |

Both validate actions also accept AllowedCountries and RejectedCountries (two-letter codes). A rejection always wins.

## Structures

| Structure | What it represents | Key fields |
|-----------|--------------------|-----------|
| IbanValidationResult | Outcome of checking one IBAN | IsValid, ErrorCode, CountryCode |
| IbanBulkValidationItem | One checked item in a bulk call | Index, IsValid, ErrorCode |
| IbanBulkValidationResult | Outcome of a bulk check | Success, TotalCount, Results |
| IbanInfo | Parts of a valid IBAN | Print, Bban, BankIdentifier, IsSepaMember |
| IbanParseResult | Outcome of parsing one IBAN | Success, ErrorCode, Info |
| IbanBulkParseItem | One parsed item in a bulk call | Index, Success, Info |
| IbanBulkParseResult | Outcome of a bulk parse | Success, TotalCount, Results |
| IbanFormatResult | Outcome of formatting one IBAN | Success, ErrorCode, Value |
| IbanBulkFormatItem | One formatted item in a bulk call | Index, Success, Value |
| IbanBulkFormatResult | Outcome of a bulk format | Success, TotalCount, Results |
| IbanCountryInfo | IBAN rules of one country | Code, IbanLength, IsSepaMember |
| IbanCountryResult | Outcome of a country lookup | Found, Country |
| IbanGenerateResult | Generated test IBANs | Success, ErrorCode, Ibans |

## How to use

- In ODC Studio, add IbanCheckerOS as a dependency and drag an action (for example ValidateIban) into a server action flow.
- Map the user's input to Iban. Spaces, dashes and lowercase are accepted; the cleaned value is returned in NormalizedIban.
- Add an If on Result.IsValid (or Result.Success). When False, branch on Result.ErrorCode and show Result.ErrorMessage.
- Save NormalizedIban to your entity and show the Print or Obfuscated format on screen.
- For bulk actions, check Result.Success for call-level errors such as TooManyItems, then loop over Result.Results. Index is zero-based.

## Constraints

- Bulk actions accept up to 5,000 items per call. Split larger lists into batches.
- Validation checks format and checksum only. It does not confirm that the bank account exists.
- GenerateTestIbans returns 1 to 1000 IBANs that are not real accounts. Never use them for payments.
- Error messages are in English. Build your logic on ErrorCode, not on ErrorMessage.
- Stateless: each call is independent.
