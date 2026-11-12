IbanCheckerOS lets ODC applications validate, parse and format International Bank Account Numbers using the official SWIFT IBAN registry (88 countries).

Key capabilities:
- Validate IBANs, one at a time or up to 5,000 per call, with optional allowed or rejected country lists - get a clear error code for every problem
- Parse an IBAN - get its country, check digits, BBAN, bank and branch identifiers and SEPA membership
- Format an IBAN - get electronic, print, obfuscated or hidden output
- Look up a country or list all supported countries - get IBAN length, examples and SEPA status
- Generate random test IBANs for QA data (never for payments)

Spaces, dashes and lowercase letters are accepted. Invalid input never fails the call. Validation checks format and checksum only, not whether the account exists.

Ideal for apps that capture or import bank details and need reliable, offline IBAN checks without calling an external API.
