using System.Collections.Generic;
using System.Linq;
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
                TipoListViewModel tipoVm = new TipoListViewModel
                {
                    TipoId = tipo.TipoId,
                    Descripcion = tipo.Descripcion
                };
                lista.Add(tipoVm);
            }

            return lista;
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
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError(string.Empty,"Registro repetido...");
                return View(tipoVm);
            }

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