using System.Globalization;
using System.Windows.Controls;

namespace Zarp.GUI.ValidationRules
{
    internal class IntegerRule : ValidationRule
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int intValue;

            try
            {
                intValue = int.Parse((string)value);
            }
            catch
            {
                return new ValidationResult(false, null);
            }

            if ((Min == null && intValue < Min) || (Max == null && intValue > Max))
            {
                return new ValidationResult(false, null);
            }

            return ValidationResult.ValidResult;
        }
    }
}
