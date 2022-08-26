using System;
using FGUFW.Core;

namespace FGUFW.HCT
{
    /*
    在传统色彩空间中，可以仅通过观察者对颜色的测量来识别颜色。CAM16 等颜色外观模型也使用有关观察颜色的环境的信息，称为观察条件。

    例如，在正午太阳白点的传统假设下，CAM16 将白色准确测量为略带彩色的蓝色。（大致，色调 203，色度 3，亮度 100）

    此类缓存仅取决于观看条件的 CAM16 转换过程的中间值，从而加快速度。
    */
    public struct ViewingConditions:IStruct
    {
        public double Aw;
        public double Nbb;
        public double Ncb;
        public double C;
        public double Nc;
        public double N;
        public double[] RgbD;
        public double Fl;
        public double FlRoot;
        public double Z;

        public readonly static ViewingConditions DEFAULT = new ViewingConditions(new double[] {ColorUtils.WHITE_POINT_D65[0],ColorUtils.WHITE_POINT_D65[1],ColorUtils.WHITE_POINT_D65[2]},(200.0 / Math.PI * ColorUtils.YFromLstar(50.0) / 100.0f),50.0,2.0,false);

        public bool IsCreated { get; set; }

        public ViewingConditions(double[] whitePoint, double adaptingLuminance, double backgroundLstar, double surround, bool discountingIlluminant)
        {
            // Transform white point XYZ to 'cone'/'rgb' responses
            double[][] matrix = Cam16.XYZ_TO_CAM16RGB;
            double[] xyz = whitePoint;
            double rW = (xyz[0] * matrix[0][0]) + (xyz[1] * matrix[0][1]) + (xyz[2] * matrix[0][2]);
            double gW = (xyz[0] * matrix[1][0]) + (xyz[1] * matrix[1][1]) + (xyz[2] * matrix[1][2]);
            double bW = (xyz[0] * matrix[2][0]) + (xyz[1] * matrix[2][1]) + (xyz[2] * matrix[2][2]);
            double f = 0.8 + (surround / 10.0);
            double c = (f >= 0.9) ? MathUtils.Lerp(0.59, 0.69, ((f - 0.9) * 10.0)) : MathUtils.Lerp(0.525, 0.59, ((f - 0.8) * 10.0));
            double d = discountingIlluminant ? 1.0 : f * (1.0 - ((1.0 / 3.6) * Math.Exp((-adaptingLuminance - 42.0) / 92.0)));
            d = MathUtils.Clamp(0.0, 1.0, d);
            double nc = f;
            double[] rgbD = new double[] { d * (100.0 / rW) + 1.0 - d, d * (100.0 / gW) + 1.0 - d, d * (100.0 / bW) + 1.0 - d };
            double k = 1.0 / (5.0 * adaptingLuminance + 1.0);
            double k4 = k * k * k * k;
            double k4F = 1.0 - k4;
            double fl = (k4 * adaptingLuminance) + (0.1 * k4F * k4F * MathUtils.Cbrt(5.0 * adaptingLuminance));
            double n = (ColorUtils.YFromLstar(backgroundLstar) / whitePoint[1]);
            double z = 1.48 + Math.Sqrt(n);
            double nbb = 0.725 / Math.Pow(n, 0.2);
            double ncb = nbb;
            double[] rgbAFactors = new double[] { Math.Pow(fl * rgbD[0] * rW / 100.0, 0.42), Math.Pow(fl * rgbD[1] * gW / 100.0, 0.42), Math.Pow(fl * rgbD[2] * bW / 100.0, 0.42) };

            double[] rgbA = new double[] { (400.0 * rgbAFactors[0]) / (rgbAFactors[0] + 27.13), (400.0 * rgbAFactors[1]) / (rgbAFactors[1] + 27.13), (400.0 * rgbAFactors[2]) / (rgbAFactors[2] + 27.13) };

            double aw = ((2.0 * rgbA[0]) + rgbA[1] + (0.05 * rgbA[2])) * nbb;

            this.N = n;
            this.Aw = aw;
            this.Nbb = nbb;
            this.Ncb = ncb;
            this.C = c;
            this.Nc = nc;
            this.RgbD = rgbD;
            this.Fl = fl;
            this.FlRoot = Math.Pow(fl, 0.25);
            this.Z = z;

            IsCreated = true;
        }
    }
}