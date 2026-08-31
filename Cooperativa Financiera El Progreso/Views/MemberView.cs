using System.Globalization;
using Cooperativa_Financiera_El_Progreso.Models;
using Cooperativa_Financiera_El_Progreso.Services;

namespace Cooperativa_Financiera_El_Progreso.Views;

public class MemberView
{
    private readonly IMemberService _memberService;
    private readonly ITrmService _trmService;
    private static readonly CultureInfo CopCulture = new("es-CO");
    private static readonly CultureInfo UsdCulture = new("en-US");

    public MemberView(IMemberService memberService, ITrmService trmService)
    {
        _memberService = memberService;
        _trmService = trmService;
    }

    public void ShowRegisterMemberForm()
    {
        Console.WriteLine("\n=== Register New Member ===");

        Console.Write("Enter Document Number: ");
        string? documentNumber = Console.ReadLine();

        Console.Write("Enter Full Name: ");
        string? fullName = Console.ReadLine();

        Console.Write("Enter Phone Number: ");
        string? phoneNumber = Console.ReadLine();

        Console.Write("Enter Address: ");
        string? address = Console.ReadLine();

        try
        {
            var member = _memberService.RegisterMember(
                documentNumber ?? string.Empty,
                fullName ?? string.Empty,
                phoneNumber ?? string.Empty,
                address ?? string.Empty
            );

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Member '{member.FullName}' (ID: {member.Id}, Doc: {member.DocumentNumber}) registered successfully!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();
        }
    }

    public void ShowAllMembers()
    {
        Console.WriteLine("\n=== Registered Members List ===");

        var members = _memberService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No members registered yet.");
            Console.ResetColor();
            return;
        }

        PrintMembersTable(members);
    }

    public void ShowSearchByDocumentForm()
    {
        Console.WriteLine("\n=== Search Member by Document ===");
        Console.Write("Enter Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);

        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        PrintMemberDetails(member);
    }

    public void ShowSearchByNameForm()
    {
        Console.WriteLine("\n=== Search Member by Name ===");
        Console.Write("Enter Name to search: ");
        string? name = Console.ReadLine();

        var members = _memberService.FindByName(name ?? string.Empty);

        if (members.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No members found matching '{name}'.");
            Console.ResetColor();
            return;
        }

        PrintMembersTable(members);
    }

    public void ShowUpdateMemberForm()
    {
        Console.WriteLine("\n=== Update Member Information ===");
        Console.Write("Enter Document Number of the member to update: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Current Name: {member.FullName}");
        Console.Write("Enter New Full Name (leave empty to keep current): ");
        string? newName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newName)) newName = member.FullName;

        Console.WriteLine($"Current Phone: {member.PhoneNumber}");
        Console.Write("Enter New Phone Number (leave empty to keep current): ");
        string? newPhone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newPhone)) newPhone = member.PhoneNumber;

        Console.WriteLine($"Current Address: {member.Address}");
        Console.Write("Enter New Address (leave empty to keep current): ");
        string? newAddress = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newAddress)) newAddress = member.Address;

        try
        {
            var updated = _memberService.UpdateMember(member.DocumentNumber, newName, newPhone, newAddress);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Member '{updated.FullName}' updated successfully!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();
        }
    }

    public void ShowDeleteMemberForm()
    {
        Console.WriteLine("\n=== Delete Member ===");
        Console.Write("Enter Document Number of the member to delete: ");
        string? doc = Console.ReadLine();

        try
        {
            _memberService.DeleteMember(doc ?? string.Empty);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Member with document '{doc}' deleted successfully.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.ResetColor();
        }
    }

    public void ShowCheckBalanceForm()
    {
        Console.WriteLine("\n=== Check Member Balance ===");
        Console.Write("Enter Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        decimal balance = _memberService.GetBalance(member.Id);

        Console.WriteLine(new string('-', 45));
        Console.WriteLine($"Member:      {member.FullName}");
        Console.WriteLine($"Document:    {member.DocumentNumber}");
        Console.WriteLine($"Balance:     {balance.ToString("C", CopCulture)} COP");
        Console.WriteLine(new string('-', 45));
    }

    public async Task ShowBalanceInUsdFormAsync()
    {
        Console.WriteLine("\n=== Check Member Balance in USD (TRM) ===");
        Console.Write("Enter Document Number: ");
        string? doc = Console.ReadLine();

        var member = _memberService.FindByDocument(doc ?? string.Empty);
        if (member == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No member found with document number '{doc}'.");
            Console.ResetColor();
            return;
        }

        decimal balanceCop = _memberService.GetBalance(member.Id);

        Console.WriteLine("\nFetching official TRM rate...");
        var trmInfo = await _trmService.GetCurrentTrmAsync();

        if (trmInfo == null || trmInfo.Value <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[NOTICE] Could not retrieve the official TRM rate from the service. The system will continue operating normally.");
            Console.ResetColor();
            Console.WriteLine($"Current Balance: {balanceCop.ToString("C", CopCulture)} COP");
            return;
        }

        decimal balanceUsd = balanceCop / trmInfo.Value;

        string validFromStr = trmInfo.ValidFrom?.ToString("yyyy-MM-dd") ?? trmInfo.VigenciaDesde;
        string validToStr = trmInfo.ValidTo?.ToString("yyyy-MM-dd") ?? trmInfo.VigenciaHasta;

        Console.WriteLine(new string('-', 55));
        Console.WriteLine($"Member:          {member.FullName}");
        Console.WriteLine($"Document:        {member.DocumentNumber}");
        Console.WriteLine($"Balance (COP):   {balanceCop.ToString("C", CopCulture)} COP");
        Console.WriteLine($"Official TRM:    ${trmInfo.Value:N2} COP");
        Console.WriteLine($"TRM Validity:    From {validFromStr} to {validToStr}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Balance (USD):   {balanceUsd.ToString("C", UsdCulture)} USD");
        Console.ResetColor();
        Console.WriteLine(new string('-', 55));
    }

    private static void PrintMembersTable(List<Member> members)
    {
        Console.WriteLine(new string('-', 85));
        Console.WriteLine($"| {"ID",-4} | {"Document",-15} | {"Full Name",-25} | {"Phone",-12} | {"Address",-15} |");
        Console.WriteLine(new string('-', 85));

        foreach (var m in members)
        {
            Console.WriteLine($"| {m.Id,-4} | {m.DocumentNumber,-15} | {m.FullName,-25} | {m.PhoneNumber,-12} | {m.Address,-15} |");
        }

        Console.WriteLine(new string('-', 85));
        Console.WriteLine($"Total members: {members.Count}");
    }

    private static void PrintMemberDetails(Member member)
    {
        Console.WriteLine(new string('-', 45));
        Console.WriteLine($"ID:          {member.Id}");
        Console.WriteLine($"Document:    {member.DocumentNumber}");
        Console.WriteLine($"Full Name:   {member.FullName}");
        Console.WriteLine($"Phone:       {member.PhoneNumber}");
        Console.WriteLine($"Address:     {member.Address}");
        Console.WriteLine($"Created At:  {member.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('-', 45));
    }
}
