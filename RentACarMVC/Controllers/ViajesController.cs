using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Viaje;

namespace RentACarMVC.Controllers
{
    [Authorize(Roles = "Admin, Cajero")]
    public class ViajesController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        private readonly IdentityDbContext _identityDbContext;

        public ViajesController()
        {
            _dbContext=new RentACarDbContext();
            _identityDbContext=new IdentityDbContext();
        }
        // GET: Viajes
        public ActionResult Index()
        {
            var listaViajes = _dbContext.Viajes
                .Include(v=>v.Auto)
                .Include(v=>v.Usuario)
                .ToList();
            var listaVm = ConstruirListaViajeListViewModel(listaViajes);   
            return View(listaVm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViajeEditViewModel viajeVm = new ViajeEditViewModel
            {
                Autos = _dbContext.Autos.Where(a => a.Disponible).ToList(),
                Clientes = CargarListaClientes()
            };
            return View(viajeVm);
        }

        private List<Usuario> CargarListaClientes()
        {
            List<Usuario> clientes=new List<Usuario>();
            var lista = _dbContext.Usuarios.ToList();
            foreach (var usuario in lista)
            {
                if (UssersHelper.GetUserRole(usuario.NombreUsuario)=="Cliente")
                {
                    clientes.Add(usuario);
                }
            }

            return clientes;

        }

        private List<ViajeListViewModel> ConstruirListaViajeListViewModel(List<Viaje> listaViajes)
        {
            List<ViajeListViewModel> listaVm=new List<ViajeListViewModel>();
            foreach (var viaje in listaViajes)
            {
                ViajeListViewModel viajeVm = new ViajeListViewModel
                {
                    ViajeId = viaje.ViajeId,
                    MovilId = viaje.Auto.MovilId,
                    Cliente = viaje.Usuario.NombreApellido,
                    FechaHoraSalida = viaje.FechaHoraSalida,
                    FechaHoraLlegada = viaje.FechaHoraLlegada,
                    Costo = viaje.Costo
                };
                listaVm.Add(viajeVm);
            }

            return listaVm;
        }
    }
}