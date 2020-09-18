using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Tipo;

namespace RentACarMVC.Controllers
{
    public class TiposController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        public TiposController()
        {
            _dbContext=new RentACarDbContext();
        }
        // GET: Tipos
        public ActionResult Index()
        {
            var listaTipos = _dbContext.Tipos.ToList();
            var listaVm = ConstruirListaVm(listaTipos);
            return View(listaVm);
        }

        private List<TipoListViewModel> ConstruirListaVm(List<Tipo> listaTipos)
        {
            List<TipoListViewModel> lista=new List<TipoListViewModel>();
            foreach (var tipo in listaTipos)
            {
                var tipoVm = ConstruirTipoListVm(tipo);
                lista.Add(tipoVm);
            }

            return lista;
        }

        private static TipoListViewModel ConstruirTipoListVm(Tipo tipo)
        {
            TipoListViewModel tipoVm = new TipoListViewModel
            {
                TipoId = tipo.TipoId,
                Descripcion = tipo.Descripcion
            };
            return tipoVm;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(TipoEditViewModel tipoVm)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoVm);
            }

            var tipo = ConstruirTipo(tipoVm);

            if (!_dbContext.Tipos.Any(t=>t.Descripcion==tipoVm.Descripcion))
            {
                _dbContext.Tipos.Add(tipo);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro agregado";

                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError(string.Empty,"Registro repetido...");
                return View(tipoVm);
            }

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tipo = _dbContext.Tipos.SingleOrDefault(t => t.TipoId == id);
            if (tipo==null)
            {
                return HttpNotFound();
            }

            var tipoVm = ConstruirTipoListVm(tipo);
            return View(tipoVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var tipo = _dbContext.Tipos.SingleOrDefault(t => t.TipoId == id);
            try
            {
                _dbContext.Tipos.Remove(tipo);
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro eliminado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var tipoVm = ConstruirTipoListVm(tipo);

                ModelState.AddModelError(string.Empty,"Error al intentar dar de baja un registro");
                return View(tipoVm);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tipo = _dbContext.Tipos.SingleOrDefault(t => t.TipoId == id);
            if (tipo==null)
            {
                return HttpNotFound();
            }

            TipoEditViewModel tipoVm = ConstruirTipoEditVm(tipo);
            return View(tipoVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(TipoEditViewModel tipoVm)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoVm);
            }

            var tipo = ConstruirTipo(tipoVm);
            try
            {
                if (_dbContext.Tipos.Any(t=>t.Descripcion==tipo.Descripcion && t.TipoId!=tipo.TipoId))
                {
                    ModelState.AddModelError(string.Empty,"Registro repetido");
                    return View(tipoVm);
                }

                _dbContext.Entry(tipo).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Error inesperado al intentar editar un registro");
                return View(tipoVm);
            }
        }
        private TipoEditViewModel ConstruirTipoEditVm(Tipo tipo)
        {
            return new TipoEditViewModel
            {
                TipoId = tipo.TipoId,
                Descripcion = tipo.Descripcion
            };
        }

        private Tipo ConstruirTipo(TipoEditViewModel tipoVm)
        {
            return new Tipo
            {
                TipoId = tipoVm.TipoId,
                Descripcion = tipoVm.Descripcion
            };
        }
    }
}