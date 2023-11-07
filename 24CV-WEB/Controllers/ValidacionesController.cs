using System.Text.RegularExpressions;
using _24CV_WEB.Models;
using _24CV_WEB.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace _24CV_WEB.Controllers
{
    public class ValidacionesController : Controller
    {
        private readonly ICurriculumService _curriculumService;

        public ValidacionesController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        public IActionResult Index()
        {
            return View();
        }
		public bool IsCurpValid(string curp)
		{
			string pattern = @"^([A-Z][AEIOUX][A-Z]{2}\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])[HM](?:AS|B[CS]|C[CLMSH]|D[FG]|G[TR]|HG|JC|M[CNS]|N[ETL]|OC|PL|Q[TR]|S[PLR]|T[CSL]|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d])(\d)$";
			return Regex.IsMatch(curp, pattern);
		}

		[HttpPost]
        public IActionResult EnviarInformacion(Curriculum model) {

			string mensaje = "";
            //model.RutaFoto = "FakePath";
            if(model.Nombre == null || model.Apellidos == null || model.PorcentajeIngles == null || model.RutaFoto == null || model.RutaFoto == null || model.CURP == null || model.Dirección == null || model.FechaNacimiento == null || model.Foto == null)
            {
                mensaje = "Datos incompletos, por favor ingresa los datos faltantes";
                TempData["msj"] = mensaje;

                return View("Index", model);
            }
			if (!IsCurpValid(model.CURP))
			{
				mensaje = "Curp con formato incorrecto";
				TempData["msj"] = mensaje;

				return View("Index", model);
			}
			string filePath = model.Foto.FileName;
			string fileExtension = Path.GetExtension(filePath).ToLower();
			bool isImage = false;
			string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".png" };

			foreach (string extension in imageExtensions)
			{
				if (fileExtension == extension)
				{
					isImage = true;
					break;
				}
			}

			if (!isImage)
			{
				mensaje = "Por favor, selecciona una imagen";
				TempData["msj"] = mensaje;

				return View("Index", model);
			}
			if (ModelState.IsValid)
            {
                var response = _curriculumService.Create(model).Result;

                mensaje = response.Message;
                TempData["msj"] = mensaje;
                return RedirectToAction("Index");
            }
            else
            {
                mensaje = "Datos incorrectos";
                TempData["msj"] = mensaje;

                return View("Index",model);
            }

        }
        public IActionResult Lista()
        {
            return View(_curriculumService.GetAll());
        }
        public IActionResult Curriculum(int id)
        {
			var curriculum = _curriculumService.GetById(id);
			if (curriculum == null)
			{
				return NotFound();
			}
			return View("Curriculum", curriculum);
        }

		[HttpGet]
		public IActionResult IrEditar(int id)
		{
			var curriculum = _curriculumService.GetById(id);
			return View("Editar", curriculum);
		}

		[HttpPost]
		public async Task<IActionResult> Editar(Curriculum model)
		{
            string mensaje;
			if(model.Foto == null)
			{
                TempData["ErrorMessage"] = "Por favor, selecciona una imagen para continuar";
                return View("Editar", model);
            }
            if (model.Nombre == null || model.Apellidos == null || model.PorcentajeIngles == null || model.RutaFoto == null || model.CURP == null || model.Dirección == null || model.FechaNacimiento == null)
            {
                mensaje = "Datos incompletos, por favor ingresa los datos faltantes";
                TempData["ErrorMessage"] = mensaje;

                return View("Editar", model);
            }
            if (!IsCurpValid(model.CURP))
            {
                mensaje = "Curp con formato incorrecto";
                TempData["ErrorMessage"] = mensaje;

                return View("Editar", model);
            }
            string filePath = model.Foto.FileName;
            string fileExtension = Path.GetExtension(filePath).ToLower();
            bool isImage = false;
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".png" };

            foreach (string extension in imageExtensions)
            {
                if (fileExtension == extension)
                {
                    isImage = true;
                    break;
                }
            }

            if (!isImage)
            {
                mensaje = "Por favor, selecciona una imagen";
                TempData["ErrorMessage"] = mensaje;

                return View("Index", model);
            }
            if (!ModelState.IsValid)
			{
                TempData["ErrorMessage"] = "El modelo es invalido";
                return View("Editar", model);
			}

			var updateResult = await _curriculumService.Update(model);

			if (updateResult.Success)
			{
				return View("Lista", _curriculumService.GetAll());
			}
			else
			{
				TempData["ErrorMessage"] = "No se pudo actualizar el currículum. Por favor, inténtalo de nuevo.";
				return View("Editar", model);
			}
		}

		[HttpGet]
		public IActionResult Eliminar(int id)
		{
			_curriculumService.Delete(id);
			return RedirectToAction("Lista");
		}
	}
}
