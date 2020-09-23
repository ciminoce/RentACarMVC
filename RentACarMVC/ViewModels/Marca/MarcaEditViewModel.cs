using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RentACarMVC.ViewModels.Marca
{
    public class MarcaEditViewModel
    {
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caracteres", MinimumLength = 2)]
        [Display(Name = "Marca")]

        public string NombreMarca { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [Display(Name = "Logo")]
        public HttpPostedFileBase LogoFile { get; set; }

    }
}