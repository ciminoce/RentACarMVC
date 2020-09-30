using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
                    else
                    {
                        file = "SinImagenDisponible.jpg";
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

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Modelo modelo = _dbContext.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Tipo)
                .SingleOrDefault(m => m.ModeloId == id);
            if (modelo == null)
            {
                return HttpNotFound("Código de Modelo inexistente");
            }

            ModeloListViewModel modeloVm = ConstruirModeloListVm(modelo);
            return View(modeloVm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            Modelo modelo = _dbContext.Modelos.SingleOrDefault(m => m.ModeloId == id);

            try
            {
                _dbContext.Modelos.Remove(modelo);
                _dbContext.SaveChanges();
                if (!modelo.Foto.Contains("SinImagenDisponible"))
                {
                    var response = Helper.DeletePhoto(modelo.Foto);
                }

                TempData["Msg"] = "Registro borrado!!!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModeloListViewModel modeloVm = ConstruirModeloListVm(modelo);

                ModelState.AddModelError(string.Empty, e.Message);
                return View(modeloVm);
            }

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Modelo modelo = _dbContext.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Tipo)
                .SingleOrDefault(m => m.ModeloId == id);
            if (modelo == null)
            {
                return HttpNotFound("Código de Modelo inexistente");
            }

            ModeloEditViewModel modeloVm = ConstruirModeloEditViewModel(modelo);
            return View(modeloVm);

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ModeloEditViewModel modeloVm)
        {
            if (!ModelState.IsValid)
            {
                modeloVm.Marcas = _dbContext.Marcas.ToList();
                modeloVm.Tipos = _dbContext.Tipos.ToList();


                return View(modeloVm);
            }

            if (_dbContext.Modelos.Any(m => m.MarcaId == modeloVm.MarcaId &&
                                          m.TipoId == modeloVm.TipoId &&
                                          m.NombreModelo == modeloVm.NombreModelo &&
                                          m.ModeloId != modeloVm.ModeloId))
            {
                modeloVm.Marcas = _dbContext.Marcas.ToList();
                modeloVm.Tipos = _dbContext.Tipos.ToList();


                ModelState.AddModelError(string.Empty, "Modelo Repetido!!!");
                return View(modeloVm);
            }

            Modelo modelo = ConstruirModelo(modeloVm);
            try
            {
                if (modeloVm.FotoFile != null)
                {
                    var folder = "~/Content/Images/Modelos/";
                    var file = $"{modelo.ModeloId}.jpg";

                    var response = Helper.UploadPhoto(modeloVm.FotoFile, folder, file);
                    if (!response)
                    {
                        file = "SinImagenDisponible.jpg";
                    }
                    modelo.Foto = $"{folder}{file}";

                }

                _dbContext.Entry(modelo).State = EntityState.Modified;
                _dbContext.SaveChanges();
                TempData["Msg"] = "Registro editado!!!";
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
        private ModeloEditViewModel ConstruirModeloEditViewModel(Modelo modelo)
        {
            return new ModeloEditViewModel
            {
                ModeloId = modelo.ModeloId,
                MarcaId = modelo.MarcaId,
                TipoId = modelo.TipoId,
                NombreModelo = modelo.NombreModelo,
                Foto = modelo.Foto,
                Marcas = _dbContext.Marcas.ToList(),
                Tipos = _dbContext.Tipos.ToList()

            };
        }

        private List<ModeloListViewModel> ConstruirListaVm(List<Modelo> listaModelos)
        {
            var listaVm = new List<ModeloListViewModel>();
            foreach (var modelo in listaModelos)
            {
                var vm = ConstruirModeloListVm(modelo);
                listaVm.Add(vm);
            }

            return listaVm;
        }

        private ModeloListViewModel ConstruirModeloListVm(Modelo modelo)
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