using System.ComponentModel.DataAnnotations;

namespace RentACarMVC.ViewModels.Modelo
{
    public class ModeloListViewModel
    {
        public int ModeloId { get; set; }
        public string Marca { get; set; }
        public string Tipo { get; set; }

        [Display(Name = "Modelo")]
        public string NombreModelo { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Foto { get; set; }

    }
}