namespace RentACarMVC.Models
{
    public class Auto
    {
        public int AutoId { get; set; }
        public string MovilId { get; set; }
        public string Patente { get; set; }
        public int TipoId { get; set; }
        public Tipo Tipo { get; set; }
        public int MarcaId { get; set; }
        public Marca Marca { get; set; }
        public int ModeloId { get; set; }
        public Modelo Modelo { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public bool Disponible { get; set; }
        public bool Activo { get; set; }
    }
}