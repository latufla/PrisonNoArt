namespace Honeylab.Utils.Maths
{
    public static class PrettyRounding
    {
        public static int Round(int source)
        {
            if (source < 10)
            {
                return source;
            }

            if (source < 1000)
            {
                return source - source % 10;
            }

            uint digitsInSource = (uint)IntMath.GetDigitsCount(source);
            const int sectionSize = 3;
            uint digitsInSection = (digitsInSource - 1) % sectionSize;
            uint sectionIndex = digitsInSource / sectionSize;

            int rounded = source - source % IntMath.Pow(10, sectionSize * (sectionIndex - 1));
            rounded -= rounded % IntMath.Pow(10, digitsInSource - digitsInSection - 2);
            return rounded;
        }
    }
}
