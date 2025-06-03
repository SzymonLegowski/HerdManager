using HerdRest.Data;
using HerdRest.DataImport;
using HerdRest.Interfaces;
using HerdRest.PublicClasses;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController(ILochaRepository lochaRepository, IMiotRepository miotRepository, IWydarzenieRepository wydarzenieRepository, DataContext context) : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            string filePath = UploadHandler.Upload(file);
            if (filePath == "0")
            {
                return BadRequest("Niepoprawny format pliku, upewnij się że przesyłasz plik .csv lub .txt");
            }
            CsvLoader csvLoader = new(lochaRepository, miotRepository, wydarzenieRepository, context);
            
            return Ok(csvLoader.LoadDataFromCsv(filePath));
        }
    }   
}