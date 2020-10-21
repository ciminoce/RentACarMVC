using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.EntityFramework;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.Models.Enums;
using RentACarMVC.ViewModels.Solicitud;
using RentACarMVC.ViewModels.Viaje;

namespace RentACarMVC.Controllers
{
    public class ViajesController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        private readonly IdentityDbContext _identityDbContext;

        public ViajesController()
        {
            _dbContext=new RentACarDbContext();
            _identityDbContext=new IdentityDbContext();
        }

        [Authorize(Roles = "Admin, Cajero")]
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

        [Authorize(Roles = "Admin, Cajero")]
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ViajeEditViewModel viajeVm)
        {
            if (!ModelState.IsValid)
            {
                viajeVm.Autos = _dbContext.Autos.Where(a => a.Disponible).ToList();
                viajeVm.Clientes = CargarListaClientes();

                return View(viajeVm);
            }

            Viaje viaje = new Viaje
            {
                ViajeId = viajeVm.ViajeId,
                AutoId = viajeVm.AutoId,
                UsuarioId = viajeVm.UsuarioId,
                FechaHoraSalida = viajeVm.FechaHoraSalida,
                Estado = EstadoViaje.EnProceso
            };
            if (_dbContext.Viajes.Any(v=>v.UsuarioId==viaje.UsuarioId && 
                                         v.Estado==EstadoViaje.EnProceso))
            {
                viajeVm.Autos = _dbContext.Autos.Where(a => a.Disponible).ToList();
                viajeVm.Clientes = CargarListaClientes();

                ModelState.AddModelError(string.Empty,"Error: Usuario con viaje sin terminar...");
                return View(viajeVm);
            }

            using (var tran=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Viajes.Add(viaje);
                    var autoEnViaje = _dbContext.Autos.SingleOrDefault(a => a.AutoId == viaje.AutoId);
                    if (autoEnViaje == null)
                    {
                        viajeVm.Autos = _dbContext.Autos.Where(a => a.Disponible).ToList();
                        viajeVm.Clientes = CargarListaClientes();

                        ModelState.AddModelError(String.Empty, "Joder... estamos en problemas");
                        return View(viajeVm);
                    }

                    autoEnViaje.Disponible = false;
                    _dbContext.Entry(autoEnViaje).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    tran.Commit();
                    TempData["Msg"] = "Viaje guardado...";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    viajeVm.Autos = _dbContext.Autos.Where(a => a.Disponible).ToList();
                    viajeVm.Clientes = CargarListaClientes();

                    ModelState.AddModelError(String.Empty, "Error al intentar guardar un viaje...");
                    tran.Rollback();
                    return View(viajeVm);
                }

            }
        }

        [Authorize(Roles = "Admin, Cajero")]
        [HttpGet]
        public ActionResult Suspend(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Viaje viaje = _dbContext.Viajes
                .Include(v=>v.Auto)
                .Include(v=>v.Usuario)
                .SingleOrDefault(v => v.ViajeId == id);
            if (viaje==null && viaje.Estado!=EstadoViaje.EnProceso)
            {
                return HttpNotFound("Código de viaje erróneo o viaje no En Proceso...");
            }

            var viajeVm = new ViajeListViewModel
            {
                ViajeId = viaje.ViajeId,
                MovilId = viaje.Auto.MovilId,
                Cliente = viaje.Usuario.NombreApellido,
                FechaHoraSalida = viaje.FechaHoraSalida,
                EstadoViaje = viaje.Estado
            };
            return View(viajeVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Suspend(ViajeListViewModel viajeVm)
        {
            var viajeEnProceso = _dbContext.Viajes.
                SingleOrDefault(v => v.ViajeId == viajeVm.ViajeId);
            viajeEnProceso.Estado = EstadoViaje.Suspendido;
            using (var tran=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Entry(viajeEnProceso).State = EntityState.Modified;
                    var autoEnViaje = _dbContext.Autos.SingleOrDefault(a => a.AutoId == viajeEnProceso.AutoId);
                    autoEnViaje.Disponible = true;
                    _dbContext.Entry(autoEnViaje).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    tran.Commit();
                    TempData["Msg"] = "Viaje suspendido...";
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {
                    ModelState.AddModelError(String.Empty, "Error al intentar suspender un viaje");
                    tran.Rollback();
                    return View(viajeVm);

                }
            }
        }

        [Authorize(Roles = "Admin, Cajero")]
        [HttpGet]
        public ActionResult Finish(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Viaje viajeEnProceso = _dbContext.Viajes
                .Include(v=>v.Auto)
                .Include(v=>v.Usuario)
                .SingleOrDefault(v => v.ViajeId == id);
            if (viajeEnProceso==null || viajeEnProceso.Estado!=EstadoViaje.EnProceso)
            {
                return HttpNotFound("Código de viaje erróneo o viaje no En Proceso");
            }
            ViajeFinishViewModel viajeVm = new ViajeFinishViewModel()
            {
                ViajeId = viajeEnProceso.ViajeId,
                MovilId = viajeEnProceso.Auto.MovilId,
                Cliente = viajeEnProceso.Usuario.NombreApellido,
                FechaHoraSalida = viajeEnProceso.FechaHoraSalida,
                EstadoViaje = viajeEnProceso.Estado
            };
            return View(viajeVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Finish(ViajeFinishViewModel viajeVm)
        {
            if (!ModelState.IsValid)
            {
                return View(viajeVm);
            }

            Viaje viajeEnProceso = _dbContext.Viajes
                .SingleOrDefault(v => v.ViajeId == viajeVm.ViajeId);
            viajeEnProceso.Estado = EstadoViaje.Finalizado;
            viajeEnProceso.FechaHoraLlegada = viajeVm.FechaHoraLlegada;
            viajeEnProceso.Costo = viajeVm.Costo;
            using (var tran=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Entry(viajeEnProceso).State = EntityState.Modified;
                    var autoEnViaje = _dbContext.Autos
                        .SingleOrDefault(a => a.AutoId == viajeEnProceso.AutoId);
                    autoEnViaje.Disponible = true;
                    _dbContext.Entry(autoEnViaje).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    tran.Commit();
                    TempData["Msg"] = "Viaje finalizado";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {

                   ModelState.AddModelError(string.Empty,"Error al intentar finalizar un viaje");
                   tran.Rollback();
                   return View(viajeVm);
                }

            }
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
                    Costo = viaje.Costo,
                    EstadoViaje = viaje.Estado

                };
                listaVm.Add(viajeVm);
            }

            return listaVm;
        }

        [Authorize(Roles = "Cliente")]
        public ActionResult MyRides()
        {

            var listaViajes = _dbContext.Viajes
                .Include(v => v.Auto)
                .Include(v => v.Usuario)
                .Where(v => v.Usuario.NombreUsuario == User.Identity.Name)
                .ToList();
            var listaVm = ConstruirListaViajeListViewModel(listaViajes);
            return View(listaVm);
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        public ActionResult RequestRide()
        {
            var usuario = _dbContext.Usuarios
                .SingleOrDefault(u => u.NombreUsuario == User.Identity.Name);

            var solicitud = new Solicitud
            {
                UsuarioId = usuario.UsuarioId,
                FechaHora = DateTime.Now,
                EstadoSolicitud = EstadoSolicitud.Pendiente
            };

            if (!_dbContext.Solicitudes.Any(s=>s.UsuarioId==usuario.UsuarioId && 
                                              s.EstadoSolicitud==EstadoSolicitud.Pendiente))
            {
                try
                {
                    _dbContext.Solicitudes.Add(solicitud);
                    _dbContext.SaveChanges();
                    TempData["Msg"] = "Solicitud cargada";
                    return RedirectToAction("MyRequests");
                }
                catch (Exception ex)
                {
                    TempData["Msg"] = "Error al cargar una solicitud";
                    return RedirectToAction("MyRides");
                }

            }
            else
            {
                TempData["Msg"] = "Error: No se puede cargar otra solicitud por tener una pendiente";
                return RedirectToAction("MyRides");
            }
        }

        [Authorize(Roles = "Cliente")]
        public ActionResult MyRequests()
        {
            var usuario = _dbContext.Usuarios
                .SingleOrDefault(u => u.NombreUsuario == User.Identity.Name);
            var listaSolicitudes = _dbContext.Solicitudes
                .Where(s => s.Usuario.UsuarioId == usuario.UsuarioId)
                .ToList();
            var listaVm = ConstruirListaSolicitudesViewModel(listaSolicitudes);
            return View(listaVm);
        }

        private List<SolicitudListViewModel> ConstruirListaSolicitudesViewModel(List<Solicitud> listaSolicitudes)
        {
            var lista=new List<SolicitudListViewModel>();
            foreach (var solicitud in listaSolicitudes)
            {
                var solicitudVm = new SolicitudListViewModel
                {
                    SolicitudId = solicitud.SolicitudId,
                    FechaHora = solicitud.FechaHora,
                    EstadoSolicitud = solicitud.EstadoSolicitud
                };
                lista.Add(solicitudVm);
            }

            return lista;
        }
    }
}