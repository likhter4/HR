using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using HRApp.Models;

namespace HRApp.Data
{
    /// <summary>
    /// All access-database plumbing lives here. The app expects an existing,
    /// blank "HRDatabase.accdb" file sitting next to the .exe (create it once
    /// via Microsoft Access: File > New > Blank Database). On first run,
    /// EnsureSchema() creates the tables automatically if they don't exist yet.
    /// </summary>
    public static class DbHelper
    {
        private static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HRDatabase.accdb");

        private static string ConnStr =>
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DbPath};";

        private static OleDbConnection GetConnection() => new OleDbConnection(ConnStr);

        public static void EnsureSchema()
        {
            if (!File.Exists(DbPath))
                throw new FileNotFoundException(
                    $"HRDatabase.accdb was not found at:\n{DbPath}\n\nCreate a blank Access database with this exact name in the app folder first.");

            using var conn = GetConnection();
            conn.Open();

            if (!TableExists(conn, "Employees"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE Employees (
                        ID COUNTER PRIMARY KEY,
                        FullNameEn TEXT(100),
                        FullNameAr TEXT(100),
                        Department TEXT(50),
                        Position TEXT(50),
                        Gender TEXT(10),
                        HireDate DATETIME,
                        Salary CURRENCY,
                        Status TEXT(20),
                        Phone TEXT(20),
                        Email TEXT(100)
                    )";
                cmd.ExecuteNonQuery();
            }
        }

        private static bool TableExists(OleDbConnection conn, string tableName)
        {
            var schema = conn.GetSchema("Tables", new[] { null, null, tableName, "TABLE" });
            return schema.Rows.Count > 0;
        }

        public static List<Employee> GetEmployees()
        {
            var list = new List<Employee>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand("SELECT * FROM Employees ORDER BY FullNameEn", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        private static Employee Map(IDataRecord r) => new Employee
        {
            Id = Convert.ToInt32(r["ID"]),
            FullNameEn = r["FullNameEn"] as string,
            FullNameAr = r["FullNameAr"] as string,
            Department = r["Department"] as string,
            Position = r["Position"] as string,
            Gender = r["Gender"] as string,
            HireDate = r["HireDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["HireDate"]),
            Salary = r["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Salary"]),
            Status = r["Status"] as string,
            Phone = r["Phone"] as string,
            Email = r["Email"] as string
        };

        public static void AddEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand(@"
                INSERT INTO Employees
                (FullNameEn, FullNameAr, Department, Position, Gender, HireDate, Salary, Status, Phone, Email)
                VALUES (?,?,?,?,?,?,?,?,?,?)", conn);
            AddParams(cmd, e);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand(@"
                UPDATE Employees SET
                FullNameEn=?, FullNameAr=?, Department=?, Position=?, Gender=?,
                HireDate=?, Salary=?, Status=?, Phone=?, Email=?
                WHERE ID=?", conn);
            AddParams(cmd, e);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteEmployee(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand("DELETE FROM Employees WHERE ID=?", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private static void AddParams(OleDbCommand cmd, Employee e)
        {
            cmd.Parameters.AddWithValue("@FullNameEn", (object)e.FullNameEn ?? "");
            cmd.Parameters.AddWithValue("@FullNameAr", (object)e.FullNameAr ?? "");
            cmd.Parameters.AddWithValue("@Department", (object)e.Department ?? "");
            cmd.Parameters.AddWithValue("@Position", (object)e.Position ?? "");
            cmd.Parameters.AddWithValue("@Gender", (object)e.Gender ?? "");
            cmd.Parameters.AddWithValue("@HireDate", e.HireDate == DateTime.MinValue ? (object)DBNull.Value : e.HireDate);
            cmd.Parameters.AddWithValue("@Salary", e.Salary);
            cmd.Parameters.AddWithValue("@Status", (object)e.Status ?? "");
            cmd.Parameters.AddWithValue("@Phone", (object)e.Phone ?? "");
            cmd.Parameters.AddWithValue("@Email", (object)e.Email ?? "");
        }

        // ---- Dashboard aggregate queries ----

        public static Dictionary<string, int> GetCountsByDepartment()
        {
            var result = new Dictionary<string, int>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand(
                "SELECT Department, COUNT(*) AS Cnt FROM Employees GROUP BY Department", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var dept = reader["Department"] as string;
                if (string.IsNullOrWhiteSpace(dept)) dept = "(Unassigned)";
                result[dept] = Convert.ToInt32(reader["Cnt"]);
            }
            return result;
        }

        public static (int total, int active, int inactive) GetStatusCounts()
        {
            int total = 0, active = 0, inactive = 0;
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OleDbCommand("SELECT Status FROM Employees", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                total++;
                var status = reader["Status"] as string;
                if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)) active++;
                else inactive++;
            }
            return (total, active, inactive);
        }
    }
}
