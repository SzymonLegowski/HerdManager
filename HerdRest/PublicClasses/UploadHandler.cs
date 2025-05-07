using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerdRest.PublicClasses
{
    public class UploadHandler
    {
        public string Upload(IFormFile file)
        {
            List<string> ValidExtensions = [".txt", ".csv"];
            string Extension = Path.GetExtension(file.FileName);
            if (!ValidExtensions.Contains(Extension))
            {
                return $"Niepoprawny format pliku({string.Join(", ", ValidExtensions)})";
            }
            string fileName = file.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            using FileStream stream = new(Path.Combine(path, fileName), FileMode.Create);
            file.CopyTo(stream);

            return $"Plik {file.FileName} został przesłany";
        }
    }
}