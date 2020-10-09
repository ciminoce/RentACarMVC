using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RentACarMVC.Models;

namespace RentACarMVC.ViewModels.Auto
{
    public class AutoEditViewModel
    {
        public int AutoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "ID Móvil")]
        [MaxLength(6, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        public string MovilId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(7, ErrorMessage = "El campo {0} debe contener no más de {1} caracteres")]
        public string Patente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo")]
        [Display(Name = "Tipo")]
        public int TipoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una marca")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un modelo")]
        [Display(Name = "Modelo")]
        public int ModeloId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un chofer")]
        [Display(Name = "Chofer")]
        public int UsuarioId { get; set; }

        public List<Models.Tipo> Tipos { get; set; }
        public List<Models.Marca> Marcas { get; set; }
        public List<Models.Modelo> Modelos { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}