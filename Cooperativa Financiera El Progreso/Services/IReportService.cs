using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Services;

public interface IReportService
{
    GeneralBalanceReport GetGeneralBalance();
    List<MemberBalanceReport> GetTop5MembersByBalance();
    List<Member> GetInactiveMembers();
    PeriodSummaryReport GetPeriodSummary(DateTime startDate, DateTime endDate);
    List<TopTransactionReport> GetTop10Transactions();
    List<MemberActivityReport> GetCashFlowSummaryByMember();
}
