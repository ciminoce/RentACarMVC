using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RentACarMVC.ViewModels.Modelo
{
    public class ModeloEditViewModel
    {
        public int ModeloId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,int.MaxValue,ErrorMessage = "Debe seleccionar una marca")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1,int.MaxValue,ErrorMessage = "Debe seleccionar un tipo")]
        [Display(Name = "Tipo")]

        public int TipoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Modelo")]
        [StringLength(20,ErrorMessage = "El campo {0} debe contener entre {2} y {1} caracteres",MinimumLength = 1)]
        public string NombreModelo { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Foto { get; set; }

        [Display(Name = "Foto")]
        public HttpPostedFileBase FotoFile { get; set; }

        public List<Models.Marca> Marcas { get; set; }
        public List<Models.Tipo> Tipos { get; set; }

    }
}