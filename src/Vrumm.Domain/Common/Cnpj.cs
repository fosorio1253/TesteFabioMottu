using System.Text.RegularExpressions;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Common;
public sealed class Cnpj
{
    private readonly string _value;
    private static readonly Regex CnpjRegex = new(@"^\d{14}$", RegexOptions.Compiled);

    private Cnpj(string value)
    {
        _value = value;
    }

    public static Cnpj Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CNPJ cannot be empty.");

        var normalizedCnpj = Regex.Replace(value, "[^0-9]", "").Trim();

        if (!CnpjRegex.IsMatch(normalizedCnpj))
            throw new DomainException("CNPJ must contain 14 digits.");

        if (!IsValidCnpj(normalizedCnpj))
            throw new DomainException("CNPJ has invalid check digits.");

        return new Cnpj(normalizedCnpj);
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            return false;

        int[] weightsFirst = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += int.Parse(cnpj[i].ToString()) * weightsFirst[i];
        int firstDigit = 11 - (sum % 11);
        if (firstDigit >= 10) firstDigit = 0;

        if (firstDigit != int.Parse(cnpj[12].ToString()))
            return false;

        int[] weightsSecond = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += int.Parse(cnpj[i].ToString()) * weightsSecond[i];
        int secondDigit = 11 - (sum % 11);
        if (secondDigit >= 10) secondDigit = 0;

        return secondDigit == int.Parse(cnpj[13].ToString());
    }

    public string Value => _value;

    public override bool Equals(object? obj) =>
        obj is Cnpj cnpj && _value == cnpj._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;
}