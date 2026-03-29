using System;
using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models
{
    public class Reciclaje
    {
        public int Id { get; set; }

        [Required]
        public string TipoMaterial { get; set; } = "";

        public int Cantidad { get; set; }

        public int PuntosGanados { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}