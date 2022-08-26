using System;

namespace FGUFW.HCT
{
    public static class MathUtils
    {
        public static double Lerp(double start, double stop, double amount)
        {
            return (1.0 - amount) * start + amount * stop;
        }        
        
        public static double Clamp(double min, double max, double input)
        {
            if (input < min)
            {
                return min;
            }
            else if (input > max)
            {
                return max;
            }

            return input;
        }

        public static double Cbrt(double x)
        {
            double y = Math.Pow(Math.Abs(x), 1.0 / 3.0);
            return x < 0.0 ? -y : y;
        }

        public static double Hypot(double a, double b)
        {
            return Math.Sqrt(a * a + b * b);
        }

        public static double Expm1(double a)
        {
            return Math.Exp(a) - 1.0;
        }

        public static double Log1p(double a)
        {
            return Math.Log(a + 1.0);
        }

        /** Multiplies a 1x3 row vector with a 3x3 matrix. */
        public static double[] MatrixMultiply(double[] row, double[][] matrix)
        {
            double a = row[0] * matrix[0][0] + row[1] * matrix[0][1] + row[2] * matrix[0][2];
            double b = row[0] * matrix[1][0] + row[1] * matrix[1][1] + row[2] * matrix[1][2];
            double c = row[0] * matrix[2][0] + row[1] * matrix[2][1] + row[2] * matrix[2][2];
            return new double[] { a, b, c };
        }

        /**
         * Sanitizes a degree measure as a floating-point number.
         *
         * @return a degree measure between 0.0 (inclusive) and 360.0 (exclusive).
         */
        public static double SanitizeDegreesDouble(double degrees)
        {
            degrees = degrees % 360.0;
            if (degrees < 0)
            {
                degrees = degrees + 360.0;
            }
            return degrees;
        }        
        
        /**
         * The signum function.
         *
         * @return 1 if num > 0, -1 if num < 0, and 0 if num = 0
         */
        public static int Signum(double num)
        {
            if (num < 0)
            {
                return -1;
            }
            else if (num == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

    }
}