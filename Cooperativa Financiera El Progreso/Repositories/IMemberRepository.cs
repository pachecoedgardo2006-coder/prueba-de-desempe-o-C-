using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Repositories;

public interface IMemberRepository
{
    List<Member> GetAll();
    Member? GetById(int id);
    void Add(Member member);
    void Update(Member member);
    void Delete(int id);
    int GetNextId();
}
