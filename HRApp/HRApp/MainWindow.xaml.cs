using System;
using System.Linq;
using System.Windows;
using HRApp.Views;

namespace HRApp
{
    public partial class MainWindow : Window
    {
        private bool _isArabic = false;

        public MainWindow()
        {
            InitializeComponent();
            ShowDashboard_Click(null, null);
        }

        private void ShowDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardView();
        }

        private void ShowEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EmployeesView();
        }

        private void ToggleLanguage_Click(object sender, RoutedEventArgs e)
        {
            _isArabic = !_isArabic;

            // Swap the merged string dictionary
            var dictUri = _isArabic
                ? "Localization/Strings.ar.xaml"
                : "Localization/Strings.en.xaml";

            var newDict = new ResourceDictionary { Source = new Uri(dictUri, UriKind.Relative) };

            var appResources = Application.Current.Resources.MergedDictionaries;
            var oldDict = appResources.FirstOrDefault(d =>
                d.Source != null && d.Source.OriginalString.Contains("Strings."));
            if (oldDict != null) appResources.Remove(oldDict);
            appResources.Add(newDict);

            // Flip layout direction for the whole window (mirrors nav/content for Arabic)
            FlowDirection = _isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Refresh current view so its own FlowDirection-dependent bits (e.g. DataGrid) update
            if (MainContent.Content is DashboardView) ShowDashboard_Click(null, null);
            else if (MainContent.Content is EmployeesView) ShowEmployees_Click(null, null);
        }
    }
}
