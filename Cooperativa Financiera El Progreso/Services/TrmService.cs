using System.Text.Json;
using Cooperativa_Financiera_El_Progreso.Models;

namespace Cooperativa_Financiera_El_Progreso.Services;

/// <summary>
/// Service responsible for fetching official TRM exchange rates from the government open data API.
/// </summary>
public class TrmService : ITrmService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(8) };
    private const string ApiUrl = "https://www.datos.gov.co/resource/32sa-8pi3.json?$order=vigenciadesde%20DESC&$limit=1";

    /// <summary>
    /// Asynchronously queries the latest official TRM rate.
    /// Returns null if the service is unreachable or encounters an error.
    /// </summary>
    public async Task<TrmInfo?> GetCurrentTrmAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync(ApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<TrmInfo>>(json);
            return list?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
