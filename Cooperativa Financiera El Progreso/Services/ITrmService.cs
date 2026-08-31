using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Services;

public interface ITrmService
{
    Task<TrmInfo?> GetCurrentTrmAsync();
}
