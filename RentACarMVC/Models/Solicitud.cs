using System;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.Models
{
    public class Solicitud
    {
        public int SolicitudId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoSolicitud EstadoSolicitud { get; set; }
        public string Comentario { get; set; }
    }
}