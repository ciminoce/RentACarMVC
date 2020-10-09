using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Usuarios
{
    public class UsuarioListViewModel
    {
        public int UsuarioId { get; set; }
        public string Nombres { get; set; }
        public string Apellido { get; set; }

        [Display(Name = "Celular")]
        public string MovilNumero { get; set; }

        [Display(Name="Dirección")]
        public string Direccion { get; set; }

    }
}