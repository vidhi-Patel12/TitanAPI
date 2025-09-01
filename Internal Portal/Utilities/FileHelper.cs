namespace Internal_Portal.Utilities
{
    public static class FileHelper
    {
        public static string? SaveFile(IFormFile? file, string folderName, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(env.WebRootPath, folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/{folderName}/{uniqueFileName}"; // relative URL for DB
        }
    }
}
