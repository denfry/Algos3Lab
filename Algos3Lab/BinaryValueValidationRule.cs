using System.Globalization;
using System.Windows.Controls;

public class BinaryValueValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string input = value as string;
        if (string.IsNullOrEmpty(input))
            return new ValidationResult(false, "Значение не может быть пустым.");
        if (input == "0" || input == "1")
            return ValidationResult.ValidResult;
        else
            return new ValidationResult(false, "Значение должно быть 0 или 1.");
    }
}