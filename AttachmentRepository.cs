using System;
using System.IO;
using System.Linq;


namespace SkyPanion.Core.BLL
    {
    public class AttachmentRepository
        {

        public void SaveImage(string imageName,string imageSource,string filePath)
            {
            try
                {
                string pattern = imageSource.Split(new string[] { ";base64," }, StringSplitOptions.None).First();
                pattern=pattern+";base64,";

                string fileString = System.Text.RegularExpressions.Regex.Replace(imageSource, pattern, string.Empty);
                byte[] imageBytes = Convert.FromBase64String(fileString);

                using( var ms = new MemoryStream(imageBytes,0,imageBytes.Length) )
                    {
                    try
                        {
                        // check image folder path
                        var directory = new DirectoryInfo(filePath);
                        if( directory.Exists==false )
                            {
                            // if image folder not exist then create it
                            directory.Create();
                            }
                        // write image in folder
                        FileStream fs = new FileStream(Path.Combine(filePath, imageName), FileMode.OpenOrCreate);
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
