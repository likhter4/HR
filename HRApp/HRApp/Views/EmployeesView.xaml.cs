using System.Windows;
using System.Windows.Controls;
using HRApp.Data;
using HRApp.Models;

namespace HRApp.Views
{
    public partial class EmployeesView : UserControl
    {
        public EmployeesView()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            Grid.ItemsSource = DbHelper.GetEmployees();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new EmployeeEditWindow(new Employee { Status = "Active" });
            if (dlg.ShowDialog() == true)
            {
                DbHelper.AddEmployee(dlg.Result);
                Refresh();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            var dlg = new EmployeeEditWindow(selected);
            if (dlg.ShowDialog() == true)
            {
                DbHelper.UpdateEmployee(dlg.Result);
                Refresh();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            var confirm = MessageBox.Show($"Delete {selected.DisplayName}?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                DbHelper.DeleteEmployee(selected.Id);
                Refresh();
            }
        }
    }
}
