using System.Globalization;
using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Services;

namespace Cooperativa_Financiera_El_Progreso.Views;

public class ReportView
{
    private readonly IReportService _reportService;
    private static readonly CultureInfo CopCulture = new("es-CO");

    public ReportView(IReportService reportService)
    {
        _reportService = reportService;
    }

    public void ShowReportsMenu()
    {
        bool inReports = true;
        while (inReports)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("           MANAGEMENT REPORTS MENU            ");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. How much money do we have? (General Balance)");
            Console.WriteLine("2. Who are our top 5 members? (Highest Balance)");
            Console.WriteLine("3. Who is inactive? (Zero Movements)");
            Console.WriteLine("4. Performance in a date range");
            Console.WriteLine("5. Top 10 largest transactions");
            Console.WriteLine("6. Who is moving cash? (Activity Summary by Member)");
            Console.WriteLine("0. Return to Main Menu");
            Console.Write("\nSelect a report option: ");

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ShowGeneralBalanceReport();
                    break;
                case "2":
                    ShowTop5MembersReport();
                    break;
                case "3":
                    ShowInactiveMembersReport();
                    break;
                case "4":
                    ShowPeriodSummaryReport();
                    break;
                case "5":
                    ShowTop10TransactionsReport();
                    break;
                case "6":
                    ShowCashFlowSummaryReport();
                    break;
                case "0":
                    inReports = false;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Invalid report option.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    private void ShowGeneralBalanceReport()
    {
        Console.WriteLine("\n=== 1. General Balance Report ===");
        var report = _reportService.GetGeneralBalance();

        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Total Cooperative Balance: {report.TotalBalance.ToString("C", CopCulture)} COP");
        Console.WriteLine($"Total Registered Members:  {report.TotalMembers}");
        Console.WriteLine($"Average Balance / Member:  {report.AverageBalance.ToString("C", CopCulture)} COP");
        Console.WriteLine(new string('-', 50));
    }

    private void ShowTop5MembersReport()
    {
        Console.WriteLine("\n=== 2. Top 5 Members with Highest Balance ===");
        var list = _reportService.GetTop5MembersByBalance();

        if (list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No members registered.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"| {"#",-3} | {"Document",-15} | {"Full Name",-25} | {"Balance",-16} |");
        Console.WriteLine(new string('-', 70));

        int rank = 1;
        foreach (var m in list)
        {
            Console.WriteLine($"| {rank++,-3} | {m.DocumentNumber,-15} | {m.FullName,-25} | {m.Balance.ToString("C", CopCulture),-16} |");
        }
        Console.WriteLine(new string('-', 70));
    }

    private void ShowInactiveMembersReport()
    {
        Console.WriteLine("\n=== 3. Inactive Members (Zero Transactions) ===");
        var list = _reportService.GetInactiveMembers();

        if (list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All registered members have at least one transaction.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('-', 75));
        Console.WriteLine($"| {"ID",-4} | {"Document",-15} | {"Full Name",-25} | {"Registered",-18} |");
        Console.WriteLine(new string('-', 75));

        foreach (var m in list)
        {
            Console.WriteLine($"| {m.Id,-4} | {m.DocumentNumber,-15} | {m.FullName,-25} | {m.CreatedAt:yyyy-MM-dd HH:mm,-18} |");
        }
        Console.WriteLine(new string('-', 75));
        Console.WriteLine($"Total inactive members: {list.Count}");
    }

    private void ShowPeriodSummaryReport()
    {
        Console.WriteLine("\n=== 4. Performance in a Date Range ===");
        Console.Write("Enter Start Date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Invalid start date format.");
            Console.ResetColor();
            return;
        }

        Console.Write("Enter End Date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Invalid end date format.");
            Console.ResetColor();
            return;
        }

        if (endDate < startDate)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] End date cannot be earlier than start date.");
            Console.ResetColor();
            return;
        }

        var report = _reportService.GetPeriodSummary(startDate, endDate);

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"Period:                    {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
        Console.WriteLine($"Total Inflow (Deposits):   {report.TotalDeposits.ToString("C", CopCulture)} COP ({report.DepositCount} transactions)");
        Console.WriteLine($"Total Outflow (Withdraws): {report.TotalWithdrawals.ToString("C", CopCulture)} COP ({report.WithdrawalCount} transactions)");
        Console.WriteLine($"Total Fees Collected:      {report.TotalFees.ToString("C", CopCulture)} COP");
        Console.ForegroundColor = report.NetDifference >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"Net Difference:            {report.NetDifference.ToString("C", CopCulture)} COP");
        Console.ResetColor();
        Console.WriteLine(new string('-', 60));
    }

    private void ShowTop10TransactionsReport()
    {
        Console.WriteLine("\n=== 5. Top 10 Largest Transactions ===");
        var list = _reportService.GetTop10Transactions();

        if (list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No transactions registered yet.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('-', 85));
        Console.WriteLine($"| {"#",-3} | {"Date",-20} | {"Type",-12} | {"Amount",-18} | {"Member",-20} |");
        Console.WriteLine(new string('-', 85));

        int rank = 1;
        foreach (var t in list)
        {
            string typeStr = t.Type == TransactionType.Deposit ? "Deposit" : "Withdrawal";
            Console.WriteLine($"| {rank++,-3} | {t.Date:yyyy-MM-dd HH:mm:ss,-20} | {typeStr,-12} | {t.Amount.ToString("C", CopCulture),-18} | {t.MemberName,-20} |");
        }
        Console.WriteLine(new string('-', 85));
    }

    private void ShowCashFlowSummaryReport()
    {
        Console.WriteLine("\n=== 6. Cash Flow Activity Summary by Member ===");
        var list = _reportService.GetCashFlowSummaryByMember();

        if (list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No members registered.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('-', 95));
        Console.WriteLine($"| {"Full Name",-24} | {"Movements",-10} | {"Deposited",-16} | {"Withdrawn",-16} | {"Balance",-16} |");
        Console.WriteLine(new string('-', 95));

        foreach (var a in list)
        {
            Console.WriteLine($"| {a.FullName,-24} | {a.MovementCount,-10} | {a.TotalDeposited.ToString("C", CopCulture),-16} | {a.TotalWithdrawn.ToString("C", CopCulture),-16} | {a.CurrentBalance.ToString("C", CopCulture),-16} |");
        }
        Console.WriteLine(new string('-', 95));
    }
}
