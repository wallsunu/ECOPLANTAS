using Microsoft.ML;
using EcoPlantas.Models.ML;

namespace EcoPlantas.Services.ML
{
    public class RecomendacionPlantaService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _modelo;
        private PredictionEngine<RecomendacionPlantaInput, RecomendacionPlantaPrediction>? _motor;
        private readonly object _lock = new();

        // Plantas reales del catálogo Productos
        private static readonly Dictionary<string, string> Motivos = new()
        {
            ["Pothos Dorado"]       = "El Pothos Dorado es ideal para quienes reciclan plástico: crece en cualquier espacio y purifica el aire de toxinas comunes en materiales sintéticos.",
            ["Suculenta Echeveria"] = "La Suculenta Echeveria complementa perfectamente el reciclaje de vidrio: su bajo consumo de agua y su diseño decorativo la convierten en símbolo de economía circular.",
            ["Helecho de Boston"]   = "El Helecho de Boston es la elección perfecta para materiales orgánicos: su alta capacidad de absorción de humedad favorece espacios con compostaje activo.",
            ["Cactus San Pedro"]    = "El Cactus San Pedro es el compañero ideal para quien recicla metal: resistente, duradero y de mínimo mantenimiento, como los materiales más robustos.",
            ["Lavanda Ecológica"]   = "La Lavanda Ecológica encaja con el reciclaje de papel: natural, aromática y sostenible, evoca la esencia del papel reciclado y el cuidado del medioambiente.",
            ["Aloe Vera Orgánico"]  = "El Aloe Vera Orgánico es la planta perfecta para interiores: versátil, medicinal y de cuidado sencillo, ideal para quienes apuestan por lo orgánico.",
        };

        public RecomendacionPlantaService()
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

                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(RecomendacionPlantaInput.PlantaRecomendada))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding("TipoMaterialEnc", nameof(RecomendacionPlantaInput.TipoMaterial)))
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding("PreferenciaEnc",  nameof(RecomendacionPlantaInput.Preferencia)))
                    .Append(_mlContext.Transforms.Concatenate("Features", "TipoMaterialEnc", nameof(RecomendacionPlantaInput.CantidadKg), "PreferenciaEnc"))
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                _modelo = pipeline.Fit(dataView);
                _motor = _mlContext.Model.CreatePredictionEngine<RecomendacionPlantaInput, RecomendacionPlantaPrediction>(_modelo);
            }
        }

        public (string Planta, string Motivo) Recomendar(string tipoMaterial, float cantidadKg, string preferencia)
        {
            EnsureModeloEntrenado();
            var prediccion = _motor!.Predict(new RecomendacionPlantaInput
            {
                TipoMaterial     = tipoMaterial,
                CantidadKg       = cantidadKg,
                Preferencia      = preferencia,
                PlantaRecomendada = string.Empty
            });

            var planta = prediccion.PlantaRecomendada;
            var motivo = Motivos.TryGetValue(planta, out var m)
                ? m
                : $"El modelo recomienda {planta} basándose en tu historial de reciclaje y preferencias.";

            return (planta, motivo);
        }

        private static IEnumerable<RecomendacionPlantaInput> GenerarDatosSinteticos()
        {
            // Lógica de reglas:
            // Orgánico  + Interior/Fácil cuidado  => Aloe Vera Orgánico
            // Orgánico  + Decorativa/Resistente    => Helecho de Boston
            // Plástico  + Fácil cuidado/Interior   => Pothos Dorado
            // Plástico  + Decorativa               => Suculenta Echeveria
            // Vidrio    + Decorativa               => Suculenta Echeveria
            // Vidrio    + Fácil cuidado/Interior   => Lavanda Ecológica
            // Metal     + Resistente               => Cactus San Pedro
            // Metal     + Fácil cuidado            => Pothos Dorado
            // Papel     + Interior/Fácil cuidado   => Helecho de Boston
            // Papel     + Decorativa               => Lavanda Ecológica
            return new[]
            {
                // Orgánico — Aloe Vera Orgánico
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 1f,  Preferencia = "Interior",      PlantaRecomendada = "Aloe Vera Orgánico" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 3f,  Preferencia = "Interior",      PlantaRecomendada = "Aloe Vera Orgánico" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 5f,  Preferencia = "Interior",      PlantaRecomendada = "Aloe Vera Orgánico" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 2f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Aloe Vera Orgánico" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 4f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Aloe Vera Orgánico" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 6f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Aloe Vera Orgánico" },
                // Orgánico — Helecho de Boston
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 8f,  Preferencia = "Decorativa",    PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 10f, Preferencia = "Decorativa",    PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 7f,  Preferencia = "Resistente",    PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Orgánico", CantidadKg = 12f, Preferencia = "Resistente",    PlantaRecomendada = "Helecho de Boston" },

                // Plástico — Pothos Dorado
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 2f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 5f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 8f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 3f,  Preferencia = "Interior",      PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 6f,  Preferencia = "Interior",      PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 10f, Preferencia = "Interior",      PlantaRecomendada = "Pothos Dorado" },
                // Plástico — Suculenta Echeveria
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 4f,  Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 9f,  Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 12f, Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 7f,  Preferencia = "Resistente",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Plástico", CantidadKg = 15f, Preferencia = "Resistente",    PlantaRecomendada = "Suculenta Echeveria" },

                // Vidrio — Suculenta Echeveria
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 3f,  Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 7f,  Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 10f, Preferencia = "Decorativa",    PlantaRecomendada = "Suculenta Echeveria" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 5f,  Preferencia = "Resistente",    PlantaRecomendada = "Suculenta Echeveria" },
                // Vidrio — Lavanda Ecológica
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 2f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 6f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 9f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 4f,  Preferencia = "Interior",      PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Vidrio", CantidadKg = 8f,  Preferencia = "Interior",      PlantaRecomendada = "Lavanda Ecológica" },

                // Metal — Cactus San Pedro
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 5f,  Preferencia = "Resistente",    PlantaRecomendada = "Cactus San Pedro" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 10f, Preferencia = "Resistente",    PlantaRecomendada = "Cactus San Pedro" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 15f, Preferencia = "Resistente",    PlantaRecomendada = "Cactus San Pedro" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 20f, Preferencia = "Resistente",    PlantaRecomendada = "Cactus San Pedro" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 8f,  Preferencia = "Decorativa",    PlantaRecomendada = "Cactus San Pedro" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 12f, Preferencia = "Decorativa",    PlantaRecomendada = "Cactus San Pedro" },
                // Metal — Pothos Dorado
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 3f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 7f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 2f,  Preferencia = "Interior",      PlantaRecomendada = "Pothos Dorado" },
                new RecomendacionPlantaInput { TipoMaterial = "Metal", CantidadKg = 6f,  Preferencia = "Interior",      PlantaRecomendada = "Pothos Dorado" },

                // Papel — Helecho de Boston
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 3f,  Preferencia = "Interior",      PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 8f,  Preferencia = "Interior",      PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 12f, Preferencia = "Interior",      PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 5f,  Preferencia = "Fácil cuidado", PlantaRecomendada = "Helecho de Boston" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 10f, Preferencia = "Fácil cuidado", PlantaRecomendada = "Helecho de Boston" },
                // Papel — Lavanda Ecológica
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 4f,  Preferencia = "Decorativa",    PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 9f,  Preferencia = "Decorativa",    PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 15f, Preferencia = "Decorativa",    PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 6f,  Preferencia = "Resistente",    PlantaRecomendada = "Lavanda Ecológica" },
                new RecomendacionPlantaInput { TipoMaterial = "Papel", CantidadKg = 11f, Preferencia = "Resistente",    PlantaRecomendada = "Lavanda Ecológica" },
            };
        }
    }
}
