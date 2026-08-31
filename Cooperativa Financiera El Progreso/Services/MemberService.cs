using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Repositories;

namespace Cooperativa_Financiera_El_Progreso.Services;

/// <summary>
/// Service handling member profile management, validations, and balance computation.
/// </summary>
public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;

    public MemberService(IMemberRepository memberRepository, ITransactionRepository transactionRepository)
    {
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Registers a new member with a unique document number.
    /// </summary>
    public Member RegisterMember(string documentNumber, string fullName, string phoneNumber, string address)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            throw new ArgumentException("Document number is required.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.");
        }

        string trimmedDocument = documentNumber.Trim();
        var existing = _memberRepository.GetAll()
            .FirstOrDefault(m => m.DocumentNumber.Equals(trimmedDocument, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            throw new InvalidOperationException($"A member with document number '{trimmedDocument}' already exists.");
        }

        var member = new Member
        {
            Id = _memberRepository.GetNextId(),
            DocumentNumber = trimmedDocument,
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Address = address.Trim(),
            CreatedAt = DateTime.Now
        };

        _memberRepository.Add(member);
        return member;
    }

    /// <summary>
    /// Returns all registered members ordered by full name.
    /// </summary>
    public List<Member> GetAllMembers()
    {
        return _memberRepository.GetAll()
            .OrderBy(m => m.FullName)
            .ToList();
    }

    /// <summary>
    /// Finds a member by document number using exact match.
    /// </summary>
    public Member? FindByDocument(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return null;
        }

        string trimmedDocument = documentNumber.Trim();
        return _memberRepository.GetAll()
            .FirstOrDefault(m => m.DocumentNumber.Equals(trimmedDocument, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds members whose names contain the specified search query (case-insensitive).
    /// </summary>
    public List<Member> FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new List<Member>();
        }

        string trimmedName = name.Trim();
        return _memberRepository.GetAll()
            .Where(m => m.FullName.Contains(trimmedName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.FullName)
            .ToList();
    }

    /// <summary>
    /// Updates basic contact and profile information of a member.
    /// </summary>
    public Member UpdateMember(string documentNumber, string fullName, string phoneNumber, string address)
    {
        var member = FindByDocument(documentNumber)
            ?? throw new KeyNotFoundException($"Member with document '{documentNumber}' not found.");

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.");
        }

        member.FullName = fullName.Trim();
        member.PhoneNumber = phoneNumber.Trim();
        member.Address = address.Trim();

        _memberRepository.Update(member);
        return member;
    }

    /// <summary>
    /// Deletes a member only if they have no transaction history and zero balance.
    /// </summary>
    public void DeleteMember(string documentNumber)
    {
        var member = FindByDocument(documentNumber)
            ?? throw new KeyNotFoundException($"Member with document '{documentNumber}' not found.");

        var transactions = _transactionRepository.GetByMemberId(member.Id);
        if (transactions.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a member with registered transactions or history.");
        }

        decimal balance = GetBalance(member.Id);
        if (balance > 0)
        {
            throw new InvalidOperationException("Cannot delete a member with an active balance.");
        }

        _memberRepository.Delete(member.Id);
    }

    /// <summary>
    /// Calculates the current balance of a member dynamically based on transaction history.
    /// </summary>
    public decimal GetBalance(int memberId)
    {
        var transactions = _transactionRepository.GetByMemberId(memberId);
        decimal deposits = transactions
            .Where(t => t.Type == TransactionType.Deposit)
            .Sum(t => t.Amount);

        decimal withdrawalsAndFees = transactions
            .Where(t => t.Type == TransactionType.Withdrawal)
            .Sum(t => t.Amount + t.Fee);

        return deposits - withdrawalsAndFees;
    }
}
