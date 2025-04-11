using System;
using System.Globalization;
using UnityEngine;


namespace Honeylab.Utils.Formatting
{
    public class AcronymedPrintCustomFormatter : IFormatProvider, ICustomFormatter
    {
        public static readonly AcronymedPrintCustomFormatter Instance = new AcronymedPrintCustomFormatter();


        object IFormatProvider.GetFormat(Type formatType) => typeof(ICustomFormatter) == formatType ? this : null;


        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(format) && format.Equals("ACR", StringComparison.InvariantCulture))
            {
                int toStringArg = arg switch
                {
                    int intNumber => intNumber,
                    float floatNumber => Mathf.RoundToInt(floatNumber),
                    double doubleNumber => (int)Math.Round(doubleNumber),
                    var _ => throw new FormatException($"Argument {arg} must be of (int/float/double) type.")
                };

                return AcronymedPrint.ToString(toStringArg);
            }

            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }

            return arg.ToString();
        }
    }
}
