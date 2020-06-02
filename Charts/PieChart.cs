using Charts.Base;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Charts
{
    public class PieChartDataPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }

    public class PieChart : Chart<List<PieChartDataPoint>>
    {
        /// <summary>
        /// How many slices to show (default 0, leave at 0 to draw infinite slices)
        /// </summary>
        public int MaxSlicesToShow { get; set; } = 0;

        /// <summary>
        /// How much padding to add.
        /// </summary>
        public int Padding { get; set; } = 0;

        /// <summary>
        /// Default Color Palette. You can Clear this list, set your own hex values, add to it, etc.
        /// </summary>
        public List<string> ColorPaletteHex { get; set; } = new List<string>
        {
            "ADF7FF",
            "6EEB83",
            "E4FF1A",
            "FFB800",
            "FF5714",
            "1BC537",
            "B88400",
            "0F6C1E",
            "FFB699",
            "001214"
        };

        private double GetTotalValue()
        {
            return DataSource.Sum(d => d.Value);
        }

        protected override void drawChart(SKCanvas canvas, int width, int height)
        {
            var ordered = DataSource.OrderByDescending(d => d.Value).ToList();
            var totalAmount = GetTotalValue();
            var curRad = 0d;
            SKColor? previousColor = null;
            
            for (var i = 0; i < ordered.Count; i++)
            {
                
                var el = ordered[i];
                var label = el.Label;
                var radian = (el.Value / totalAmount) * 360.0;
                var colorHex = ColorPaletteHex[i % ColorPaletteHex.Count];
                var color = SKColor.Parse(colorHex);
                if (i > 0 && i+1 == MaxSlicesToShow)
                {
                    var remainingElements = ordered.GetRange(i, ordered.Count - i - 1);
                    var total = remainingElements.Sum(d => d.Value);
                    radian = (total / totalAmount) * 360.0;
                    if(radian + curRad != 0)
                    {
                        radian = 360 - curRad;
                    }
                    i = ordered.Count;
                    label = "Other";
                }

                drawSlice(curRad, radian, canvas, color, width, height, label);
                
                curRad += radian;
                previousColor = color;
            }
        }

        private void drawSlice(double startAngle, double angleAmount, SKCanvas canvas, SKColor color, int width, int height, string label)
        {
            SKPoint center = new SKPoint(width / 2.0f, height / 2.0f);
            SKRect rect = new SKRect(Padding, Padding, width - Padding, height - Padding);
            var finalAngle = startAngle + angleAmount;
            using (var path = new SKPath())
            {
                using (var paint = new SKPaint())
                {
                    path.MoveTo(center);
                    path.ArcTo(rect, (float)startAngle, (float)angleAmount, false);
                    path.Close();
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = color;
                    paint.IsAntialias = true;
                    canvas.DrawPath(path, paint);

                    var textPaint = new SKPaint();
                    textPaint.Color = SKColors.Black;
                    textPaint.Style = SKPaintStyle.Fill;
                    textPaint.TextSize = 15;
                    textPaint.TextAlign = SKTextAlign.Center;
                    textPaint.FakeBoldText = true;
                    textPaint.IsAntialias = true;
                    var textWidth = textPaint.MeasureText(label);
                    if(textWidth > path.Bounds.Width)
                    {
                        var obj = new object();
                    }
                    var degrees = (angleAmount / 2f);
                    SKPoint middleOfSlice = new SKPoint(path.Bounds.MidX, path.Bounds.MidY);
                    canvas.DrawText(label, middleOfSlice, textPaint);
                }
            }
        }
    }
}
