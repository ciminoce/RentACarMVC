using System;
using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.ViewModels.Solicitud
{
    public class SolicitudListViewModel
    {
        [Display(Name="Solicitud Nro.")]
        public int SolicitudId { get; set; }

        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; }

        [Display(Name = "Estado")]
        public EstadoSolicitud EstadoSolicitud { get; set; }

    }
}