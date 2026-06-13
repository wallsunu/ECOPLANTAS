using Microsoft.ML.Data;

namespace EcoPlantas.Models.ML
{
    public class RecomendacionPlantaInput
    {
        [LoadColumn(0)]
        public string TipoMaterial { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float CantidadKg { get; set; }

        [LoadColumn(2)]
        public string Preferencia { get; set; } = string.Empty;

        [LoadColumn(3)]
        public string PlantaRecomendada { get; set; } = string.Empty;
    }
}
