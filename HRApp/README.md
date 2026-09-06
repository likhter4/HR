# HR Desktop App (WPF + MS Access) — Bilingual (English / Arabic)

A Windows desktop HR application with:
- **Dashboard**: total/active/inactive employee KPIs + a department headcount chart
- **Employees module**: add, edit, delete, browse (bilingual name fields — English & Arabic)
- **Full Arabic support**: a language toggle button switches all UI text and flips the whole layout to right-to-left
- **Database**: Microsoft Access (`.accdb`), accessed via OLE DB — no server needed, the file lives next to the app

This must be built and run on **Windows** (WPF and the Access OLE DB driver are Windows-only).

## 1. Prerequisites

1. **Visual Studio 2022** (Community edition is fine) with the **.NET desktop development** workload installed.
2. **.NET 8 SDK** (Visual Studio installs this for you if you pick the workload above).
3. **Microsoft Access Database Engine 2016 Redistributable** — install the **64-bit** version if your Visual Studio / Windows is 64-bit (which is standard today):
   https://www.microsoft.com/en-us/download/details.aspx?id=54920
   (If you already have Microsoft Access installed on this PC, this is usually already satisfied — just make sure the "bitness" — 32 vs 64-bit — of Office matches your build.)

## 2. Create the empty database file

1. Open **Microsoft Access** → *Blank database*.
2. Name it exactly **`HRDatabase.accdb`**.
3. Save it — for now, don't create any tables; the app creates the `Employees` table automatically the first time it runs.
4. Copy `HRDatabase.accdb` into the same folder as the compiled `HRApp.exe` (i.e. `HRApp\bin\Debug\net8.0-windows\`). You'll need to re-copy it there after every clean rebuild, or set "Copy to Output Directory" on the file once you add it to the Visual Studio project.

> Don't have Microsoft Access installed? You can still create a blank `.accdb`: open Excel or any PC with Access/Office 365 once to create the empty file, or ask and I can give you a small helper script that creates it via COM automation.

## 3. Open and run the project

1. Open `HRApp.sln` in Visual Studio.
2. Build (Ctrl+Shift+B) — this restores the `System.Data.OleDb` NuGet package automatically.
3. Copy `HRDatabase.accdb` into the output folder as described above.
4. Press F5 to run.

## 4. Project structure

```
HRApp/
  HRApp.csproj
  App.xaml / App.xaml.cs          <- startup, initializes DB schema
  MainWindow.xaml / .cs           <- sidebar nav + language toggle
  Models/Employee.cs
  Data/DbHelper.cs                <- all Access/OLE DB code (CRUD + dashboard queries)
  Views/
    DashboardView.xaml / .cs      <- KPIs + department bar chart
    EmployeesView.xaml / .cs      <- employee grid + toolbar
    EmployeeEditWindow.xaml / .cs <- add/edit form
  Localization/
    Strings.en.xaml
    Strings.ar.xaml
```

## No Windows machine? Build the .exe for free with GitHub Actions

This project includes `.github/workflows/build.yml`, which builds `HRApp.exe` automatically on a free Microsoft-hosted Windows machine — you don't need Windows, Visual Studio, or anything installed locally, just a (free) GitHub account and a web browser.

1. Go to https://github.com and create a free account if you don't have one.
2. Click **New repository** (top right → the "+" icon), give it any name (e.g. `hr-app`), keep it **Private** or **Public** as you prefer, and click **Create repository**.
3. On the new repo's page, click **uploading an existing file** (or drag-and-drop), then drag in the entire unzipped `HRApp` folder (including the hidden `.github` folder — if your unzip tool hides it, use "Show hidden files" first, or zip only that folder separately and upload/extract it as its own commit). Commit the files.
4. Click the **Actions** tab at the top of the repo. You should see a workflow run start automatically (or click **Run workflow** if it didn't).
5. Wait 1–2 minutes for it to finish (green checkmark).
6. Click into the finished run, scroll to **Artifacts**, and download **HRApp-exe** — that's a zip containing your real, ready-to-run `HRApp.exe`.
7. Copy that `.exe` next to your `HRDatabase.accdb` file on any Windows PC and run it.

Repeat step 6 any time you push new code changes — it rebuilds automatically.

## 5. Extending it

This covers the core (Employees + Dashboard). Natural next additions, following the same pattern (a `Views/XView.xaml` + a table/method in `DbHelper.cs`):
- **Attendance & leave tracking** — a `Leaves` / `Attendance` table, a calendar-style view, linked to `Employees.ID`.
- **Payroll** — a `Payroll` table with monthly runs, computed from `Employees.Salary`.
- **Login/roles** — a simple `Users` table + a login window shown before `MainWindow`.

Want me to add any of these next?
