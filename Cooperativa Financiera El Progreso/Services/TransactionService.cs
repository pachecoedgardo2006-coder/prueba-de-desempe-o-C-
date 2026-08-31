using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Repositories;

namespace Cooperativa_Financiera_El_Progreso.Services;

/// <summary>
/// Service managing deposits, withdrawals, and transaction validation rules.
/// </summary>
public class TransactionService : ITransactionService
{
    private const decimal LargeWithdrawalThreshold = 1000000m;
    private const decimal WithdrawalFeeAmount = 8000m;

    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMemberService _memberService;

    public TransactionService(
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IMemberService memberService)
    {
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _memberService = memberService;
    }

    /// <summary>
    /// Registers a deposit for an active member.
    /// </summary>
    public Transaction Deposit(string documentNumber, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be greater than zero.");
        }

        var member = _memberService.FindByDocument(documentNumber)
            ?? throw new KeyNotFoundException($"Member with document '{documentNumber}' not found.");

        var transaction = new Transaction
        {
            Id = _transactionRepository.GetNextId(),
            MemberId = member.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            Fee = 0,
            Date = DateTime.Now
        };

        _transactionRepository.Add(transaction);
        return transaction;
    }

    /// <summary>
    /// Registers a withdrawal, applying an 8,000 COP fee if amount exceeds 1,000,000 COP.
    /// Validates that the account does not enter negative balance.
    /// </summary>
    public Transaction Withdraw(string documentNumber, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be greater than zero.");
        }

        var member = _memberService.FindByDocument(documentNumber)
            ?? throw new KeyNotFoundException($"Member with document '{documentNumber}' not found.");

        decimal fee = amount > LargeWithdrawalThreshold ? WithdrawalFeeAmount : 0;
        decimal totalDebit = amount + fee;
        decimal currentBalance = _memberService.GetBalance(member.Id);

        if (totalDebit > currentBalance)
        {
            string feeNotice = fee > 0 ? $" (including a ${WithdrawalFeeAmount:N0} cash management fee)" : "";
            throw new InvalidOperationException(
                $"Insufficient funds. Attempted to withdraw ${amount:N0}{feeNotice}, but available balance is ${currentBalance:N0}.");
        }

        var transaction = new Transaction
        {
            Id = _transactionRepository.GetNextId(),
            MemberId = member.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            Fee = fee,
            Date = DateTime.Now
        };

        _transactionRepository.Add(transaction);
        return transaction;
    }

    /// <summary>
    /// Retrieves all transactions for a specific member by document number.
    /// </summary>
    public List<Transaction> GetMemberTransactions(string documentNumber)
    {
        var member = _memberService.FindByDocument(documentNumber)
            ?? throw new KeyNotFoundException($"Member with document '{documentNumber}' not found.");

        return _transactionRepository.GetByMemberId(member.Id);
    }
}
