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
        #region Chart Settings
        /// <summary>
        /// How many slices to show (default 0, leave at 0 to draw infinite slices)
        /// </summary>
        public int MaxSlicesToShow { get; set; } = 0;
        #endregion


        /// <summary>
        /// How much padding to add.
        /// </summary>
        public int Padding { get; set; } = 5;

        #region Legend Settings
        /// <summary>
        /// What size do you want the font to be in the legend?
        /// </summary>
        public int LegendFontSize { get; set; } = 20;

        /// <summary>
        /// The distance between the color box for the legend and the beginning of the chart / the legend label.
        /// </summary>
        public int LegendPadding { get; set; } = 5;

        /// <summary>
        /// Indicates what color you want the legend to be printed as.
        /// </summary>
        public SKColor LegendFontColor { get; set; } = SKColor.Parse("535a60");
        #endregion

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

        private SKPaint GetLegendFontPaint()
        {
            var textPaint = new SKPaint();
            textPaint.Color = SKColors.Black;
            textPaint.Style = SKPaintStyle.Fill;
            textPaint.TextSize = LegendFontSize;
            textPaint.TextAlign = SKTextAlign.Left;
            textPaint.FakeBoldText = true;
            textPaint.IsAntialias = true;
            return textPaint;
        }

        private double DetermineLegendWidth()
        {
            var longestLabel = DataSource.OrderByDescending(d => d.Label.Length).FirstOrDefault();
            var textPaint = GetLegendFontPaint();
            var textWidth = textPaint.MeasureText(longestLabel.Label);
            return textWidth + LegendFontSize + LegendPadding * 2;
        }

        private SKRect GetChartRect(int width, int height)
        {
            var legendWidth = DetermineLegendWidth();
            var chartWidth = width - (Padding * 2) - (float)legendWidth;
            var chartHeight = height - (Padding * 2);

            var box = chartWidth < chartHeight ? chartWidth : chartHeight;

            var yPos = (height - (Padding * 2) - box) / 2f;

            SKRect chartRect = SKRect.Create(Padding, yPos, box, box);
            return chartRect;
        }

        private SKPoint GetLegendTextPosition(int iter, SKRect chartRect)
        {
            var x = Padding + chartRect.Width + LegendPadding*2 + LegendFontSize;
            var y = Padding * 2 + (LegendPadding + (iter * LegendFontSize));


            return new SKPoint(x, y);
        }

        private void drawLegendEntry(SKCanvas canvas, string label, SKColor color, int iter, SKRect chartRect)
        {
            var legendPos = GetLegendTextPosition(iter, chartRect);

            var textPaint = GetLegendFontPaint();


            canvas.DrawText(label, legendPos, textPaint);
            
        }

        protected override void drawChart(SKCanvas canvas, int width, int height)
        {
            var ordered = DataSource.OrderByDescending(d => d.Value).ToList();
            var totalAmount = GetTotalValue();
            var curRad = 0d;

            using (SKPaint paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                canvas.DrawRect(0, 0, width, height, paint);
            }

            SKColor? previousColor = null;
            var chartRect = GetChartRect(width, height);
            bool done = false;
            
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
                    done = true;
                    label = "Other";
                }

                drawSlice(curRad, radian, canvas, color, width, height, chartRect);
                drawLegendEntry(canvas, label, color, i, chartRect);
                
                curRad += radian;
                previousColor = color;
                if (done)
                {
                    break;
                }
            }
        }

        private void drawSlice(double startAngle, double angleAmount, SKCanvas canvas, SKColor color, int width, int height, SKRect chartRect/*string label*/)
        {
            SKPoint center = new SKPoint(chartRect.MidX, chartRect.MidY);
            var finalAngle = startAngle + angleAmount;
            using (var path = new SKPath())
            {
                using (var paint = new SKPaint())
                {
                    path.MoveTo(center);
                    path.ArcTo(chartRect, (float)startAngle, (float)angleAmount, false);
                    path.Close();
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = color;
                    paint.IsAntialias = true;
                    canvas.DrawPath(path, paint);
                }
            }
        }
    }
}
