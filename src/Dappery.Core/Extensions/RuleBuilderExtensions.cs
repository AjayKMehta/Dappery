using System.Text.RegularExpressions;

using FluentValidation;

namespace Dappery.Core.Extensions;

public static partial class RuleBuilderExtensions
{
    [GeneratedRegex("\\d{1,5}\\s(\\b\\w*\\b\\s){1,2}\\w*\\.")]
    private static partial Regex StreetAddressRegex();

    [GeneratedRegex("^((A[LKZR])|(C[AOT])|(D[EC])|(FL)|(GA)|(HI)|(I[DLNA])|(K[SY])|(LA)|(M[EDAINSOT])|(N[EVHJMYCD])|(O[HKR])|(PA)|(RI)|(S[CD])|(T[NX])|(UT)|(V[TA])|(W[AVIY]))$")]
    private static partial Regex ValidStateRegex();

    [GeneratedRegex("^\\d{5}$")]
    private static partial Regex ZipCodeRegex();

    public static void NotNullOrEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        _ = ruleBuilder.Custom((stringToValidate, context) =>
          {
              if (string.IsNullOrWhiteSpace(stringToValidate))
                  context.AddFailure($"{context.PropertyName} cannot be null, or empty");
          });
    }

    public static void HasValidStateAbbreviation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        _ = ruleBuilder.Custom((stateAbbreviation, context) =>
          {
              if (stateAbbreviation is not null && !ValidStateRegex().IsMatch(stateAbbreviation))
                  context.AddFailure($"{stateAbbreviation} is not a valid state code");
          })
            .NotEmpty()
            .WithMessage("State code cannot be empty");
    }

    public static void HasValidStreetAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        _ = ruleBuilder.Custom((streetAddress, context) =>
          {
              if (string.IsNullOrWhiteSpace(streetAddress))
              {
                  // Add the context failure and break out of the validation
                  context.AddFailure("Must supply a street address");
                  return;
              }

              if (!StreetAddressRegex().IsMatch(streetAddress))
                  context.AddFailure($"{streetAddress} is not a valid street address");
          });
    }

    public static void HasValidZipCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        _ = ruleBuilder.Custom((zipCode, context) =>
          {
              if (string.IsNullOrWhiteSpace(zipCode))
              {
                  // Add the context failure and break out of the validation
                  context.AddFailure("Must supply the zip code");
                  return;
              }

              if (!ZipCodeRegex().IsMatch(zipCode))
                  context.AddFailure($"{zipCode} is not a valid zipcode");
          });
    }
}
