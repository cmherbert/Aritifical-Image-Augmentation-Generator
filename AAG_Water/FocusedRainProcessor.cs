﻿using ArtificalAugmentationGenerator.Plugins;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAG_Water
{
    internal class FocusedRainProcessor : AugmentationProcessor<FocusedRainAugmentation>
    {
        public FocusedRainProcessor(FocusedRainAugmentation augmentation) : base(augmentation)
        {
        }

        public override AugmentationProcessorResult ProcessImage(Mat image)
        {
            AugmentationProcessorResult epr = new AugmentationProcessorResult(true);

            int mm = Math.Min(image.Width, image.Height);
            var drops = CommonRain.CreateDrops(random, 0, 0, image.Width, image.Height, properties.Tolerance, properties.Drops, new OpenCvSharp.Size(ScaleSize((int)(mm * 0.9d), true), ScaleSize((int)(mm * 0.9d), false)), new OpenCvSharp.Size(ScaleSize(mm, true), ScaleSize(mm, false)));
            var dropmats = CommonRain.RenderNormalDrops(drops);
            epr.AddLayer(Blur(image));
            epr.AddLayer(CommonRain.RenderRefraction(drops, ref dropmats, image, properties.Refraction, properties.ForegroundBlur));

            return epr;
        }

        private int ScaleSize(int x, bool isWidth)
        {
            if (!isWidth)
                return (int)Math.Round((21.569 * Math.Pow(Math.E, 0.0006 * x)));
            else
                return (int)Math.Round((10.178 * Math.Pow(Math.E, 0.0016 * x)));
        }

        private Mat Blur(Mat m)
        {
            var sz = properties.BackgroundBlur % 2 == 1 ? properties.BackgroundBlur : properties.BackgroundBlur + 1;
            m = m.GaussianBlur(new OpenCvSharp.Size(sz, sz), properties.BackgroundBlurSD);
            return m;
        }
    }
}
