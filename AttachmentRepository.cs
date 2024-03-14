using SkyPanion.Core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TextLogger;

namespace SkyPanion.Core.BLL
{
    public class AttachmentRepository : BaseRepository
    {
        /// <summary>
        ///  It calls to get list of Attachments as per search parameters
        /// </summary>
        /// <param name="companyID">Search parameter selected Property Id from Search Filter</param>
        /// <param name="status">Attachment Status like(Static,Rejected etc.)</param>
        /// <param name="accountID"></param>
        /// <param name="pageNumber">Pagination Parameter for list pagination </param>
        /// <param name="rowsPerPage">Pagination Parameter for list pagination </param>
        /// <param name="UserID"></param>
        /// <param name="IsSuperUser"></param>
        /// <returns></returns>
        public List<spGetAttachmentsForList_Result> GetAttachments(long? companyID, string status, long accountID, int pageNumber, int rowsPerPage, long UserID, bool IsSuperUser)
        {
            try
            {
                if (companyID == 0)
                {
                    companyID = null;
                }
                List<spGetAttachmentsForList_Result> attachements = DBContext.spGetAttachmentsForList(companyID, status, accountID, pageNumber, rowsPerPage, UserID).ToList();
                return attachements;
            }

            catch
            {
                throw;
            }
        }

        /// <summary>
        /// It calls to remove selected attachment by Attachment Id
        /// </summary>
        /// <param name="attachemntID">Selected Attachment's Id</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public long? DeleteAttachment(long? attachemntID, long userID)
        {
            long? attachID = null;
            try
            {
                attachID = DBContext.spDeleteAttachment(attachemntID, userID).Value;


                if (attachID == -100)
                {
                    Logger.WriteMessage("Failed to delete attachment for attachment id " + attachemntID);
                    throw new System.InvalidOperationException("Attachment status has been alreay changed.");
                }
                return attachID;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// It call to Reassign Rejected Pdf (it means user can move Selected pdf in rejected state to other property folder and updates it status to static)
        /// </summary>
        /// <param name="attachmentID">Selected Pdf(Attachment's) Id</param>
        /// <param name="companyID">Selected Pdf Current Company Id</param>
        /// <param name="newCompanyNumber">Company Number where Pdf will move</param>
        /// <param name="oldCompanyNumber">Current Company Number where pdf exists right now</param>
        /// <param name="userID">LoggedIn user Id</param>
        /// <param name="attachmentName">Selected Pdf(Attachment's) name</param>
        /// <returns></returns>
        public string GetChangeAttachmentProperty(long? attachmentID, long? companyID, string newCompanyNumber, string oldCompanyNumber, long userID, string attachmentName)
        {
            try
            {
                var settings = SettingConfiguration.GetSkypanionSettings();
                string sourcePath = Path.Combine(settings.PdfStoreLocation, oldCompanyNumber, attachmentName);
                string destinationDirectory = Path.Combine(settings.PdfStoreLocation, newCompanyNumber);
                string destinationPath = Path.Combine(settings.PdfStoreLocation, newCompanyNumber, attachmentName);
                string btrievePath = "";

                //It checks pdf exists on current location
                if (File.Exists(sourcePath))
                {
                    //it checks if destinationDirectory exists before move
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }
                    if (!File.Exists(destinationPath))
                    {
                        File.Move(sourcePath, destinationPath);
                    }
                    else {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
                        attachmentName = fileNameWithoutExt + Guid.NewGuid().ToString().Substring(0, 4) + ".pdf";
                        destinationPath = Path.Combine(settings.PdfStoreLocation, newCompanyNumber, attachmentName);
                        File.Move(sourcePath, destinationPath);
                    }
                    btrievePath = Path.Combine(settings.PdfStoreLocation, newCompanyNumber);

                }

                DBContext.spResetAttachmentProperty(attachmentID, companyID, userID, attachmentName);
                return "success";

            }
            catch
            {
                throw;
            }
        }


        public string getAttachmentDetailsByAttachmentID(long? attachmentID)
        {
            string attachmentName = "";
            try
            {
                var attachment = DBContext.spGetAttachmentByAttachmentID(attachmentID).FirstOrDefault();
                if (attachment != null) {
                    attachmentName = attachment.AttachmentName;
                }

                return attachmentName; 
             }

            catch (Exception ex)
            {
                return attachmentName;
            }
        }


        public List<spGetManualAttachments_Result> GetManualAttachments(long userID)
        {
            try
            {
                var attachments = DBContext.spGetManualAttachments(0).ToList();
                return attachments;
            }
            catch
            {
                throw;
            }
        }


    }
}

    

