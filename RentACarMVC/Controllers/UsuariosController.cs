using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Usuarios;

namespace RentACarMVC.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public ActionResult Index(string roleName=null)
        {
            var lista = _dbContext.Usuarios.ToList();
            var listaVm = ConstruirListaUsuarioListViewModel(lista, roleName);
            TempData["Rol"] = "Usuarios";
            if (roleName=="Chofer")
            {
                TempData["Rol"] = "Choferes";
            }
            else if(roleName=="Cliente")
            {
                TempData["Rol"] = "Clientes";
            }
            return View(listaVm);
        }

        private List<UsuarioListViewModel> ConstruirListaUsuarioListViewModel(List<Usuario> lista,string roleName)
        {
            List<UsuarioListViewModel> listaVm=new List<UsuarioListViewModel>();
            foreach (var usuario in lista)
            {
                if (!string.IsNullOrEmpty(roleName))
                {
                    if (UssersHelper.GetUserRole(usuario.NombreUsuario)==roleName)
                    {
                        UsuarioListViewModel usuarioVm = ConstruirListaUsuarioListViewModel(usuario);
                        listaVm.Add(usuarioVm);
                    }
                }
                else
                {
                     UsuarioListViewModel usuarioVm = ConstruirListaUsuarioListViewModel(usuario);
                    listaVm.Add(usuarioVm);
                   
                }
            }

            return listaVm;
        }

        private UsuarioListViewModel ConstruirListaUsuarioListViewModel(Usuario usuario)
        {
            return new UsuarioListViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nombres = usuario.Nombres,
                Apellido = usuario.Apellido,
                MovilNumero = usuario.MovilNumero,
                Direccion = usuario.Direccion
            };
        }

        [Authorize(Users = "admin@example.com")]
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