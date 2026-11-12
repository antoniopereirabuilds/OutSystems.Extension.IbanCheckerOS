using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IbanNet;
using IbanNet.Registry;
using IbanNet.Validation.Results;
using IbanNet.Validation.Rules;

namespace IbanCheckerOS
{
    /// <summary>
    /// Implementation of <see cref="IIbanNet"/> backed by IbanNet and the SWIFT IBAN registry.
    /// All actions are stateless; expensive objects (registry, default validator, parser,
    /// country list and rule-specific validators) are cached in static fields.
    /// </summary>
    /// <remarks>
    /// Static caches survive between warm Lambda invocations but never change results: the registry is
    /// read-only, and the rule-specific validator cache is keyed by the normalized allowed/rejected country
    /// lists and cleared when it reaches 128 entries. <c>IbanGenerator</c> is not thread-safe, so a new one
    /// is created per <see cref="GenerateTestIbans"/> call. No public action throws; failures are reported
    /// through the result structures (see <see cref="IbanErrorCodes"/>).
    /// </remarks>
    public class IbanNetService : IIbanNet
    {
        /// <summary>Maximum number of items accepted by the bulk actions.</summary>
        /// <remarks>Larger lists return <see cref="IbanErrorCodes.TooManyItems"/>. Chosen to stay within the ODC 5.5 MB payload limit.</remarks>
        public const int MaxBulkItems = 5000;

        /// <summary>Maximum number of IBANs that <see cref="GenerateTestIbans"/> can produce per call.</summary>
        /// <remarks>A <c>Count</c> outside 1..1000 returns <see cref="IbanErrorCodes.InvalidCount"/>.</remarks>
        public const int MaxGeneratedIbans = 1000;

        private const int MaxCachedValidators = 128;
        private const int MaxGenerateAttemptsPerIban = 10;

        /// <summary>The SWIFT IBAN registry (IbanRegistry.Default uses only the SwiftRegistryProvider).</summary>
        private static readonly IIbanRegistry Registry = IbanRegistry.Default;

        private static readonly IbanValidator DefaultValidator = new(Registry);

        private static readonly IbanParser DefaultParser = new(DefaultValidator);

        private static readonly ConcurrentDictionary<string, IbanValidator> RuleValidators = new(StringComparer.Ordinal);

        private static readonly Lazy<IReadOnlyList<IbanCountryInfo>> SupportedCountries = new(
            () => Registry
                .Select(ToCountryInfo)
                .OrderBy(c => c.Code, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly());

        private static readonly Dictionary<string, IbanFormat> Formats = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Electronic"] = IbanFormat.Electronic,
            ["Print"] = IbanFormat.Print,
            ["Obfuscated"] = IbanFormat.Obfuscated,
            ["Hidden"] = IbanFormat.Hidden
        };

        #region ValidateIban

        /// <inheritdoc />
        public void ValidateIban(string Iban, List<string> AllowedCountries, List<string> RejectedCountries, out IbanValidationResult Result)
        {
            Result = new IbanValidationResult();
            string normalized = string.Empty;
            try
            {
                normalized = Normalize(Iban);
                var validator = GetValidator(AllowedCountries, RejectedCountries);
                Result = Validate(validator, normalized);
            }
            catch (Exception ex)
            {
                Result = new IbanValidationResult
                {
                    NormalizedIban = normalized,
                    ErrorCode = IbanErrorCodes.UnexpectedError,
                    ErrorMessage = UnexpectedMessage(ex)
                };
            }
        }

        /// <inheritdoc />
        public void ValidateIbanBulk(List<string> Ibans, List<string> AllowedCountries, List<string> RejectedCountries, bool OnlyInvalid, out IbanBulkValidationResult Result)
        {
            Result = new IbanBulkValidationResult();
            var input = Ibans ?? new List<string>();
            Result.TotalCount = input.Count;

            if (input.Count > MaxBulkItems)
            {
                Result.Success = false;
                Result.ErrorCode = IbanErrorCodes.TooManyItems;
                Result.ErrorMessage = TooManyItemsMessage(input.Count);
                return;
            }

            IbanValidator? validator = null;
            string validatorError = string.Empty;
            try
            {
                validator = GetValidator(AllowedCountries, RejectedCountries);
            }
            catch (Exception ex)
            {
                validatorError = UnexpectedMessage(ex);
            }

            var results = new List<IbanBulkValidationItem>(OnlyInvalid ? 0 : input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                string raw = input[i] ?? string.Empty;
                string normalized = string.Empty;
                IbanValidationResult single;
                try
                {
                    normalized = Normalize(raw);
                    single = validator is null
                        ? new IbanValidationResult { NormalizedIban = normalized, ErrorCode = IbanErrorCodes.UnexpectedError, ErrorMessage = validatorError }
                        : Validate(validator, normalized);
                }
                catch (Exception ex)
                {
                    single = new IbanValidationResult { NormalizedIban = normalized, ErrorCode = IbanErrorCodes.UnexpectedError, ErrorMessage = UnexpectedMessage(ex) };
                }

                if (single.IsValid)
                {
                    Result.SuccessCount++;
                }
                else
                {
                    Result.FailureCount++;
                }

                if (OnlyInvalid && single.IsValid)
                {
                    continue;
                }

                results.Add(new IbanBulkValidationItem
                {
                    Index = i,
                    Input = raw,
                    NormalizedIban = single.NormalizedIban,
                    IsValid = single.IsValid,
                    ErrorCode = single.ErrorCode,
                    ErrorMessage = single.ErrorMessage,
                    CountryCode = single.CountryCode
                });
            }

            Result.Results = results;
        }

        #endregion

        #region ParseIban

        /// <inheritdoc />
        public void ParseIban(string Iban, out IbanParseResult Result)
        {
            Result = Parse(Iban);
        }

        /// <inheritdoc />
        public void ParseIbanBulk(List<string> Ibans, out IbanBulkParseResult Result)
        {
            Result = new IbanBulkParseResult();
            var input = Ibans ?? new List<string>();
            Result.TotalCount = input.Count;

            if (input.Count > MaxBulkItems)
            {
                Result.Success = false;
                Result.ErrorCode = IbanErrorCodes.TooManyItems;
                Result.ErrorMessage = TooManyItemsMessage(input.Count);
                return;
            }

            var results = new List<IbanBulkParseItem>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                string raw = input[i] ?? string.Empty;
                var single = Parse(raw);
                if (single.Success)
                {
                    Result.SuccessCount++;
                }
                else
                {
                    Result.FailureCount++;
                }

                results.Add(new IbanBulkParseItem
                {
                    Index = i,
                    Input = raw,
                    NormalizedIban = single.NormalizedIban,
                    Success = single.Success,
                    ErrorCode = single.ErrorCode,
                    ErrorMessage = single.ErrorMessage,
                    Info = single.Info
                });
            }

            Result.Results = results;
        }

        #endregion

        #region FormatIban

        /// <inheritdoc />
        public void FormatIban(string Iban, string Format, out IbanFormatResult Result)
        {
            if (!TryGetFormat(Format, out var format))
            {
                Result = new IbanFormatResult
                {
                    NormalizedIban = SafeNormalize(Iban),
                    ErrorCode = IbanErrorCodes.InvalidFormat,
                    ErrorMessage = InvalidFormatMessage(Format)
                };
                return;
            }

            Result = FormatSingle(Iban, format);
        }

        /// <inheritdoc />
        public void FormatIbanBulk(List<string> Ibans, string Format, out IbanBulkFormatResult Result)
        {
            Result = new IbanBulkFormatResult();
            var input = Ibans ?? new List<string>();
            Result.TotalCount = input.Count;

            if (input.Count > MaxBulkItems)
            {
                Result.Success = false;
                Result.ErrorCode = IbanErrorCodes.TooManyItems;
                Result.ErrorMessage = TooManyItemsMessage(input.Count);
                return;
            }

            if (!TryGetFormat(Format, out var format))
            {
                Result.Success = false;
                Result.ErrorCode = IbanErrorCodes.InvalidFormat;
                Result.ErrorMessage = InvalidFormatMessage(Format);
                return;
            }

            var results = new List<IbanBulkFormatItem>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                string raw = input[i] ?? string.Empty;
                var single = FormatSingle(raw, format);
                if (single.Success)
                {
                    Result.SuccessCount++;
                }
                else
                {
                    Result.FailureCount++;
                }

                results.Add(new IbanBulkFormatItem
                {
                    Index = i,
                    Input = raw,
                    NormalizedIban = single.NormalizedIban,
                    Success = single.Success,
                    ErrorCode = single.ErrorCode,
                    ErrorMessage = single.ErrorMessage,
                    Value = single.Value
                });
            }

            Result.Results = results;
        }

        #endregion

        #region Countries

        /// <inheritdoc />
        public void GetCountryInfo(string CountryCode, out IbanCountryResult Result)
        {
            Result = new IbanCountryResult();
            string code = NormalizeCountryCode(CountryCode);
            if (code.Length == 0)
            {
                return;
            }

            if (Registry.TryGetValue(code, out var country) && country is not null)
            {
                Result.Found = true;
                Result.Country = ToCountryInfo(country);
            }
        }

        /// <inheritdoc />
        public void ListSupportedCountries(out List<IbanCountryInfo> Countries)
        {
            Countries = new List<IbanCountryInfo>(SupportedCountries.Value);
        }

        #endregion

        #region GenerateTestIbans

        /// <inheritdoc />
        public void GenerateTestIbans(string CountryCode, int Count, out IbanGenerateResult Result)
        {
            Result = new IbanGenerateResult();
            string code = NormalizeCountryCode(CountryCode);

            if (code.Length == 0)
            {
                Result.ErrorCode = IbanErrorCodes.Empty;
                Result.ErrorMessage = "The country code is required.";
                return;
            }

            if (!Registry.TryGetValue(code, out var country) || country is null)
            {
                Result.ErrorCode = IbanErrorCodes.UnknownCountry;
                Result.ErrorMessage = $"The country code '{code}' is not supported by the SWIFT IBAN registry.";
                return;
            }

            if (Count < 1 || Count > MaxGeneratedIbans)
            {
                Result.ErrorCode = IbanErrorCodes.InvalidCount;
                Result.ErrorMessage = $"Count must be between 1 and {MaxGeneratedIbans}; received {Count}.";
                return;
            }

            try
            {
                // IbanGenerator holds a Random instance and is not thread-safe, so create one per call.
                var generator = new IbanGenerator(Registry);
                var ibans = new List<string>(Count);
                for (int i = 0; i < Count; i++)
                {
                    ibans.Add(GenerateOne(generator, country.TwoLetterISORegionName));
                }

                Result.Success = true;
                Result.Ibans = ibans;
            }
            catch (Exception ex)
            {
                Result = new IbanGenerateResult
                {
                    ErrorCode = IbanErrorCodes.UnexpectedError,
                    ErrorMessage = UnexpectedMessage(ex)
                };
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Normalizes an IBAN input: trims, removes all whitespace and dash characters, uppercases (invariant).
        /// </summary>
        /// <param name="value">The raw input; null is treated as empty.</param>
        /// <returns>The normalized value (never null).</returns>
        internal static string Normalize(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c) || IsDash(c))
                {
                    continue;
                }

                sb.Append(char.ToUpperInvariant(c));
            }

            return sb.ToString();
        }

        private static string SafeNormalize(string? value)
        {
            try
            {
                return Normalize(value);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsDash(char c) =>
            c == '-' || (c >= '‐' && c <= '―') || c == '−' || c == '﹣' || c == '－';

        private static string NormalizeCountryCode(string? value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();

        private static List<string> NormalizeCountryList(List<string>? values)
        {
            if (values is null || values.Count == 0)
            {
                return new List<string>();
            }

            return values
                .Select(NormalizeCountryCode)
                .Where(c => c.Length > 0)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(c => c, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Returns a validator for the given country rules. Without rules the shared default validator is used;
        /// otherwise a validator with <see cref="AcceptCountryRule"/> and/or <see cref="RejectCountryRule"/> is
        /// built and cached by its normalized rule key.
        /// </summary>
        private static IbanValidator GetValidator(List<string>? allowed, List<string>? rejected)
        {
            var allowedList = NormalizeCountryList(allowed);
            var rejectedList = NormalizeCountryList(rejected);

            if (allowedList.Count == 0 && rejectedList.Count == 0)
            {
                return DefaultValidator;
            }

            string key = "A:" + string.Join(",", allowedList) + "|R:" + string.Join(",", rejectedList);
            if (RuleValidators.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var rules = new List<IIbanValidationRule>(2);
            if (allowedList.Count > 0)
            {
                rules.Add(new AcceptCountryRule(allowedList));
            }

            if (rejectedList.Count > 0)
            {
                rules.Add(new RejectCountryRule(rejectedList));
            }

            var validator = new IbanValidator(Registry, rules);

            if (RuleValidators.Count >= MaxCachedValidators)
            {
                RuleValidators.Clear();
            }

            RuleValidators.TryAdd(key, validator);
            return validator;
        }

        private static IbanValidationResult Validate(IbanValidator validator, string normalized)
        {
            var result = new IbanValidationResult { NormalizedIban = normalized };

            if (normalized.Length == 0)
            {
                result.ErrorCode = IbanErrorCodes.Empty;
                result.ErrorMessage = "The IBAN is required.";
                return result;
            }

            ValidationResult vr = validator.Validate(normalized);
            result.CountryCode = vr.Country?.TwoLetterISORegionName ?? string.Empty;

            if (vr.IsValid)
            {
                result.IsValid = true;
                return result;
            }

            (result.ErrorCode, result.ErrorMessage) = MapError(vr.Error, vr.Country, normalized);
            return result;
        }

        /// <summary>
        /// Maps an IbanNet error result type to this library's error code and an English message.
        /// </summary>
        private static (string Code, string Message) MapError(ErrorResult? error, IbanCountry? country, string normalized)
        {
            string countryCode = country?.TwoLetterISORegionName ?? string.Empty;
            return error switch
            {
                InvalidLengthResult when country?.Iban is not null =>
                    (IbanErrorCodes.InvalidLength, $"The IBAN has an incorrect length: expected {country.Iban.Length} characters for {countryCode}, received {normalized.Length}."),
                InvalidLengthResult =>
                    (IbanErrorCodes.InvalidLength, "The IBAN has an incorrect length."),
                IllegalCountryCodeCharactersResult =>
                    (IbanErrorCodes.IllegalCharacters, "The IBAN contains illegal characters in the position where the country code is expected."),
                IllegalCharactersResult =>
                    (IbanErrorCodes.IllegalCharacters, "The IBAN contains illegal characters. Only letters A-Z and digits 0-9 are allowed."),
                InvalidStructureResult =>
                    (IbanErrorCodes.InvalidStructure, $"The IBAN does not match the structure defined for {(countryCode.Length > 0 ? countryCode : "its country")}."),
                InvalidCheckDigitsResult =>
                    (IbanErrorCodes.InvalidCheckDigits, "The IBAN check digits are incorrect."),
                UnknownCountryCodeResult =>
                    (IbanErrorCodes.UnknownCountry, $"The country code '{(normalized.Length >= 2 ? normalized.Substring(0, 2) : normalized)}' is unknown or not supported by the SWIFT IBAN registry."),
                CountryNotAcceptedResult =>
                    (IbanErrorCodes.CountryNotAccepted, $"IBANs from country '{countryCode}' are not accepted."),
                ExceptionResult ex =>
                    (IbanErrorCodes.UnexpectedError, UnexpectedMessage(ex.Exception)),
                null =>
                    (IbanErrorCodes.UnexpectedError, "The IBAN is invalid."),
                _ =>
                    (IbanErrorCodes.UnexpectedError, string.IsNullOrEmpty(error.ErrorMessage) ? "The IBAN is invalid." : error.ErrorMessage)
            };
        }

        private static IbanParseResult Parse(string? input)
        {
            var result = new IbanParseResult();
            string normalized = string.Empty;
            try
            {
                normalized = Normalize(input);
                var validation = Validate(DefaultValidator, normalized);
                result.NormalizedIban = normalized;

                if (!validation.IsValid)
                {
                    result.ErrorCode = validation.ErrorCode;
                    result.ErrorMessage = validation.ErrorMessage;
                    return result;
                }

                if (!DefaultParser.TryParse(normalized, out var iban) || iban is null)
                {
                    result.ErrorCode = IbanErrorCodes.UnexpectedError;
                    result.ErrorMessage = "The IBAN passed validation but could not be parsed.";
                    return result;
                }

                result.Success = true;
                result.Info = ToIbanInfo(iban);
                return result;
            }
            catch (Exception ex)
            {
                return new IbanParseResult
                {
                    NormalizedIban = normalized,
                    ErrorCode = IbanErrorCodes.UnexpectedError,
                    ErrorMessage = UnexpectedMessage(ex)
                };
            }
        }

        private static IbanFormatResult FormatSingle(string? input, IbanFormat format)
        {
            var result = new IbanFormatResult();
            string normalized = string.Empty;
            try
            {
                normalized = Normalize(input);
                var validation = Validate(DefaultValidator, normalized);
                result.NormalizedIban = normalized;

                if (!validation.IsValid)
                {
                    result.ErrorCode = validation.ErrorCode;
                    result.ErrorMessage = validation.ErrorMessage;
                    return result;
                }

                if (!DefaultParser.TryParse(normalized, out var iban) || iban is null)
                {
                    result.ErrorCode = IbanErrorCodes.UnexpectedError;
                    result.ErrorMessage = "The IBAN passed validation but could not be parsed.";
                    return result;
                }

                result.Success = true;
                result.Value = iban.ToString(format);
                return result;
            }
            catch (Exception ex)
            {
                return new IbanFormatResult
                {
                    NormalizedIban = normalized,
                    ErrorCode = IbanErrorCodes.UnexpectedError,
                    ErrorMessage = UnexpectedMessage(ex)
                };
            }
        }

        private static bool TryGetFormat(string? format, out IbanFormat value)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                value = IbanFormat.Electronic;
                return true;
            }

            return Formats.TryGetValue(format.Trim(), out value);
        }

        private static string GenerateOne(IbanGenerator generator, string countryCode)
        {
            for (int attempt = 0; attempt < MaxGenerateAttemptsPerIban; attempt++)
            {
                string candidate = generator.Generate(countryCode).ToString(IbanFormat.Electronic);
                if (DefaultValidator.Validate(candidate).IsValid)
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException($"Unable to generate a valid IBAN for country '{countryCode}'.");
        }

        private static IbanInfo ToIbanInfo(Iban iban)
        {
            string electronic = iban.ToString(IbanFormat.Electronic);
            return new IbanInfo
            {
                Electronic = electronic,
                Print = iban.ToString(IbanFormat.Print),
                Obfuscated = iban.ToString(IbanFormat.Obfuscated),
                CountryCode = iban.Country.TwoLetterISORegionName,
                CountryName = iban.Country.EnglishName ?? string.Empty,
                CheckDigits = electronic.Length >= 4 ? electronic.Substring(2, 2) : string.Empty,
                Bban = iban.Bban ?? string.Empty,
                BankIdentifier = iban.BankIdentifier ?? string.Empty,
                BranchIdentifier = iban.BranchIdentifier ?? string.Empty,
                IsSepaMember = iban.Country.Sepa?.IsMember ?? false
            };
        }

        private static IbanCountryInfo ToCountryInfo(IbanCountry country)
        {
            return new IbanCountryInfo
            {
                Code = country.TwoLetterISORegionName,
                EnglishName = country.EnglishName ?? string.Empty,
                IbanLength = country.Iban?.Length ?? 0,
                IbanExample = country.Iban?.Example ?? string.Empty,
                BbanExample = country.Bban?.Example ?? string.Empty,
                IsSepaMember = country.Sepa?.IsMember ?? false,
                HasBankIdentifier = country.Bank is { Length: > 0 },
                HasBranchIdentifier = country.Branch is { Length: > 0 },
                EffectiveDate = country.EffectiveDate.UtcDateTime
            };
        }

        private static string TooManyItemsMessage(int count) =>
            $"The input contains {count} items; the maximum per call is {MaxBulkItems}. Split the list into smaller batches.";

        private static string InvalidFormatMessage(string? format) =>
            $"The format '{format}' is invalid. Use Electronic, Print, Obfuscated or Hidden.";

        private static string UnexpectedMessage(Exception ex) =>
            $"Unexpected error: {ex.Message}";

        #endregion
    }
}
