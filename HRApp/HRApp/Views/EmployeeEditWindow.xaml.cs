using System;
using System.Globalization;
using System.Windows;
using HRApp.Models;

namespace HRApp.Views
{
    public partial class EmployeeEditWindow : Window
    {
        private readonly Employee _employee;
        public Employee Result { get; private set; }

        public EmployeeEditWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            TxtNameEn.Text = employee.FullNameEn;
            TxtNameAr.Text = employee.FullNameAr;
            TxtDepartment.Text = employee.Department;
            TxtPosition.Text = employee.Position;
            TxtSalary.Text = employee.Salary == 0 ? "" : employee.Salary.ToString(CultureInfo.InvariantCulture);
            TxtPhone.Text = employee.Phone;
            TxtEmail.Text = employee.Email;
            DpHireDate.SelectedDate = employee.HireDate == DateTime.MinValue ? DateTime.Today : employee.HireDate;

            SetCombo(CmbGender, employee.Gender);
            SetCombo(CmbStatus, string.IsNullOrEmpty(employee.Status) ? "Active" : employee.Status);
        }

        private static void SetCombo(System.Windows.Controls.ComboBox combo, string value)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in combo.Items)
                if (string.Equals(item.Content?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = item;
                    return;
                }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNameEn.Text) && string.IsNullOrWhiteSpace(TxtNameAr.Text))
            {
                MessageBox.Show("Enter at least one name (English or Arabic).");
                return;
            }

            decimal.TryParse(TxtSalary.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var salary);

            _employee.FullNameEn = TxtNameEn.Text.Trim();
            _employee.FullNameAr = TxtNameAr.Text.Trim();
            _employee.Department = TxtDepartment.Text.Trim();
            _employee.Position = TxtPosition.Text.Trim();
            _employee.Gender = (CmbGender.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            _employee.HireDate = DpHireDate.SelectedDate ?? DateTime.Today;
            _employee.Salary = salary;
            _employee.Status = (CmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Active";
            _employee.Phone = TxtPhone.Text.Trim();
            _employee.Email = TxtEmail.Text.Trim();

            Result = _employee;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
