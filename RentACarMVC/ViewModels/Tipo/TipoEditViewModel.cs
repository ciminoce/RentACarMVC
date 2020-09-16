using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Tipo
{
    public class TipoEditViewModel
    {
        public int TipoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30,ErrorMessage = "El campo {0} debe contener entre {2} y {1} caracteres",MinimumLength =3)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

    }
}