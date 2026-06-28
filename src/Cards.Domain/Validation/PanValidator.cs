namespace Cards.Domain.Validation;

/// <summary>
/// Validates card PAN: 13–19 digits with Luhn check (ISO/IEC 7812).
/// </summary>
public static class PanValidator
{
    public static string Normalize(string value) =>
        new(value.Where(char.IsDigit).ToArray());

    public static bool IsValid(string? value)
    {
        var pan = Normalize(value ?? "");
        return pan.Length is >= 13 and <= 19 && LuhnSum(pan) == 0;
    }

    internal static string WithCheckDigit(string digitsWithoutCheck)
    {
        var check = (10 - (LuhnSum(digitsWithoutCheck + "0") % 10)) % 10;
        return digitsWithoutCheck + check;
    }

    private static int LuhnSum(string digits)
    {
        var sum = 0;
        var doubleDigit = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var n = digits[i] - '0';
            if (doubleDigit)
            {
                n *= 2;
                if (n > 9)
                    n -= 9;
            }

            sum += n;
            doubleDigit = !doubleDigit;
        }

        return sum % 10;
    }
}
