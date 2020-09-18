using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Marca
{
    public class MarcaListViewModel
    {
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caracteres", MinimumLength = 3)]
        [Display(Name = "Marca")]

        public string NombreMarca { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

    }
}