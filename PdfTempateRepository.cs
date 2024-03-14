using System;
using System.IO;
using System.Linq;

namespace SkyPanion.Core.BLL
    {
    public class PdfTempateRepository
        {
        public void SaveImage(string imageName,string imageSource,string filePath)
            {
            try
                {
                string pattern = imageSource.Split(new string[] { ";base64," }, StringSplitOptions.None).First();
                pattern=pattern+";base64,";

                string fileString = imageSource.Replace(pattern, string.Empty);
                byte[] imageBytes = Convert.FromBase64String(fileString);

                using( var ms = new MemoryStream(imageBytes,0,imageBytes.Length) )
                    {
                    try
                        {
                        var directory = new DirectoryInfo(filePath);
                        if( directory.Exists==false )
                            {
                            directory.Create();
                            }

                        var root = directory.Root.FullName.ToString();
                        string fullPath = Path.GetFullPath(Path.Combine(filePath, imageName));

                        if( !fullPath.StartsWith(root) )
                            {
                            System.ArgumentException argEx = new System.ArgumentException("Bad Request, Invalid path");
                            throw argEx;
                            }

                        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                        fs.Write(imageBytes,0,imageBytes.Length);

                        fs.Flush();
                        fs.Dispose();
                        }
                    catch
                        {
                        throw;
                        }
                    }
                }
            catch
                {
                throw;
                }
            }

        }
}
