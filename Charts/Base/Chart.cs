using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Charts.Base
{
    public abstract class Chart<T>
    {
        public T DataSource { get; set; }

        public virtual Stream Generate(int width, int height)
        {
            SKImageInfo imageInfo = new SKImageInfo(width, height);
            using (SKSurface surface = SKSurface.Create(imageInfo))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear();
                drawChart(canvas, width, height);
                var img = surface.Snapshot();
                var data = img.Encode(SKEncodedImageFormat.Png, 100);
                return data.AsStream(false);
            }
        }



        protected abstract void drawChart(SKCanvas canvas, int width, int height);
    }
}
