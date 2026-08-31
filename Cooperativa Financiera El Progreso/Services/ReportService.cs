using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Repositories;

namespace Cooperativa_Financiera_El_Progreso.Services;

/// <summary>
/// Service providing management analytical reports powered by LINQ queries.
/// </summary>
public class ReportService : IReportService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMemberService _memberService;

    public ReportService(
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IMemberService memberService)
    {
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _memberService = memberService;
    }

    /// <summary>
    /// Generates general financial summary (total balance, members count, average balance).
    /// </summary>
    public GeneralBalanceReport GetGeneralBalance()
    {
        var members = _memberRepository.GetAll();
        var transactions = _transactionRepository.GetAll();

        decimal totalDeposits = transactions.Where(t => t.Type == TransactionType.Deposit).Sum(t => t.Amount);
        decimal totalWithdrawals = transactions.Where(t => t.Type == TransactionType.Withdrawal).Sum(t => t.Amount + t.Fee);
        decimal totalBalance = totalDeposits - totalWithdrawals;

        int totalMembers = members.Count;
        decimal averageBalance = totalMembers > 0 ? totalBalance / totalMembers : 0;

        return new GeneralBalanceReport
        {
            TotalBalance = totalBalance,
            TotalMembers = totalMembers,
            AverageBalance = averageBalance
        };
    }

    /// <summary>
    /// Retrieves top 5 members with the highest savings balance.
    /// </summary>
    public List<MemberBalanceReport> GetTop5MembersByBalance()
    {
        var members = _memberRepository.GetAll();

        return members
            .Select(m => new MemberBalanceReport
            {
                DocumentNumber = m.DocumentNumber,
                FullName = m.FullName,
                Balance = _memberService.GetBalance(m.Id)
            })
            .OrderByDescending(r => r.Balance)
            .Take(5)
            .ToList();
    }

    /// <summary>
    /// Identifies members with zero recorded transactions since registration.
    /// </summary>
    public List<Member> GetInactiveMembers()
    {
        var members = _memberRepository.GetAll();
        var transactions = _transactionRepository.GetAll();

        var activeMemberIds = transactions.Select(t => t.MemberId).Distinct().ToHashSet();

        return members
            .Where(m => !activeMemberIds.Contains(m.Id))
            .OrderBy(m => m.FullName)
            .ToList();
    }

    /// <summary>
    /// Analyzes financial movements within a specified date range.
    /// </summary>
    public PeriodSummaryReport GetPeriodSummary(DateTime startDate, DateTime endDate)
    {
        DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
        DateTime startOfDay = startDate.Date;

        var periodTransactions = _transactionRepository.GetAll()
            .Where(t => t.Date >= startOfDay && t.Date <= endOfDay)
            .ToList();

        var deposits = periodTransactions.Where(t => t.Type == TransactionType.Deposit).ToList();
        var withdrawals = periodTransactions.Where(t => t.Type == TransactionType.Withdrawal).ToList();

        return new PeriodSummaryReport
        {
            StartDate = startOfDay,
            EndDate = endDate.Date,
            TotalDeposits = deposits.Sum(t => t.Amount),
            DepositCount = deposits.Count,
            TotalWithdrawals = withdrawals.Sum(t => t.Amount),
            WithdrawalCount = withdrawals.Count,
            TotalFees = withdrawals.Sum(t => t.Fee)
        };
    }

    /// <summary>
    /// Retrieves the 10 largest financial transactions executed across the cooperative.
    /// </summary>
    public List<TopTransactionReport> GetTop10Transactions()
    {
        var transactions = _transactionRepository.GetAll();
        var membersDict = _memberRepository.GetAll().ToDictionary(m => m.Id, m => m.FullName);

        return transactions
            .OrderByDescending(t => t.Amount)
            .Take(10)
            .Select(t => new TopTransactionReport
            {
                Date = t.Date,
                Type = t.Type,
                Amount = t.Amount,
                MemberName = membersDict.TryGetValue(t.MemberId, out var name) ? name : "Unknown"
            })
            .ToList();
    }

    /// <summary>
    /// Generates cash flow activity breakdown grouped by member.
    /// </summary>
    public List<MemberActivityReport> GetCashFlowSummaryByMember()
    {
        var members = _memberRepository.GetAll();
        var transactions = _transactionRepository.GetAll();

        var groupedTransactions = transactions
            .GroupBy(t => t.MemberId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return members
            .Select(m =>
            {
                var memberTx = groupedTransactions.TryGetValue(m.Id, out var list) ? list : new List<Transaction>();
                decimal deposited = memberTx.Where(t => t.Type == TransactionType.Deposit).Sum(t => t.Amount);
                decimal withdrawn = memberTx.Where(t => t.Type == TransactionType.Withdrawal).Sum(t => t.Amount + t.Fee);

                return new MemberActivityReport
                {
                    FullName = m.FullName,
                    MovementCount = memberTx.Count,
                    TotalDeposited = deposited,
                    TotalWithdrawn = withdrawn,
                    CurrentBalance = deposited - withdrawn
                };
            })
            .OrderByDescending(r => r.MovementCount)
            .ToList();
    }
}
