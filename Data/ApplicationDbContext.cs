using Microsoft.EntityFrameworkCore;
using EcoPlantas.Models;

namespace EcoPlantas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Reciclaje> Reciclajes { get; set; }
        public DbSet<ProductoEco> Productos { get; set; }
        public DbSet<ContactoMensaje> ContactoMensajes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
