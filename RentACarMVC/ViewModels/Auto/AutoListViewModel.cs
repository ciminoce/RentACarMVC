using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models;

namespace RentACarMVC.ViewModels.Auto
{
    public class AutoListViewModel
    {
        public int AutoId { get; set; }

        [Display(Name = "ID Móvil")]
        public string MovilId { get; set; }
        public string Patente { get; set; }
        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Chofer { get; set; }
        public bool Disponible { get; set; }

    }
}