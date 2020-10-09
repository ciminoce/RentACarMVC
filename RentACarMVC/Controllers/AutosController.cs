using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Auto;

namespace RentACarMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AutosController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        private readonly IdentityDbContext _identityDbContext;

        public AutosController()
        {
            _dbContext=new RentACarDbContext();
            _identityDbContext=new IdentityDbContext();
        }
        // GET: Autos
        [AllowAnonymous]
        public ActionResult Index()
        {
            var listaAutos = _dbContext.Autos
                .Include(a=>a.Tipo)
                .Include(a=>a.Marca)
                .Include(a=>a.Modelo)
                .Include(a=>a.Usuario)
                .ToList();
            var listaVm = ConstruirListaAutosListViewModel(listaAutos);
            if (User.IsInRole("Admin"))
            {
                return View(listaVm);
               
            }
            else
            {
                return View("ReadOnlyIndex", listaVm);
            }
        }

        [Authorize(Users = "admin@example.com")]
        [HttpGet]
        public ActionResult Create()
        {
            var autoVm = new AutoEditViewModel
            {
                Tipos = _dbContext.Tipos.OrderBy(t=>t.Descripcion).ToList(),
                Marcas = _dbContext.Marcas.OrderBy(m=>m.NombreMarca).ToList(),
                Modelos = _dbContext.Modelos.OrderBy(m=>m.NombreModelo).ToList(),
                Usuarios = ConstruirListaChoferes()
            };
            return View(autoVm);

        }

        private List<Usuario> ConstruirListaChoferes()
        {
            var listaUsuarios = _dbContext.Usuarios.ToList();
            var listaChoferes=new List<Usuario>();
            foreach (var usuario in listaUsuarios)
            {
                if (UssersHelper.GetUserRole(usuario.NombreUsuario)=="Chofer")
                {
                    listaChoferes.Add(usuario);
                }
            }

            return listaChoferes;
        }

        private List<AutoListViewModel> ConstruirListaAutosListViewModel(List<Auto> listaAutos)
        {
            List<AutoListViewModel> listaVm=new List<AutoListViewModel>();
            foreach (var auto in listaAutos)
            {
                AutoListViewModel autoVm = new AutoListViewModel
                {
                    AutoId = auto.AutoId,
                    MovilId = auto.MovilId,
                    Patente = auto.Patente,
                    Tipo = auto.Tipo.Descripcion,
                    Marca = auto.Marca.NombreMarca,
                    Modelo = auto.Modelo.NombreModelo,
                    Chofer = auto.Usuario.NombreApellido,
                    Disponible = auto.Disponible

                };
                listaVm.Add(autoVm);
            }

            return listaVm;
        }
    }
}