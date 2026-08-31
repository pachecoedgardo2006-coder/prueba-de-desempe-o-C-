namespace Cooperativa_Financiera_El_Progreso.Models;

/// <summary>
/// Represents an associated member.
/// </summary>
public class Member
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
