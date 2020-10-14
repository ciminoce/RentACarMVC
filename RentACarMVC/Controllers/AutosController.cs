using System;
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
            _dbContext = new RentACarDbContext();
            _identityDbContext = new IdentityDbContext();
        }
        // GET: Autos
        [AllowAnonymous]
        public ActionResult Index()
        {
            var listaAutos = _dbContext.Autos
                .Include(a => a.Tipo)
                .Include(a => a.Marca)
                .Include(a => a.Modelo)
                .Include(a => a.Usuario)
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
                MovilId = GetMovilId(),
                Tipos = _dbContext.Tipos.OrderBy(t => t.Descripcion).ToList(),
                Marcas = _dbContext.Marcas.OrderBy(m => m.NombreMarca).ToList(),
                Modelos = _dbContext.Modelos.OrderBy(m => m.NombreModelo).ToList(),
                Usuarios = ConstruirListaChoferes()
            };
            return View(autoVm);

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(AutoEditViewModel autoVm)
        {
            if (!ModelState.IsValid)
            {
                autoVm.Tipos = _dbContext.Tipos.OrderBy(t => t.Descripcion).ToList();
                autoVm.Marcas = _dbContext.Marcas.OrderBy(m => m.NombreMarca).ToList();
                autoVm.Modelos = _dbContext.Modelos.OrderBy(m => m.NombreModelo).ToList();
                autoVm.Usuarios = ConstruirListaChoferes();

                return View(autoVm);
            }

            Auto auto = ConstruirAuto(autoVm);
            if (_dbContext.Autos.Any(a => a.Patente == auto.Patente))
            {
                autoVm.Tipos = _dbContext.Tipos.OrderBy(t => t.Descripcion).ToList();
                autoVm.Marcas = _dbContext.Marcas.OrderBy(m => m.NombreMarca).ToList();
                autoVm.Modelos = _dbContext.Modelos.OrderBy(m => m.NombreModelo).ToList();
                autoVm.Usuarios = ConstruirListaChoferes();
                ModelState.AddModelError(string.Empty, "Auto con patente repetida!!");
                return View(autoVm);
            }

            try
            {
                _dbContext.Autos.Add(auto);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Auto agregado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                autoVm.Tipos = _dbContext.Tipos.OrderBy(t => t.Descripcion).ToList();
                autoVm.Marcas = _dbContext.Marcas.OrderBy(m => m.NombreMarca).ToList();
                autoVm.Modelos = _dbContext.Modelos.OrderBy(m => m.NombreModelo).ToList();
                autoVm.Usuarios = ConstruirListaChoferes();

                ModelState.AddModelError(string.Empty, "Error al intentar agregar un auto!!");
                return View(autoVm);
            }
        }

        private Auto ConstruirAuto(AutoEditViewModel autoVm)
        {
            return new Auto
            {
                AutoId = autoVm.AutoId,
                MovilId = autoVm.MovilId,
                Patente = autoVm.Patente,
                TipoId = autoVm.TipoId,
                MarcaId = autoVm.MarcaId,
                ModeloId = autoVm.ModeloId,
                UsuarioId = autoVm.UsuarioId,
                Activo = true,
                Disponible = true
            };
        }

        private string GetMovilId()
        {
            var movilId = _dbContext.Autos
                .OrderByDescending(a => a.AutoId)
                .Select(a => a.MovilId)
                .First();
            var nroId = int.Parse(movilId.Substring(2)) + 1;

            return $"MO{nroId.ToString().PadLeft(4, '0')}";
        }

        private List<Usuario> ConstruirListaChoferes()
        {
            var listaUsuarios = _dbContext.Usuarios.ToList();
            var listaChoferes = new List<Usuario>();
            foreach (var usuario in listaUsuarios)
            {
                if (UssersHelper.GetUserRole(usuario.NombreUsuario) == "Chofer")
                {
                    listaChoferes.Add(usuario);
                }
            }

            return listaChoferes;
        }

        private List<AutoListViewModel> ConstruirListaAutosListViewModel(List<Auto> listaAutos)
        {
            List<AutoListViewModel> listaVm = new List<AutoListViewModel>();
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

        public JsonResult GetModelos(int tipoId, int marcaId)
        {
            _dbContext.Configuration.ProxyCreationEnabled = false;
            var modelos = _dbContext.Modelos.Where(m => m.TipoId == tipoId && m.MarcaId == marcaId);
            return Json(modelos);
        }

    }
}