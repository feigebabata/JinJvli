using System;
using UnityEngine;

namespace FGUFW.HCT
{
    public static class ColorUtils
    {
        public readonly static double[] WHITE_POINT_D65 = new double[] { 95.047, 100.0, 108.883 };
        public readonly static double[][] SRGB_TO_XYZ = new double[][] { new double[] { 0.41233895, 0.35762064, 0.18051042 }, new double[] { 0.2126, 0.7152, 0.0722 }, new double[] { 0.01932141, 0.11916382, 0.95034478 }, };

        /**
         * Converts an L* value to a Y value.
         *
         * <p>L* in L*a*b* and Y in XYZ measure the same quantity, luminance.
         *
         * <p>L* measures perceptual luminance, a linear scale. Y in XYZ measures relative luminance, a
         * logarithmic scale.
         *
         * @param lstar L* in L*a*b*
         * @return Y in XYZ
         */
        public static double YFromLstar(double lstar)
        {
            return 100.0 * labInvf((lstar + 16.0) / 116.0);
        }

        private static double labInvf(double ft)
        {
            double e = 216.0 / 24389.0;
            double kappa = 24389.0 / 27.0;
            double ft3 = ft * ft * ft;
            if (ft3 > e)
            {
                return ft3;
            }
            else
            {
                return (116 * ft - 16) / kappa;
            }
        }

        /**
        * Linearizes an RGB component.
        *
        * @param rgbComponent 0 <= rgb_component <= 255, represents R/G/B channel
        * @return 0.0 <= output <= 100.0, color channel converted to linear RGB space
        */
        public static double Linearized(int rgbComponent)
        {
            double normalized = rgbComponent / 255.0;
            if (normalized <= 0.040449936)
            {
                return normalized / 12.92 * 100.0;
            }
            else
            {
                return Math.Pow((normalized + 0.055) / 1.055, 2.4) * 100.0;
            }
        }

        /**
         * Computes the L* value of a color in ARGB representation.
         *
         * @param argb ARGB representation of a color
         * @return L*, from L*a*b*, coordinate of the color
         */
        public static double LstarFromArgb(int argb)
        {
            double y = xyzFromArgb(argb)[1];
            return 116.0 * labF(y / 100.0) - 16.0;
        }

        /** Converts a color from XYZ to ARGB. */
        public static double[] xyzFromArgb(int argb)
        {
            double r = Linearized(redFromArgb(argb));
            double g = Linearized(greenFromArgb(argb));
            double b = Linearized(blueFromArgb(argb));
            return MathUtils.MatrixMultiply(new double[] { r, g, b }, SRGB_TO_XYZ);
        }

        private static double labF(double t)
        {
            double e = 216.0 / 24389.0;
            double kappa = 24389.0 / 27.0;
            if (t > e)
            {
                return Math.Pow(t, 1.0 / 3.0);
            }
            else
            {
                return (kappa * t + 16) / 116;
            }
        }

        /** Returns the red component of a color in ARGB format. */
        public static int redFromArgb(int argb)
        {
            return (argb >> 16) & 255;
        }

        /** Returns the green component of a color in ARGB format. */
        public static int greenFromArgb(int argb)
        {
            return (argb >> 8) & 255;
        }

        /** Returns the blue component of a color in ARGB format. */
        public static int blueFromArgb(int argb)
        {
            return argb & 255;
        }

        /**
         * Converts an L* value to an ARGB representation.
         *
         * @param lstar L* in L*a*b*
         * @return ARGB representation of grayscale color with lightness matching L*
         */
        public static int ArgbFromLstar(double lstar)
        {
            double y = YFromLstar(lstar);
            int component = Delinearized(y);
            return ArgbFromRgb(component, component, component);
        }        
        
        public static int ArgbFromRgb(int red, int green, int blue)
        {
            return (255 << 24) | ((red & 255) << 16) | ((green & 255) << 8) | (blue & 255);
        }

        /** Converts a color from linear RGB components to ARGB format. */
        public static int ArgbFromLinrgb(double[] linrgb)
        {
            int r = Delinearized(linrgb[0]);
            int g = Delinearized(linrgb[1]);
            int b = Delinearized(linrgb[2]);
            return ArgbFromRgb(r, g, b);
        }

        /**
         * Delinearizes an RGB component.
         *
         * @param rgbComponent 0.0 <= rgb_component <= 100.0, represents linear R/G/B channel
         * @return 0 <= output <= 255, color channel converted to regular RGB space
         */
        public static int Delinearized(double rgbComponent)
        {
            double normalized = rgbComponent / 100.0;
            double delinearized = 0.0;
            if (normalized <= 0.0031308)
            {
                delinearized = normalized * 12.92;
            }
            else
            {
                delinearized = 1.055 * Math.Pow(normalized, 1.0 / 2.4) - 0.055;
            }
            var v = (int)Math.Round(delinearized * 255.0);
            return Mathf.Clamp(v ,0, 255);
        }

    }
}