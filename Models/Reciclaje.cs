using System;

namespace EcoPlantas.Models
{
    public class Reciclaje
    {
        public int Id { get; set; }
        public string TipoMaterial { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}