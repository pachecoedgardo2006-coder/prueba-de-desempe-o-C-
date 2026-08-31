using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Repositories;

public interface ITransactionRepository
{
    List<Transaction> GetAll();
    List<Transaction> GetByMemberId(int memberId);
    void Add(Transaction transaction);
    int GetNextId();
}
