using Microsoft.ML.Data;

namespace EcoPlantas.Models.ML
{
    public class ReciclajeClasificacionInput
    {
        [LoadColumn(0)]
        public string TipoMaterial { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float CantidadKg { get; set; }

        [LoadColumn(2)]
        public string NivelImpacto { get; set; } = string.Empty;
    }
}
