using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Services;

public interface IMemberService
{
    Member RegisterMember(string documentNumber, string fullName, string phoneNumber, string address);
    List<Member> GetAllMembers();
    Member? FindByDocument(string documentNumber);
    List<Member> FindByName(string name);
    Member UpdateMember(string documentNumber, string fullName, string phoneNumber, string address);
    void DeleteMember(string documentNumber);
    decimal GetBalance(int memberId);
}
