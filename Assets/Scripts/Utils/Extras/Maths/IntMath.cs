namespace Honeylab.Utils.Maths
{
    public static class IntMath
    {
        public static int GetDigitsCount(int value)
        {
            int result = 0;
            do
            {
                value /= 10;
                result++;
            } while (value > 0);

            return result;
        }


        // https://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
        public static int Pow(int x, uint pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                {
                    ret *= x;
                }

                x *= x;
                pow >>= 1;
            }

            return ret;
        }
    }
}
