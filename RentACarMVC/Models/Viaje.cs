using System;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.Models
{
    public class Viaje
    {
        public int ViajeId { get; set; }
        public int AutoId { get; set; }
        public Auto Auto { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public decimal? Costo { get; set; }
        public EstadoViaje Estado { get; set; }
    }
}