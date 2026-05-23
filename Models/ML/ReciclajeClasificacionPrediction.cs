using Microsoft.ML.Data;

namespace EcoPlantas.Models.ML
{
    public class ReciclajeClasificacionPrediction
    {
        [ColumnName("PredictedLabel")]
        public string NivelImpacto { get; set; } = string.Empty;

        public float[] Score { get; set; } = Array.Empty<float>();
    }
}
