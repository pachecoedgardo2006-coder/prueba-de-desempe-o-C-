using System.Globalization;
using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Services;

namespace Cooperativa_Financiera_El_Progreso.Views;

public class TransactionView
{
    private readonly ITransactionService _transactionService;
    private readonly IMemberService _memberService;
    private static readonly CultureInfo CopCulture = new("es-CO");

    public TransactionView(ITransactionService transactionService, IMemberService memberService)
    {
        _transactionService = transactionService;
        _memberService = memberService;
    }

    public void ShowDepositForm()
    {
        Console.WriteLine("\n=== Register Deposit ===");
        Console.Write("Enter Member Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Member: {member.FullName}");
        Console.Write("Enter Deposit Amount (COP): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Invalid deposit amount. Must be a positive number.");
            Console.ResetColor();
            return;
        }

        try
        {
            var tx = _transactionService.Deposit(member.DocumentNumber, amount);
            decimal newBalance = _memberService.GetBalance(member.Id);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Deposit of {tx.Amount.ToString("C", CopCulture)} processed successfully!");
            Console.ResetColor();
            Console.WriteLine($"New Balance: {newBalance.ToString("C", CopCulture)} COP");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();
        }
    }

    public void ShowWithdrawalForm()
    {
        Console.WriteLine("\n=== Register Withdrawal ===");
        Console.Write("Enter Member Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        decimal currentBalance = _memberService.GetBalance(member.Id);
        Console.WriteLine($"Member: {member.FullName} | Current Balance: {currentBalance.ToString("C", CopCulture)} COP");

        Console.Write("Enter Withdrawal Amount (COP): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Invalid withdrawal amount. Must be a positive number.");
            Console.ResetColor();
            return;
        }

        try
        {
            var tx = _transactionService.Withdraw(member.DocumentNumber, amount);
            decimal newBalance = _memberService.GetBalance(member.Id);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Withdrawal of {tx.Amount.ToString("C", CopCulture)} processed successfully!");
            if (tx.Fee > 0)
            {
                Console.WriteLine($"Cash management fee applied: {tx.Fee.ToString("C", CopCulture)} COP");
            }
            Console.ResetColor();
            Console.WriteLine($"New Balance: {newBalance.ToString("C", CopCulture)} COP");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();
        }
    }

    public void ShowMemberTransactions()
    {
        Console.WriteLine("\n=== Member Transaction History ===");
        Console.Write("Enter Member Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        var transactions = _transactionService.GetMemberTransactions(member.DocumentNumber);

        Console.WriteLine(new string('-', 85));
        Console.WriteLine($"Member: {member.FullName} | Doc: {member.DocumentNumber}");
        Console.WriteLine(new string('-', 85));

        if (transactions.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No transactions found for this member.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"| {"ID",-4} | {"Date",-20} | {"Type",-12} | {"Amount",-18} | {"Fee",-12} |");
        Console.WriteLine(new string('-', 85));

        decimal runningBalance = 0;
        foreach (var tx in transactions)
        {
            string typeStr = tx.Type == TransactionType.Deposit ? "Deposit" : "Withdrawal";
            string feeStr = tx.Fee > 0 ? tx.Fee.ToString("C", CopCulture) : "$0";
            Console.WriteLine($"| {tx.Id,-4} | {tx.Date:yyyy-MM-dd HH:mm:ss,-20} | {typeStr,-12} | {tx.Amount.ToString("C", CopCulture),-18} | {feeStr,-12} |");

            if (tx.Type == TransactionType.Deposit)
            {
                runningBalance += tx.Amount;
            }
            else
            {
                runningBalance -= (tx.Amount + tx.Fee);
            }
        }

        Console.WriteLine(new string('-', 85));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Current Total Balance: {runningBalance.ToString("C", CopCulture)} COP");
        Console.ResetColor();
    }
}
