using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Marca;

namespace RentACarMVC.Controllers
{
    public class MarcasController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        public MarcasController()
        {
            _dbContext=new RentACarDbContext();
        }
        // GET: Marcas
        public ActionResult Index()
        {
            var listaMarca = _dbContext.Marcas.ToList();
            var listaVm = ConstruirListaVm(listaMarca);
            return View(listaVm);
        }

        private List<MarcaListViewModel> ConstruirListaVm(List<Marca> listaMarca)
        {
            var lista = new List<MarcaListViewModel>();
            foreach (var marca in listaMarca)
            {
                var vm = new MarcaListViewModel
                {
                    MarcaId = marca.MarcaId,
                    NombreMarca = marca.NombreMarca,
                    Logo = marca.Logo
                };
                lista.Add(vm);
            }

            return lista;
        }
    }
}