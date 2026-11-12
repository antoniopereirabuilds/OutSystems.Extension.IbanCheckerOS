# IbanCheckerOS

ODC External Library wrapping [IbanNet](https://www.nuget.org/packages/IbanNet) 6.2.1 (SWIFT IBAN registry).
Exposes the `IbanCheckerOS` action group: ValidateIban, ValidateIbanBulk, ParseIban, ParseIbanBulk,
FormatIban, FormatIbanBulk, GetCountryInfo, ListSupportedCountries, GenerateTestIbans.

Build the upload package with `./generate_upload_package.sh` (macOS/Linux) or
`./generate_upload_package.ps1` (Windows) and upload `IbanCheckerOS_Asset.zip` in ODC Portal.
See the repository README for the full action reference.
