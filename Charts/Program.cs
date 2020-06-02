using System;
using System.IO;

namespace Charts
{
    class Program
    {
        static void Main(string[] args)
        {
            var pc = new PieChart();
            pc.DataSource = new System.Collections.Generic.List<PieChartDataPoint>
            {
                new PieChartDataPoint
                {
                    Label = "Flower",
                    Value = 805.23
                }
                ,new PieChartDataPoint
                {
                    Label = "Pre-Roll",
                    Value = 400
                }
                ,new PieChartDataPoint
                {
                    Label = "Misc",
                    Value = 23.94
                }
                ,new PieChartDataPoint
                {
                    Label = "Topicals",
                    Value = 198.23
                }
                ,new PieChartDataPoint
                {
                    Label = "Edibles",
                    Value = 600.23
                }
                ,new PieChartDataPoint
                {
                    Label = "Vapes",
                    Value = 2457.23
                }
                ,new PieChartDataPoint
                {
                    Label = "Pet",
                    Value = 54.04
                }
            };
            pc.MaxSlicesToShow = 5;

            var ms = pc.Generate(500, 500);
            var fName = Guid.NewGuid().ToString();
            var fPath = $@"C:\tmp\{fName}.png";
            FileStream file = new FileStream(fPath, FileMode.Create, FileAccess.Write);
            ms.CopyTo(file);
            Console.WriteLine($"Image generated at {fPath}");
        }
    }
}
