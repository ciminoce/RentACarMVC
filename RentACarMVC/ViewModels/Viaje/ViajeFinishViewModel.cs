using System;
using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.ViewModels.Viaje
{
    public class ViajeFinishViewModel
    {
        public int ViajeId { get; set; }
        [Display(Name = "ID Móvil")]
        public string MovilId { get; set; }
        public string Cliente { get; set; }

        [Display(Name = "Fecha y Hora de Salida")]
        public DateTime FechaHoraSalida { get; set; }

        [Display(Name = "Fecha y Hora de Llegada")]
        public DateTime FechaHoraLlegada { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,int.MaxValue,ErrorMessage = "El costo debe estar comprendido entre {1} y {2}")]
        public decimal Costo { get; set; }

        public EstadoViaje EstadoViaje { get; set; }

    }
}