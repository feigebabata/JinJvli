using System;
using System.Collections.Generic;
using FGUFW.Core;

namespace FGUFW.HCT
{
    /*
    给定大量颜色，删除不适合 UI 主题的颜色，并根据适用性对其余颜色进行排名。
    启用高聚类数进行图像量化，从而确保颜色不会混淆，同时管理高聚类数 要少数适当的选择。
    */
    public static class ScoreUtils
    {
        /// <summary>
        /// 色度大于15
        /// </summary>
        public const double CUTOFF_CHROMA = 15.0;

        /// <summary>
        /// 色调区段(±15)占比大于1%
        /// </summary>
        public const double CUTOFF_EXCITED_PROPORTION = 0.01;

        /// <summary>
        /// 亮度大于10
        /// </summary>
        public const double CUTOFF_TONE = 10.0;

        /// <summary>
        /// 色度边界48
        /// </summary>
        public const double TARGET_CHROMA = 48.0;

        /// <summary>
        /// 色调占比权重
        /// </summary>
        public const double WEIGHT_PROPORTION = 0.7 * 100;

        /// <summary>
        /// 高于色度边界权重
        /// </summary>
        public const double WEIGHT_CHROMA_ABOVE = 0.3;

        /// <summary>
        /// 低于色度边界权重
        /// </summary>
        public const double WEIGHT_CHROMA_BELOW = 0.1;

        /// <summary>
        /// 色调区段半径
        /// </summary>
        public const int HUE_RANGE = 15;

        public static List<int> Score(Dictionary<int, int> colors2Count)
        {
            int colorCount = colors2Count.Count;
            double colorCountSum = 0;
            Dictionary<int,Cam16> color2Cam16 = new Dictionary<int, Cam16>(colorCount);
            double[] hueCount360 = new double[361];

            foreach (var kv in colors2Count)
            {
                int color = kv.Key;
                int count = kv.Value;
                colorCountSum+=count;
                if(!color2Cam16.ContainsKey(color))
                {
                    var cam16 = new Cam16(color,ViewingConditions.DEFAULT);
                    color2Cam16.Add(color,cam16);
                    var hue = (int)Math.Round(cam16.Hue);
                    hueCount360[hue]+=count;
                }
            }

            Dictionary<int,double> color2HueRangeProportion = new Dictionary<int, double>(colorCount);

            foreach (var kv in color2Cam16)
            {
                var color = kv.Key;
                var cam16 = kv.Value;
                int hue = (int)Math.Round(cam16.Hue);

                double hueRangeSum = 0;
                for (int i = hue-HUE_RANGE; i < hue+HUE_RANGE; i++)
                {
                    var idx = (i+360)%360;
                    hueRangeSum += hueCount360[idx];
                }
                double proportion = hueRangeSum/colorCountSum;
                color2HueRangeProportion.Add(color,proportion);
            }

            //过滤 太暗和占比少的颜色
            List<int> filter = new List<int>();
            foreach (var kv in color2Cam16)
            {
                var color = kv.Key;
                var cam16 = kv.Value;
                var proportion = color2HueRangeProportion[color];
                var tone = ColorUtils.LstarFromArgb(color);
                if(cam16.Chroma>CUTOFF_CHROMA && tone>CUTOFF_TONE && proportion >= CUTOFF_EXCITED_PROPORTION )
                {
                    filter.Add(color);
                }
            }

            //评分
            List<Tuple<int,double>> colorScores = new List<Tuple<int,double>>();
            foreach (var color in filter)
            {
                var cam16 = color2Cam16[color];
                double proportion = color2HueRangeProportion[color];

                double proportionScore = proportion * WEIGHT_PROPORTION;

                double chromaWeight = cam16.Chroma<TARGET_CHROMA?WEIGHT_CHROMA_BELOW:WEIGHT_CHROMA_ABOVE;
                double chromaScore = chromaWeight * (cam16.Chroma-TARGET_CHROMA);

                colorScores.Add(new Tuple<int, double>(color,chromaScore+chromaWeight));
            }

            //降序排序
            colorScores.Sort((l,r)=>MathHelper.SortInt(r.Item2-l.Item2));

            List<int> results = new List<int>();
            //过滤相似色调
            foreach (var item in colorScores)
            {
                var newColor = item.Item1;
                var newCam16 = color2Cam16[newColor];
                bool skip = false;
                foreach (var oldColor in results)
                {
                    var oldCam16 = color2Cam16[oldColor];
                    if(Math.Abs(newCam16.Hue-oldCam16.Hue)%180.0<HUE_RANGE)
                    {
                        skip = true;
                        break;
                    }
                }
                if(!skip)
                {
                    results.Add(newColor);
                }
            }

            return results;
        }

    }
}