using Honeylab.Utils.Maths;
using System;
using System.Globalization;


namespace Honeylab.Utils.Formatting
{
    public static class AcronymedPrint
    {
        private const int SectionSize = 3;
        private static readonly string[] SectionPostfixes = { "", "k", "m" };


        public static string ToString(int value) => $"{GetNumberString(value)}{GetNumberPostfix(value)}";


        private static string GetNumberString(int value)
        {
            int sectionIndex = GetSectionIndex(value);
            decimal sectionsCount = value / (decimal)Math.Pow(10.0, sectionIndex * SectionSize);

            for (int i = SectionSize - 1; i >= 0; i--)
            {
                if (sectionsCount >= (decimal)Math.Pow(10.0, i))
                {
                    int accuracyWidth = SectionSize - 1 - i;
                    decimal truncateMultiplier = (decimal)Math.Pow(10.0, accuracyWidth);
                    sectionsCount = Math.Truncate(sectionsCount * truncateMultiplier) / truncateMultiplier;
                    break;
                }
            }

            return sectionsCount.ToString($"G{SectionSize}", CultureInfo.InvariantCulture);
        }


        private static string GetNumberPostfix(int value)
        {
            int sectionIndex = GetSectionIndex(value);
            return SectionPostfixes[sectionIndex];
        }


        private static int GetSectionIndex(int value) => Math.Min((IntMath.GetDigitsCount(value) - 1) / SectionSize,
            SectionPostfixes.Length - 1);
    }
}
