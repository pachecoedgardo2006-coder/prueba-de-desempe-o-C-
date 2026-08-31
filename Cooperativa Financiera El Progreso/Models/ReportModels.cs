namespace Cooperativa_Financiera_El_Progreso.Models;

public class GeneralBalanceReport
{
    public decimal TotalBalance { get; set; }
    public int TotalMembers { get; set; }
    public decimal AverageBalance { get; set; }
}

public class MemberBalanceReport
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class PeriodSummaryReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDeposits { get; set; }
    public int DepositCount { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public int WithdrawalCount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetDifference => TotalDeposits - (TotalWithdrawals + TotalFees);
}

public class TopTransactionReport
{
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string MemberName { get; set; } = string.Empty;
}

public class MemberActivityReport
{
    public string FullName { get; set; } = string.Empty;
    public int MovementCount { get; set; }
    public decimal TotalDeposited { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public decimal CurrentBalance { get; set; }
}
