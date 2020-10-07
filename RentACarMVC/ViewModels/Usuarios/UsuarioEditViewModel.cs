using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Usuarios
{
    public class UsuarioEditViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(256, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress)]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(10, ErrorMessage = "The filed {0} must be maximun {1} characters length")]
        [Display(Name = "Nro. Documento")]
        public string DocumentoNro { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Ingreso")]
        public System.DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(30, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Móvil Nro.")]

        public string MovilNumero { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Rol { get; set; }
        public List<IdentityRole> ListaRoles { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(256, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

    }
}