using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Services;

public interface ITransactionService
{
    Transaction Deposit(string documentNumber, decimal amount);
    Transaction Withdraw(string documentNumber, decimal amount);
    List<Transaction> GetMemberTransactions(string documentNumber);
}
