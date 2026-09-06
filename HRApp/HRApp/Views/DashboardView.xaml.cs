using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using HRApp.Data;

namespace HRApp.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            Load();
        }

        private class Bar
        {
            public string Department { get; set; }
            public int Count { get; set; }
            public double BarHeight { get; set; }
        }

        private void Load()
        {
            var (total, active, inactive) = DbHelper.GetStatusCounts();
            TxtTotal.Text = total.ToString();
            TxtActive.Text = active.ToString();
            TxtInactive.Text = inactive.ToString();

            var byDept = DbHelper.GetCountsByDepartment();
            TxtDepartments.Text = byDept.Count.ToString();

            const double maxBarHeight = 180;
            int max = byDept.Values.DefaultIfEmpty(1).Max();
            if (max == 0) max = 1;

            var bars = new List<Bar>();
            foreach (var kv in byDept.OrderByDescending(k => k.Value))
            {
                bars.Add(new Bar
                {
                    Department = kv.Key,
                    Count = kv.Value,
                    BarHeight = (kv.Value / (double)max) * maxBarHeight
                });
            }

            ChartItems.ItemsSource = bars;
        }
    }
}
