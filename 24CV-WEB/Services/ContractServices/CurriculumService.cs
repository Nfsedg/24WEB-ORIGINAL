﻿using _24CV_WEB.Models;
using _24CV_WEB.Repository;
using _24CV_WEB.Repository.CurriculumDAO;
using _24CV_WEB.Services.Contracts;

namespace _24CV_WEB.Services.ContractServices
{
	public class CurriculumService : ICurriculumService
	{
		private CurriculumRepository _repository;

        public CurriculumService(ApplicationDbContext context)
        {
            _repository = new CurriculumRepository(context);
        }

        public async Task<ResponseHelper> Create(Curriculum model)
		{
			ResponseHelper responseHelper = new ResponseHelper();

			try
			{
				string filePath = "";
				string fileName = "";

                if (model.Foto != null && model.Foto.Length > 0)
				{
					fileName = Path.GetFileName(model.Foto.FileName);
					filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Fotos", fileName);
				}
				model.RutaFoto = fileName;

				//Copia el archivo en un directorio
				using( var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await model.Foto.CopyToAsync(fileStream);
				}
				if (_repository.Create(model) > 0)
				{
					responseHelper.Success = true;
					responseHelper.Message = $"Se agregó el curriculum de {model.Nombre} con éxito.";
				}
				else
				{
					responseHelper.Message = "Ocurrió un error al agregar el dato.";
				}
			}
			catch (Exception e)
			{
				responseHelper.Message = $"Ocurrió un error al agregar el dato. Error: {e.Message}";
			}


			return responseHelper;	
		}

		public ResponseHelper Delete(int id)
		{
			throw new NotImplementedException();
		}

		public List<Curriculum> GetAll()
		{
			try
			{
				List<Curriculum> list = new List<Curriculum>();
				list = _repository.GetAll();
				return list;

			} catch(Exception e)
			{
				throw;
			}
		}

		public Curriculum GetById(int id)
		{
			throw new NotImplementedException();
		}

		public ResponseHelper Update(Curriculum model)
		{
			throw new NotImplementedException();
		}
	}
}
