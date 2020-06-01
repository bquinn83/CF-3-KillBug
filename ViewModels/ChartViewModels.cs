using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KillBug.ViewModels
{
    public class ChartOptions
    {
        public List<string> BackgroundColors = new List<string>()
            {
                "rgba(255, 99, 132, 0.2)",
                "rgba(54, 162, 235, 0.2)",
                "rgba(255, 206, 86, 0.2)",
                "rgba(219, 133, 20, 0.2)",
                "rgba(75, 192, 192, 0.2)",
                "rgba(153, 102, 255, 0.2)",
                "rgba(255, 159, 64, 0.2)"
            };
        public List<string> BorderColors = new List<string>()
            {
                "rgba(255, 99, 132, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(219, 133, 20, 1)",
                "rgba(75, 192, 192, 1)",
                "rgba(153, 102, 255, 1)",
                "rgba(255, 159, 64, 1)"
            };
        public int BorderWidth = 1;
    }
    public class ChartData
    {
        public string KeyLabel { get; set; }
        public List<int> Data { get; set; }
        public List<string> Labels { get; set; }
        public ChartOptions Options { get; set; }

        public ChartData()
        {
            Data = new List<int>();
            Labels = new List<string>();
            Options = new ChartOptions();
        }
    }
}