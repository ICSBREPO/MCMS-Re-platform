namespace McmsApp.Persistence
{
    public interface ISaveFile
    {
        string SaveBinaryPdf(string filename, byte[] bytes);
        string SaveBinaryImg(string filename, byte[] bytes);
        string GetFilePath(string filename);
        string GetPath();
        void SaveImageFromByte(byte[] imageByte, string filename);
    }
}
