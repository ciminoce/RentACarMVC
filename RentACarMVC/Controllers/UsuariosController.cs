using System;
using System.Linq;
using System.Web.Mvc;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Usuarios;

namespace RentACarMVC.Controllers
{
    [Authorize(Users = "admin@example.com")]
    public class UsuariosController : Controller
    {
        private readonly RentACarDbContext _dbContext;
        private readonly ApplicationDbContext _applicationDb;
        public UsuariosController()
        {
            _dbContext=new RentACarDbContext();
            _applicationDb=new ApplicationDbContext();
        }
        // GET: Usuarios
        public ActionResult Index()
        {
            var lista = _dbContext.Usuarios.ToList();
            return View(lista);
        }

        [HttpGet]
        public ActionResult Create()
        {
            UsuarioEditViewModel usuarioVm = new UsuarioEditViewModel
            {
                ListaRoles = _applicationDb.Roles.ToList()
            };
            return View(usuarioVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(UsuarioEditViewModel usuarioVm)
        {
            if (!ModelState.IsValid)
            {
                usuarioVm.ListaRoles = _applicationDb.Roles.ToList();

                return View(usuarioVm);
            }

            if (_dbContext.Usuarios.Any(e => e.DocumentoNro == usuarioVm.DocumentoNro))
            {
                ModelState.AddModelError(string.Empty, "Nro. de documento repetido");
                usuarioVm.ListaRoles = _applicationDb.Roles.ToList();

                return View(usuarioVm);
            }
            if (_dbContext.Usuarios.Any(e => e.NombreUsuario == usuarioVm.NombreUsuario))
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario repetido");
                usuarioVm.ListaRoles = _applicationDb.Roles.ToList();

                return View(usuarioVm);

            }

            Usuario empleado = ConstruirUsuario(usuarioVm);
            using (var tran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Usuarios.Add(empleado);
                    _dbContext.SaveChanges();
                    UssersHelper.CreateUserASP(usuarioVm.NombreUsuario, usuarioVm.Rol);
                    tran.Commit();
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {

                    tran.Rollback();
                    ModelState.AddModelError(string.Empty, ex.Message);
                    usuarioVm.ListaRoles = _applicationDb.Roles.ToList();
                    return View(usuarioVm);
                }
            }


        }

        private Usuario ConstruirUsuario(UsuarioEditViewModel usuarioVm)
        {
            return new Usuario
            {
                UsuarioId = usuarioVm.UsuarioId,
                NombreUsuario = usuarioVm.NombreUsuario,
                Nombres = usuarioVm.Nombres,
                Apellido = usuarioVm.Apellido,
                MovilNumero = usuarioVm.MovilNumero,
                FechaIngreso = usuarioVm.FechaIngreso,
                DocumentoNro = usuarioVm.DocumentoNro,
                Direccion = usuarioVm.Direccion
            };
        }
    }
}