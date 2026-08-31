namespace Cooperativa_Financiera_El_Progreso.Models;

public class Transaction
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}
