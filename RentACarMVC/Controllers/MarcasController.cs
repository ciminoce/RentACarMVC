using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels;
using RentACarMVC.ViewModels.Marca;

namespace RentACarMVC.Controllers
{
    [Authorize]
    public class MarcasController : Controller
    {
        private readonly RentACarDbContext _dbContext;
        private readonly int _registrosPorPagina = 6;
        private Listador<MarcaListViewModel> _listador;
        public MarcasController()
        {
            _dbContext=new RentACarDbContext();
        }
        // GET: Marcas
        
        public ActionResult Index(int pagina=1)
        {

            int totalRegistros = 0;
            // Número total de registros de la tabla Customers
            totalRegistros = _dbContext.Marcas.Count();
            // Obtenemos la 'página de registros' de la tabla Customers
            var listaMarca = _dbContext.Marcas
                .OrderBy(m => m.NombreMarca)
                .Skip((pagina - 1) * _registrosPorPagina)
                .Take(_registrosPorPagina)
                .ToList();
            var listaVm = ConstruirListaVm(listaMarca);
            // Número total de páginas de la tabla Customers
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / _registrosPorPagina);
            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _listador = new Listador<MarcaListViewModel>()
            {
                RegistrosPorPagina = _registrosPorPagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                Registros = listaVm
            };

            //if (User.IsInRole("Admin"))
            //{
            //    return View(listaVm);
                
            //}

            //return View("ReadOnlyIndex", listaVm);
            return View(_listador);
        }
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(MarcaEditViewModel marcaVm)
        {
            if (!ModelState.IsValid)
            {
                return View(marcaVm);
            }

            Marca marca = ConstruirMarca(marcaVm);
            if (_dbContext.Marcas.Any(m => m.NombreMarca == marca.NombreMarca))
            {
                ModelState.AddModelError(string.Empty,"Registro repetido!!!");
                return View(marcaVm);
            }

            using (var transaction=_dbContext.Database.BeginTransaction())
            {
                try
                {
                    var folder = "~/Content/Images/Marcas/";
                    string file = "";
                    _dbContext.Marcas.Add(marca);
                    _dbContext.SaveChanges();
                    if (marcaVm.LogoFile!=null)
                    {
                        file = $"{marca.MarcaId}.jpg";
                        var response = Helper.UploadPhoto(marcaVm.LogoFile, folder, file);
                        if (!response)
                        {
                            file = "SinImagenDisponible.jpg";
                        }
                    }
                    else
                    {
                        file = "SinImagenDisponible.jpg";
                    }
                    marca.Logo = $"{folder}{file}";
                    _dbContext.Entry(marca).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    transaction.Commit();
                    TempData["Msg"] = "Registro agregado";

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(marcaVm);

                }

            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Marca marca = _dbContext.Marcas.SingleOrDefault(m => m.MarcaId == id);
            if (marca==null)
            {
                return HttpNotFound("Código de marca no encontrado");
            }

            MarcaEditViewModel marcaVm = ConstruirMarcaEdit(marca);
            return View(marcaVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(MarcaEditViewModel marcaVm)
        {
            if (!ModelState.IsValid)
            {
                return View(marcaVm);
            }

            Marca marca = ConstruirMarca(marcaVm);
            if (_dbContext.Marcas.Any(m=>m.NombreMarca==marcaVm.NombreMarca && 
                                         m.MarcaId!=marcaVm.MarcaId))
            {
                ModelState.AddModelError(string.Empty,"Registro repetido!!!");
                return View(marcaVm);
            }

            try
            {
                var folder = "~/Content/Images/Marcas/";
                if (marcaVm.LogoFile!=null)
                {
                    var file = $"{marca.MarcaId}.jpg";
                    var response = Helper.UploadPhoto(marcaVm.LogoFile, folder, file);
                    if (!response)
                    {
                        file = "SinImagenDisponible.jpg";
                    }
                    marca.Logo = $"{folder}{file}";

                }
                _dbContext.Entry(marca).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(marcaVm);

            }
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Marca marca = _dbContext.Marcas.SingleOrDefault(m => m.MarcaId == id);
            if (marca == null)
            {
                return HttpNotFound("Código de marca no encontrado");
            }

            MarcaListViewModel marcaVm = ConstruirMarcaList(marca);
            
            return View(marcaVm);

        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            Marca marca = _dbContext.Marcas.SingleOrDefault(m => m.MarcaId == id);
            try
            {
                _dbContext.Marcas.Remove(marca);
                _dbContext.SaveChanges();
                if (!marca.Logo.Contains("SinImagenDisponible"))
                {
                    var response = Helper.DeletePhoto(marca.Logo);
                    
                }
                TempData["Msg"] = "Registro borrada";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                MarcaListViewModel marcaVm = ConstruirMarcaList(marca);
                ModelState.AddModelError(string.Empty,e.Message);
                return View(marcaVm);
            }
        }

        private MarcaListViewModel ConstruirMarcaList(Marca marca)
        {
            return new MarcaListViewModel
            {
                MarcaId = marca.MarcaId,
                NombreMarca = marca.NombreMarca,
                Logo = marca.Logo
            };
        }

        private MarcaEditViewModel ConstruirMarcaEdit(Marca marca)
        {
            return new MarcaEditViewModel
            {
                MarcaId = marca.MarcaId,
                NombreMarca = marca.NombreMarca,
                Logo = marca.Logo
            };
        }

        private Marca ConstruirMarca(MarcaEditViewModel marcaVm)
        {
            return new Marca
            {
                MarcaId = marcaVm.MarcaId,
                NombreMarca = marcaVm.NombreMarca,
                Logo = marcaVm.Logo
            };
        }

        private List<MarcaListViewModel> ConstruirListaVm(List<Marca> listaMarca)
        {
            var lista = new List<MarcaListViewModel>();
            foreach (var marca in listaMarca)
            {
                var vm = ConstruirMarcaList(marca);
                lista.Add(vm);
            }

            return lista;
        }
    }
}