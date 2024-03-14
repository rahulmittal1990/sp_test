using iText.Forms.Xfdf;
using SkyPanion.Core.DAL;
using SkyPanion.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkyPanion.Core.BLL
{
    public class PdfTempateRepository : BaseRepository
    {
        public PdfTemplateObject GetPdfTemplateDetails(long accountID)
        {
            try
            {
                string webApiUrl = ConfigurationManager.AppSettings["WebApiUrl"];
                var pdfTemplate = DBContext.spGetPdfTemplateDetails(accountID).FirstOrDefault();
                PdfTemplateObject obj = new PdfTemplateObject();
                if (pdfTemplate != null)
                {
                    obj.id = pdfTemplate.id;
                    obj.header_image = pdfTemplate.header_image;
                    obj.header_description = pdfTemplate.header_description;
                    obj.header_bk_color = pdfTemplate.header_bk_color;
                    obj.footer_image = pdfTemplate.footer_image;
                    obj.footer_description = pdfTemplate.footer_description;
                    obj.footer_color = pdfTemplate.footer_color;
                    obj.develop_date = pdfTemplate.develop_date;
                    obj.develop_by = pdfTemplate.develop_by;
                    obj.template_width = pdfTemplate.template_width;
                    obj.template_height = pdfTemplate.template_height;
                    obj.bk_color = pdfTemplate.bk_color;
                    obj.bk_image = pdfTemplate.bk_image;

                    if (pdfTemplate != null)
                    {
                        if (string.IsNullOrEmpty(obj.header_image))
                        {
                            obj.header_image = "No-Image";
                            obj.image_source = webApiUrl + "Images/Template_Image/no-image.png";
                        }
                        else
                        {
                            obj.image_source = webApiUrl + "Images/Template_Image/" + obj.header_image;
                        }
                    }
                }
                return obj;
            }

            catch
            {
                throw;
            }
        }

        public List<CompaniesPdfComponentsObject> GetCompaniesPdfComponents(long accountID)
        {
            List<CompaniesPdfComponentsObject> pdfComponents= new List<CompaniesPdfComponentsObject>();
            try
            {
                var components = DBContext.spGetCompaniesPdfComponents(accountID).ToList();

                foreach (var i in components) {
                    CompaniesPdfComponentsObject obj = new CompaniesPdfComponentsObject();
                    obj.id = i.id;
                    obj.template_id = i.template_id;
                    obj.compnent_name = i.compnent_name;
                    obj.is_component_visible = i.is_component_visible;
                    obj.component_value = i.component_value;
                    obj.develop_date = i.develop_date;
                    obj.develop_by = i.develop_by;

                    pdfComponents.Add(obj);
                }
                return pdfComponents;

            }

            catch
            {
                throw;
            }
        }


        public long SavePdfTemplateDetails(
                                            long? id , 
                                            string header_image,
                                           string header_description,
                                           string header_bk_color,
                                           string footer_image,
                                           string footer_description,
                                          string footer_color,
                                          long  template_width,
                                          long  template_height,
                                          string bk_color,
                                          string bk_image,
                                          string image_source,
                                          bool is_new_upload ,
                                          string userName ) {

            try
            {
                string filePath = ConfigurationManager.AppSettings["UploadImagePath"] + "/Template_Image";

                if (image_source != null && is_new_upload)
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string imageName = "template_header" + "_" + GenerateRandomNo().ToString() + "_" + header_image;
                    header_image = imageName;
                    SaveImage(imageName, image_source, filePath);

                }


                long  invid = DBContext.spSavePdfTemplateDetails
                                                        (
                                                        id,
                                                        header_image,
                                                        header_description,
                                                        header_bk_color,
                                                        footer_image,
                                                        footer_description,
                                                        footer_color,
                                                        template_width,
                                                        template_height,
                                                        bk_color,
                                                        bk_image,
                                                        userName
                                                        );

                return invid;
            }

            catch
            {
                throw;
            } 
        }


        public void SaveImage(string imageName, string imageSource, string filePath)
        {
            try
            {
                string pattern = imageSource.Split(new string[] { ";base64," }, StringSplitOptions.None).First();
                pattern = pattern + ";base64,";

                string fileString = imageSource.Replace(pattern, string.Empty);
                byte[] imageBytes = Convert.FromBase64String(fileString);

                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    try
                    {
                        var directory = new DirectoryInfo(filePath);
                        if (directory.Exists == false)
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
                        fs.Write(imageBytes, 0, imageBytes.Length);

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

        public string GenerateRandomNo()
        {
            Random _random = new Random();
            return _random.Next(0, 9999).ToString("D4");
        }



        public long SavePdfTemplateComponents(List<CompaniesPdfComponentsObject> obj,
                                              string userName)
        {
            try
            {
                foreach (var i in obj)
                {
                    DBContext.spSavePdfTemplateComponents
                                                        (i.id,
                                                        i.template_id , 
                                                        i.compnent_name ,
                                                        i.is_component_visible , 
                                                        i.component_value,
                                                        userName);
                }
                return 0;
            }

            catch
            {
                throw;
            }

        }
    }
}
