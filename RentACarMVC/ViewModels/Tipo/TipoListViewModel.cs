using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Tipo
{
    public class TipoListViewModel
    {
        public int TipoId { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}