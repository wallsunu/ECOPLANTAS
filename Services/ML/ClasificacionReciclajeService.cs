using Microsoft.ML;
using Microsoft.ML.Data;
using EcoPlantas.Models.ML;

namespace EcoPlantas.Services.ML
{
    public class ClasificacionReciclajeService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _modelo;
        private PredictionEngine<ReciclajeClasificacionInput, ReciclajeClasificacionPrediction>? _motor;
        private readonly object _lock = new();

        public ClasificacionReciclajeService()
        {
            _mlContext = new MLContext(seed: 42);
        }

        private void EnsureModeloEntrenado()
        {
            if (_motor != null) return;
            lock (_lock)
            {
                if (_motor != null) return;
                var datos = GenerarDatosSinteticos();
                var dataView = _mlContext.Data.LoadFromEnumerable(datos);

                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ReciclajeClasificacionInput.NivelImpacto))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding("TipoMaterialEncoded", nameof(ReciclajeClasificacionInput.TipoMaterial)))
                    .Append(_mlContext.Transforms.Concatenate("Features", "TipoMaterialEncoded", nameof(ReciclajeClasificacionInput.CantidadKg)))
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                _modelo = pipeline.Fit(dataView);
                _motor = _mlContext.Model.CreatePredictionEngine<ReciclajeClasificacionInput, ReciclajeClasificacionPrediction>(_modelo);
            }
        }

        public ReciclajeClasificacionPrediction Clasificar(string tipoMaterial, float cantidadKg)
        {
            EnsureModeloEntrenado();
            return _motor!.Predict(new ReciclajeClasificacionInput
            {
                TipoMaterial = tipoMaterial,
                CantidadKg = cantidadKg,
                NivelImpacto = string.Empty
            });
        }

        private static IEnumerable<ReciclajeClasificacionInput> GenerarDatosSinteticos()
        {
            // Bajo: < 5 kg para materiales ligeros, < 3 para metales/vidrio
            // Medio: rango intermedio por material
            // Alto: grandes cantidades
            return new[]
            {
                // Plástico — umbral bajo: <4, medio: 4-10, alto: >10
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 1f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 2f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 3f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 5f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 7f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 9f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 12f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 15f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Plástico", CantidadKg = 20f, NivelImpacto = "Alto" },

                // Vidrio — umbral bajo: <3, medio: 3-8, alto: >8
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 1f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 2f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 4f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 6f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 7f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 10f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 14f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Vidrio", CantidadKg = 18f, NivelImpacto = "Alto" },

                // Metal — umbral bajo: <5, medio: 5-12, alto: >12
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 2f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 3f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 4f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 6f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 8f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 11f, NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 15f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 20f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Metal", CantidadKg = 25f, NivelImpacto = "Alto" },

                // Papel — umbral bajo: <5, medio: 5-15, alto: >15
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 1f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 3f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 4f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 7f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 10f, NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 13f, NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 18f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 22f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Papel", CantidadKg = 30f, NivelImpacto = "Alto" },

                // Orgánico — umbral bajo: <3, medio: 3-8, alto: >8
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 1f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 2f,  NivelImpacto = "Bajo" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 4f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 6f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 8f,  NivelImpacto = "Medio" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 10f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 15f, NivelImpacto = "Alto" },
                new ReciclajeClasificacionInput { TipoMaterial = "Orgánico", CantidadKg = 20f, NivelImpacto = "Alto" },
            };
        }
    }
}
