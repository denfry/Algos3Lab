using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Algos3Lab
{
    public class BinaryValueValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string input = value as string;
            if (string.IsNullOrEmpty(input))
                return new ValidationResult(false, "Value cannot be empty.");

            if (input.Trim() == "0" || input.Trim() == "1")
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Only '0' or '1' are allowed.");
        }
    }
}
