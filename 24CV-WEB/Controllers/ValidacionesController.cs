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
		public bool IsCurpValid(string email)
		{
            string valCurp = @"^[A-Z]{4}[0-9]{6}[HM][A-Z]{2}[A-Z0-9]{3}[0-9]$";
			return Regex.IsMatch(email, valCurp);
		}
		[HttpPost]
        public IActionResult EnviarInformacion(Curriculum model) {

			string mensaje = "";
            //model.RutaFoto = "FakePath";
            if(model.Nombre == null || model.Apellidos == null || model.PorcentajeIngles == null || model.RutaFoto == null || model.RutaFoto == null || model.CURP == null || model.Dirección == null || model.FechaNacimiento == null || model.Foto == null)
            {
				if (!IsCurpValid(model.CURP))
                {
				    mensaje = "Curp con formato incorrecto";
				    TempData["msj"] = mensaje;

				    return View("Index", model);
			    }
                mensaje = "Datos incompletos, por favor ingresa los datos faltantes";
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


			//if (!ModelState.IsValid)
			//{
			//	return View("Editar", model);
			//}

			var updateResult = await _curriculumService.Update(model);

			if (updateResult.Success)
			{
				return RedirectToAction("Lista");
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
