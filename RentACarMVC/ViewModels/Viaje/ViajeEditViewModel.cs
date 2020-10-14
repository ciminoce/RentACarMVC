using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.ViewModels.Viaje
{
    public class ViajeEditViewModel
    {
        public int ViajeId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un móvil")]
        [Display(Name = "ID Móvil")]
        public int AutoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Cliente")]
        [Display(Name = "Cliente")]

        public int UsuarioId { get; set; }

        [Display(Name = "Fecha y Hora Salida:")]
        public DateTime FechaHoraSalida { get; set; }

        [Display(Name = "Fecha y Hora Llegada:")]
        public DateTime? FechaHoraLlegada { get; set; }

        public decimal? Costo { get; set; }
        public EstadoViaje Estado { get; set; }

        public List<Models.Auto> Autos { get; set; }
        public List<Usuario> Clientes { get; set; }

    }
}