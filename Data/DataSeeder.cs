using EcoPlantas.Models;

namespace EcoPlantas.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            SeedProductos(context);
            SeedUsuarios(context);
        }

        private static void SeedProductos(ApplicationDbContext context)
        {
            if (context.Productos.Any()) return;

            context.Productos.AddRange(
                new ProductoEco { Nombre = "Pothos Dorado", Categoria = "Planta Interior", Descripcion = "Planta trepadora de fácil cuidado, purifica el aire.", Precio = 12.99m, ImagenUrl = "https://via.placeholder.com/300x200/2d6a4f/white?text=Pothos", Disponible = true },
                new ProductoEco { Nombre = "Suculenta Echeveria", Categoria = "Suculenta", Descripcion = "Roseta compacta de hojas carnosas, muy resistente.", Precio = 8.50m, ImagenUrl = "https://via.placeholder.com/300x200/52b788/white?text=Suculenta", Disponible = true },
                new ProductoEco { Nombre = "Helecho de Boston", Categoria = "Planta Interior", Descripcion = "Helecho frondoso, ideal para humidificar espacios.", Precio = 15.00m, ImagenUrl = "https://via.placeholder.com/300x200/1b4332/white?text=Helecho", Disponible = true },
                new ProductoEco { Nombre = "Cactus San Pedro", Categoria = "Cactus", Descripcion = "Cactus columnar de crecimiento rápido, muy decorativo.", Precio = 20.00m, ImagenUrl = "https://via.placeholder.com/300x200/40916c/white?text=Cactus", Disponible = true },
                new ProductoEco { Nombre = "Lavanda Ecológica", Categoria = "Aromáticas", Descripcion = "Planta aromática con propiedades relajantes.", Precio = 10.75m, ImagenUrl = "https://via.placeholder.com/300x200/74c69d/white?text=Lavanda", Disponible = true },
                new ProductoEco { Nombre = "Aloe Vera Orgánico", Categoria = "Medicinal", Descripcion = "Planta medicinal multipropósito, crece sin pesticidas.", Precio = 9.99m, ImagenUrl = "https://via.placeholder.com/300x200/b7e4c7/black?text=Aloe", Disponible = true }
            );
            context.SaveChanges();
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
