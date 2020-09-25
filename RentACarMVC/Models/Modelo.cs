namespace RentACarMVC.Models
{
    public class Modelo
    {
        public int ModeloId { get; set; }
        public int MarcaId { get; set; }
        public Marca Marca { get; set; }
        public int TipoId { get; set; }
        public Tipo Tipo { get; set; }
        public string NombreModelo { get; set; }
        public string Foto { get; set; }
    }
}