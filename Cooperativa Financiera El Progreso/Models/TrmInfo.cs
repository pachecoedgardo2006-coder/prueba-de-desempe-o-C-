using System.Globalization;
using System.Text.Json.Serialization;

namespace Cooperativa_Financiera_El_Progreso.Models;

public class TrmInfo
{
    [JsonPropertyName("valor")]
    public string Valor { get; set; } = "0";

    [JsonPropertyName("unidad")]
    public string Unidad { get; set; } = "COP";

    [JsonPropertyName("vigenciadesde")]
    public string VigenciaDesde { get; set; } = string.Empty;

    [JsonPropertyName("vigenciahasta")]
    public string VigenciaHasta { get; set; } = string.Empty;

    [JsonIgnore]
    public decimal Value => decimal.TryParse(Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0;

    [JsonIgnore]
    public DateTime? ValidFrom => DateTime.TryParse(VigenciaDesde, out var date) ? date : null;

    [JsonIgnore]
    public DateTime? ValidTo => DateTime.TryParse(VigenciaHasta, out var date) ? date : null;
}
