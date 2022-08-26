using FGUFW.Core;

namespace FGUFW.HCT
{
    /*
    HCT、色相、色度和色调。一种颜色系统，提供感知上准确的颜色测量系统，该系统还可以准确地呈现在不同照明环境中会出现的颜色。
    */
    public struct Hct
    {
        /// <summary>
        /// 0 <=Hct< 360 一个数字，以度为单位，代表 ex。红色、橙色、黄色等。范围从 
        /// </summary>
        public double Hue;

        /// <summary>
        /// 0 <= newChroma<= 150 色度可能会降低，因为色度对于任何给定的色调和色调都有不同的最大值。
        /// </summary>
        public double Chroma;

        /// <summary>
        /// 0 <= tone <= 100
        /// </summary>
        public double Tone;
        public int Argb;

        public Hct(int argb)
        {
            this.Argb = argb;
            Cam16 cam = new Cam16(argb,ViewingConditions.DEFAULT);
            this.Hue = cam.Hue;
            this.Chroma = cam.Chroma;
            this.Tone = ColorUtils.LstarFromArgb(argb);
        }

        /**
         * Create an HCT color from hue, chroma, and tone.
         *
         * @param hue 0 <= hue < 360; invalid values are corrected.
         * @param chroma 0 <= chroma < ?; Informally, colorfulness. The color returned may be lower than
         *     the requested chroma. Chroma has a different maximum for any given hue and tone.
         * @param tone 0 <= tone <= 100; invalid values are corrected.
         * @return HCT representation of a color in default viewing conditions.
         */
        public Hct(double hue, double chroma, double tone)
        {
            int argb = HctSolver.solveToInt(hue, chroma, tone);
            var color = ColorHelper.FromARGBInt(argb);
            var val = new Hct(argb);
            this.Argb = val.Argb;
            this.Hue = val.Hue;
            this.Chroma = val.Chroma;
            this.Tone = val.Tone;
        }
    }
}