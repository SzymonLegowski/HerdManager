namespace HerdRest.PublicClasses
{
    public static class UploadHandler
    {
        public static string Upload(IFormFile file)
        {
            List<string> ValidExtensions = [".txt", ".csv"];
            string Extension = Path.GetExtension(file.FileName);
            if (!ValidExtensions.Contains(Extension))
            {
                return "0";
            }
            string fileName = file.FileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            using FileStream stream = new(Path.Combine(path, fileName), FileMode.Create);
            file.CopyTo(stream);

            return path + "/" + fileName;
        }
    }
}