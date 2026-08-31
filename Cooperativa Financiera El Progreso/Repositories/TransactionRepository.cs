using System.Text.Json;
using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public TransactionRepository(string relativePath = "data/transactions.json")
    {
        _filePath = Path.IsPathRooted(relativePath)
            ? relativePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        EnsureFileExists();
    }

    public List<Transaction> GetAll()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Transaction>();
        }

        string json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Transaction>();
        }

        return JsonSerializer.Deserialize<List<Transaction>>(json, _jsonOptions) ?? new List<Transaction>();
    }

    public List<Transaction> GetByMemberId(int memberId)
    {
        return GetAll()
            .Where(t => t.MemberId == memberId)
            .OrderBy(t => t.Date)
            .ToList();
    }

    public void Add(Transaction transaction)
    {
        var transactions = GetAll();
        transactions.Add(transaction);
        SaveAll(transactions);
    }

    public int GetNextId()
    {
        var transactions = GetAll();
        return transactions.Count == 0 ? 1 : transactions.Max(t => t.Id) + 1;
    }

    private void SaveAll(List<Transaction> transactions)
    {
        string json = JsonSerializer.Serialize(transactions, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    private void EnsureFileExists()
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
}
