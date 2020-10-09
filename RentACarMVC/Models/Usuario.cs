using System;

namespace RentACarMVC.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public string DocumentoNro { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string NombreUsuario { get; set; }
        public string MovilNumero { get; set; }
        public string Direccion { get; set; }

        public string NombreApellido => $"{Nombres} {Apellido}";
    }
}