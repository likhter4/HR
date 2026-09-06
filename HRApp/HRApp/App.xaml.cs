using System.Windows;
using HRApp.Data;

namespace HRApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DbHelper.EnsureSchema();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to / initialize the Access database.\n\n" + ex.Message +
                    "\n\nMake sure:\n" +
                    "1) HRDatabase.accdb exists next to the app, and\n" +
                    "2) Microsoft Access Database Engine (64-bit) is installed.",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
