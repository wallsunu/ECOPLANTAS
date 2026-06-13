using Microsoft.ML.Data;

namespace EcoPlantas.Models.ML
{
    public class RecomendacionPlantaPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PlantaRecomendada { get; set; } = string.Empty;

        public float[] Score { get; set; } = Array.Empty<float>();
    }
}
