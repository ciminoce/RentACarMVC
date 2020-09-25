using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentACarMVC.Classes;
using RentACarMVC.Context;
using RentACarMVC.Models;
using RentACarMVC.ViewModels.Modelo;

namespace RentACarMVC.Controllers
{
    public class ModelosController : Controller
    {
        private readonly RentACarDbContext _dbContext;

        public ModelosController()
        {
            _dbContext = new RentACarDbContext();
        }
        // GET: Modelos
        public ActionResult Index()
        {
            var listaModelos = _dbContext.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Tipo)
                .ToList();
            var listaVm = ConstruirListaVm(listaModelos);
            return View(listaVm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ModeloEditViewModel vm = new ModeloEditViewModel
            {
                Marcas = _dbContext.Marcas.ToList(),
                Tipos = _dbContext.Tipos.ToList()
            };
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ModeloEditViewModel modeloVm)
        {
            if (!ModelState.IsValid)
            {
                modeloVm.Marcas = _dbContext.Marcas.ToList();
                modeloVm.Tipos = _dbContext.Tipos.ToList();
                return View(modeloVm);
            }

            Modelo modelo = ConstruirModelo(modeloVm);
            if (_dbContext.Modelos.Any(m => m.MarcaId == modelo.MarcaId &&
                                          m.TipoId == modelo.TipoId &&
                                          m.NombreModelo == modelo.NombreModelo))
            {
                modeloVm.Marcas = _dbContext.Marcas.ToList();
                modeloVm.Tipos = _dbContext.Tipos.ToList();
                ModelState.AddModelError(string.Empty, "Modelo repetido!!!");
                return View(modeloVm);
            }

            using (var tran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var folder = "~/Content/Images/Modelos/";
                    var file = "";
                    _dbContext.Modelos.Add(modelo);
                    _dbContext.SaveChanges();
                    if (modeloVm.FotoFile != null)
                    {
                        file = $"{modelo.ModeloId}.jpg";
                        var response = Helper.UploadPhoto(modeloVm.FotoFile, folder, file);
                        if (!response)
                        {
                            file = "SinImagenDisponible.jpg";
                        }
                    }

                    modelo.Foto = $"{folder}{file}";
                    _dbContext.Entry(modelo).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    tran.Commit();
                    TempData["Msg"] = "Registro agregado";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    modeloVm.Marcas = _dbContext.Marcas.ToList();
                    modeloVm.Tipos = _dbContext.Tipos.ToList();

                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(modeloVm);
                }
            }
        }

        private Modelo ConstruirModelo(ModeloEditViewModel modeloVm)
        {
            return new Modelo
            {
                ModeloId = modeloVm.ModeloId,
                MarcaId = modeloVm.MarcaId,
                TipoId = modeloVm.TipoId,
                NombreModelo = modeloVm.NombreModelo,
                Foto = modeloVm.Foto
            };
        }

        private List<ModeloListViewModel> ConstruirListaVm(List<Modelo> listaModelos)
        {
            var listaVm = new List<ModeloListViewModel>();
            foreach (var modelo in listaModelos)
            {
                var vm = ConstruirListaListVm(modelo);
                listaVm.Add(vm);
            }

            return listaVm;
        }

        private ModeloListViewModel ConstruirListaListVm(Modelo modelo)
        {
            return new ModeloListViewModel
            {
                ModeloId = modelo.ModeloId,
                Marca = modelo.Marca.NombreMarca,
                Tipo = modelo.Tipo.Descripcion,
                NombreModelo = modelo.NombreModelo,
                Foto = modelo.Foto
            };
        }
    }
}