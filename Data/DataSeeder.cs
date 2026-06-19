using EcoPlantas.Models;

namespace EcoPlantas.Data
{
    public static class DataSeeder
    {
        // Rutas locales de imágenes (SVG en wwwroot/images/productos). No dependen de servicios externos.
        private static readonly Dictionary<string, string> ImagenesPorNombre = new()
        {
            ["Pothos Dorado"]        = "/images/productos/pothos-dorado.svg",
            ["Suculenta Echeveria"]  = "/images/productos/suculenta-echeveria.svg",
            ["Helecho de Boston"]    = "/images/productos/helecho-boston.svg",
            ["Cactus San Pedro"]     = "/images/productos/cactus-san-pedro.svg",
            ["Lavanda Ecológica"]    = "/images/productos/lavanda-ecologica.svg",
            ["Aloe Vera Orgánico"]   = "/images/productos/aloe-vera-organico.svg"
        };

        public static void Seed(ApplicationDbContext context)
        {
            SeedProductos(context);
            ActualizarImagenesProductos(context);
            SeedUsuarios(context);
        }

        private static void SeedProductos(ApplicationDbContext context)
        {
            if (context.Productos.Any()) return;

            context.Productos.AddRange(
                new ProductoEco { Nombre = "Pothos Dorado", Categoria = "Planta Interior", Descripcion = "Planta trepadora de fácil cuidado, purifica el aire.", Precio = 12.99m, ImagenUrl = ImagenesPorNombre["Pothos Dorado"], Disponible = true },
                new ProductoEco { Nombre = "Suculenta Echeveria", Categoria = "Suculenta", Descripcion = "Roseta compacta de hojas carnosas, muy resistente.", Precio = 8.50m, ImagenUrl = ImagenesPorNombre["Suculenta Echeveria"], Disponible = true },
                new ProductoEco { Nombre = "Helecho de Boston", Categoria = "Planta Interior", Descripcion = "Helecho frondoso, ideal para humidificar espacios.", Precio = 15.00m, ImagenUrl = ImagenesPorNombre["Helecho de Boston"], Disponible = true },
                new ProductoEco { Nombre = "Cactus San Pedro", Categoria = "Cactus", Descripcion = "Cactus columnar de crecimiento rápido, muy decorativo.", Precio = 20.00m, ImagenUrl = ImagenesPorNombre["Cactus San Pedro"], Disponible = true },
                new ProductoEco { Nombre = "Lavanda Ecológica", Categoria = "Aromáticas", Descripcion = "Planta aromática con propiedades relajantes.", Precio = 10.75m, ImagenUrl = ImagenesPorNombre["Lavanda Ecológica"], Disponible = true },
                new ProductoEco { Nombre = "Aloe Vera Orgánico", Categoria = "Medicinal", Descripcion = "Planta medicinal multipropósito, crece sin pesticidas.", Precio = 9.99m, ImagenUrl = ImagenesPorNombre["Aloe Vera Orgánico"], Disponible = true }
            );
            context.SaveChanges();
        }

        // Corrige de forma idempotente las imágenes de productos ya existentes en la BD
        // (por ejemplo, los que se sembraron antes con URLs externas rotas de via.placeholder).
        // Solo actualiza datos; no modifica el esquema.
        private static void ActualizarImagenesProductos(ApplicationDbContext context)
        {
            var huboCambios = false;
            foreach (var producto in context.Productos.ToList())
            {
                if (ImagenesPorNombre.TryGetValue(producto.Nombre, out var ruta)
                    && producto.ImagenUrl != ruta)
                {
                    producto.ImagenUrl = ruta;
                    huboCambios = true;
                }
            }
            if (huboCambios) context.SaveChanges();
        }

        private static void SeedUsuarios(ApplicationDbContext context)
        {
            if (context.Usuarios.Any()) return;

            context.Usuarios.Add(new Usuario
            {
                Email = "admin@ecoplantas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = "Admin",
                Activo = true
            });
            context.SaveChanges();
        }
    }
}
