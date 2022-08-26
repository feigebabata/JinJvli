using System;
using UnityEngine;

namespace FGUFW.HCT
{
    /*
    CAM16，颜色外观模型。颜色不仅仅由其十六进制代码定义，而是由十六进制代码和查看条件定义。

    CAM16 实例在 CAM16-UCS 空间中也有坐标，在代码中称为 J*、a*、b* 或 jstar、astar、bstar。CAM16-UCS 包含在 CAM16 规范中，应在测量颜色之间的距离时使用。

    在传统色彩空间中，可以仅通过观察者对颜色的测量来识别颜色。CAM16 等颜色外观模型也使用有关观察颜色的环境的信息，称为观察条件。

    例如，在正午太阳白点的传统假设下，CAM16 将白色准确测量为略带彩色的蓝色。（大致，色相203，色度3，明度100）CAM16，色相模型。颜色不仅仅由其十六进制代码定义，而是由十六进制代码和查看条件定义。

    CAM16 实例在 CAM16-UCS 空间中也有坐标，在代码中称为 J*、a*、b* 或 jstar、astar、bstar。CAM16-UCS 包含在 CAM16 规范中，应在测量颜色之间的距离时使用。

    在传统色彩空间中，可以仅通过观察者对颜色的测量来识别颜色。CAM16 等颜色外观模型也使用有关观察颜色的环境的信息，称为观察条件。

    例如，在正午太阳白点的传统假设下，CAM16 将白色准确测量为略带彩色的蓝色。（大致，色调 203，色度 3，亮度 100）
    */
    public struct Cam16
    {
        public static readonly double[][] XYZ_TO_CAM16RGB = new double[][]{new double[]{0.401288, 0.650173, -0.051461},new double[]{-0.250268, 1.204414, 0.045854},new double[]{-0.002079, 0.048952, 0.953127}};
        public static readonly double[][] CAM16RGB_TO_XYZ = new double[][]{new double[]{1.8620678, -1.0112547, 0.14918678},new double[]{0.38752654, 0.62144744, -0.00897398},new double[]{-0.01584150, -0.03412294, 1.0499644}};

        /// <summary>
        /// 比如红色、橙色、黄色、绿色等。
        /// </summary>
        public double Hue;

        /// <summary>
        /// 非正式地，色彩/颜色强度。与 HSL 中的饱和度类似，但在感知上是准确的。
        /// </summary>
        public double Chroma;
        public double J;

        /// <summary>
        /// 亮度; 亮度与白点亮度之比
        /// </summary>
        public double Q;
        public double M;
        public double S;

        // Coordinates in UCS space. Used to determine color distance, like delta E equations in L*a*b*.
        public double Jstar;
        public double Astar;
        public double Bstar;


        /**
         * Create a CAM16 color from a color in defined viewing conditions.
         *
         * @param argb ARGB representation of a color.
         * @param viewingConditions Information about the environment where the color was observed.
         */
        // The RGB => XYZ conversion matrix elements are derived scientific constants. While the values
        // may differ at runtime due to floating point imprecision, keeping the values the same, and
        // accurate, across implementations takes precedence.
        // @SuppressWarnings("FloatingPointLiteralPrecision")
        public Cam16(int argb, ViewingConditions viewingConditions)
        {
            if(!viewingConditions.IsCreated)viewingConditions = ViewingConditions.DEFAULT;
            // Transform ARGB int to XYZ
            int red = (argb & 0x00ff0000) >> 16;
            int green = (argb & 0x0000ff00) >> 8;
            int blue = (argb & 0x000000ff);
            double redL = ColorUtils.Linearized(red);
            double greenL = ColorUtils.Linearized(green);
            double blueL = ColorUtils.Linearized(blue);
            double x = 0.41233895 * redL + 0.35762064 * greenL + 0.18051042 * blueL;
            double y = 0.2126 * redL + 0.7152 * greenL + 0.0722 * blueL;
            double z = 0.01932141 * redL + 0.11916382 * greenL + 0.95034478 * blueL;

            // Transform XYZ to 'cone'/'rgb' responses
            double[][] matrix = XYZ_TO_CAM16RGB;
            double rT = (x * matrix[0][0]) + (y * matrix[0][1]) + (z * matrix[0][2]);
            double gT = (x * matrix[1][0]) + (y * matrix[1][1]) + (z * matrix[1][2]);
            double bT = (x * matrix[2][0]) + (y * matrix[2][1]) + (z * matrix[2][2]);

            // Discount illuminant
            double rD = viewingConditions.RgbD[0] * rT;
            double gD = viewingConditions.RgbD[1] * gT;
            double bD = viewingConditions.RgbD[2] * bT;

            // Chromatic adaptation
            double rAF = Math.Pow(viewingConditions.Fl * Math.Abs(rD) / 100.0, 0.42);
            double gAF = Math.Pow(viewingConditions.Fl * Math.Abs(gD) / 100.0, 0.42);
            double bAF = Math.Pow(viewingConditions.Fl * Math.Abs(bD) / 100.0, 0.42);
            double rA = Math.Sign(rD) * 400.0 * rAF / (rAF + 27.13);
            double gA = Math.Sign(gD) * 400.0 * gAF / (gAF + 27.13);
            double bA = Math.Sign(bD) * 400.0 * bAF / (bAF + 27.13);

            // redness-greenness
            double a = (11.0 * rA + -12.0 * gA + bA) / 11.0;
            // yellowness-blueness
            double b = (rA + gA - 2.0 * bA) / 9.0;

            // auxiliary components
            double u = (20.0 * rA + 20.0 * gA + 21.0 * bA) / 20.0;
            double p2 = (40.0 * rA + 20.0 * gA + bA) / 20.0;

            // hue
            double atan2 = Math.Atan2(b, a);
            // double atanDegrees = Math.toDegrees(atan2);
            double atanDegrees = Mathf.Rad2Deg * atan2;
            double hue = atanDegrees < 0 ? atanDegrees + 360.0 : atanDegrees >= 360 ? atanDegrees - 360.0 : atanDegrees;
            // double hueRadians = Math.toRadians(hue);
            double hueRadians = Mathf.Deg2Rad * hue;

            // achromatic response to color
            double ac = p2 * viewingConditions.Nbb;

            // CAM16 lightness and brightness
            double j = 100.0 * Math.Pow( ac / viewingConditions.Aw, viewingConditions.C * viewingConditions.Z);
            double q = 4.0 / viewingConditions.C * Math.Sqrt(j / 100.0) * (viewingConditions.Aw + 4.0) * viewingConditions.FlRoot;

            // CAM16 chroma, colorfulness, and saturation.
            double huePrime = (hue < 20.14) ? hue + 360 : hue;
            double eHue = 0.25 * (Math.Cos(Mathf.Deg2Rad * huePrime + 2.0) + 3.8);
            double p1 = 50000.0 / 13.0 * eHue * viewingConditions.Nc * viewingConditions.Ncb;
            double t = p1 * MathUtils.Hypot(a, b) / (u + 0.305);
            double alpha = Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73) * Math.Pow(t, 0.9);
            // CAM16 chroma, colorfulness, saturation
            double c = alpha * Math.Sqrt(j / 100.0);
            double m = c * viewingConditions.FlRoot;
            double s = 50.0 * Math.Sqrt((alpha * viewingConditions.C) / (viewingConditions.Aw + 4.0));

            // CAM16-UCS components
            double jstar = (1.0 + 100.0 * 0.007) * j / (1.0 + 0.007 * j);
            double mstar = 1.0 / 0.0228 * MathUtils.Log1p(0.0228 * m);
            double astar = mstar * Math.Cos(hueRadians);
            double bstar = mstar * Math.Sin(hueRadians);

            this.Hue = hue;
            this.Chroma = c;
            this.J = j;
            this.Q = q;
            this.M = m;
            this.S = s;
            this.Jstar = jstar;
            this.Astar = astar;
            this.Bstar = bstar;
        }
    }
}