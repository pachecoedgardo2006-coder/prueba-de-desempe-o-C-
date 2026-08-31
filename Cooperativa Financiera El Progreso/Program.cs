using Cooperativa_Financiera_El_Progreso.Repositories;
using Cooperativa_Financiera_El_Progreso.Services;
using Cooperativa_Financiera_El_Progreso.Views;

namespace Cooperativa_Financiera_El_Progreso;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IMemberRepository memberRepository = new MemberRepository();
        ITransactionRepository transactionRepository = new TransactionRepository();
        IMemberService memberService = new MemberService(memberRepository, transactionRepository);
        ITransactionService transactionService = new TransactionService(memberRepository, transactionRepository, memberService);
        IReportService reportService = new ReportService(memberRepository, transactionRepository, memberService);
        ITrmService trmService = new TrmService();

        MemberView memberView = new MemberView(memberService, trmService);
        TransactionView transactionView = new TransactionView(transactionService, memberService);
        ReportView reportView = new ReportView(reportService);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("    COOPERATIVA FINANCIERA EL PROGRESO       ");
            Console.WriteLine("==============================================");
            Console.WriteLine(" 1. Register a new member");
            Console.WriteLine(" 2. List all members");
            Console.WriteLine(" 3. Search member by document");
            Console.WriteLine(" 4. Search member by name");
            Console.WriteLine(" 5. Update member information");
            Console.WriteLine(" 6. Delete a member");
            Console.WriteLine(" 7. Check member balance (COP)");
            Console.WriteLine(" 8. Check member balance (USD - TRM)");
            Console.WriteLine(" 9. Register a deposit");
            Console.WriteLine("10. Register a withdrawal");
            Console.WriteLine("11. View member transactions history");
            Console.WriteLine("12. Management reports");
            Console.WriteLine(" 0. Exit");
            Console.Write("\nSelect an option: ");

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    memberView.ShowRegisterMemberForm();
                    break;
                case "2":
                    memberView.ShowAllMembers();
                    break;
                case "3":
                    memberView.ShowSearchByDocumentForm();
                    break;
                case "4":
                    memberView.ShowSearchByNameForm();
                    break;
                case "5":
                    memberView.ShowUpdateMemberForm();
                    break;
                case "6":
                    memberView.ShowDeleteMemberForm();
                    break;
                case "7":
                    memberView.ShowCheckBalanceForm();
                    break;
                case "8":
                    await memberView.ShowBalanceInUsdFormAsync();
                    break;
                case "9":
                    transactionView.ShowDepositForm();
                    break;
                case "10":
                    transactionView.ShowWithdrawalForm();
                    break;
                case "11":
                    transactionView.ShowMemberTransactions();
                    break;
                case "12":
                    reportView.ShowReportsMenu();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("\nExiting application. Goodbye!");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ERROR] Invalid option. Please try again.");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
