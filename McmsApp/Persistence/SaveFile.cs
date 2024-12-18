namespace McmsApp.Persistence
{
    public class SaveFile : ISaveFile
    {
        public string SaveBinaryPdf(string filename, byte[] bytes)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = Path.Combine(folderPath, filename + ".pdf");

            if (File.Exists((filePath)))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, bytes);
            return filePath;
        }

        public string SaveBinaryImg(string filename, byte[] bytes)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = Path.Combine(folderPath, filename + ".jpg");

            if (File.Exists((filePath)))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, bytes);
            return filePath;
        }

        public string GetFilePath(string filename)
        {
            var folderPath = GetPath();
            var filePath = Path.Combine(folderPath, filename + ".pdf");
            return Path.Combine(filePath, filename);
        }

        public string GetPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public void SaveImageFromByte(byte[] imageByte, string filename)
        {

        }
    }
}
