﻿using System;
using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models.Enums;

namespace RentACarMVC.ViewModels.Viaje
{
    public class ViajeListViewModel
    {
        public int ViajeId { get; set; }
        [Display(Name = "ID Móvil")]
        public string MovilId { get; set; }
        public string Cliente { get; set; }

        [Display(Name = "Fecha y Hora de Salida")]
        public DateTime FechaHoraSalida { get; set; }
        [Display(Name = "Fecha y Hora de Llegada")]
        public DateTime? FechaHoraLlegada { get; set; }
        public decimal? Costo { get; set; }
        public EstadoViaje EstadoViaje { get; set; }
    }
}