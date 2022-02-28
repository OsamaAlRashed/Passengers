using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Passengers.SharedKernel.Enums;
using System.Linq;

namespace Passengers.SharedKernel.Files
{
    public static class FileExtensions
    {
        #region FormFile
        public static string TryUploadImage(this IFormFile image, string uploadsFolderName, string webRootPath)
        {
            string path = null;
            try
            {
                if (image != null)
                {
                    var documentsDirectory = Path.Combine("wwwroot", "Documents", uploadsFolderName);
                    if (!Directory.Exists(documentsDirectory))
                    {
                        Directory.CreateDirectory(documentsDirectory);
                    }
                    path = Path.Combine("Documents", uploadsFolderName, Guid.NewGuid().ToString() + "_" + image.FileName);
                    string filePath = Path.Combine(webRootPath, path);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }
                }
                return path;
            }
            catch (Exception e) when (e is IOException || e is Exception)
            {
                return "";
            }
        }
        public static List<string> TryUploadImages(this List<IFormFile> images, string uploadsFolderName, string webRootPath)
        {
            return images.Select(image => TryUploadImage(image, uploadsFolderName, webRootPath)).ToList();
        }

        public static bool TryDeleteImage(this string path , string webRootPath)
        {
            if (!path.IsNullOrEmpty())
            {
                string filePath = Path.Combine(webRootPath, path);
                try { File.Delete(filePath); } catch (Exception e) { return false; }
            }
            return true;
        }

        public static async Task<string> TryUploadImageAsync(this IFormFile image , string uploadsFolderName, string webRootPath)
        {
            string path = null;
            try
            {
                if (image != null)
                {
                    var documentsDirectory = Path.Combine("wwwroot", "Documents", uploadsFolderName);

                    if (!Directory.Exists(documentsDirectory))
                        Directory.CreateDirectory(documentsDirectory);

                    path = Path.Combine("Documents", uploadsFolderName , Guid.NewGuid().ToString() + "_" + image.FileName);
                    string filePath = Path.Combine(webRootPath, path);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }
                return path;
            }
            catch (Exception e) when (e is IOException || e is Exception)
            {
                return "";
            }
        }

        public static DocumentTypes GetDocumentType(this string fileName)
        {
            var extention = Path.GetExtension(fileName).Substring(1);
            var imageExtentions = new List<string>() { "JPEG", "JPG", "PNG", "GIF", "TIFF" };
            var videoExtentions = new List<string>() { "MP4", "MOV", "WMV", "AVI", "AVCHD", "FLV", "F4V", "SWF", "MKV" };
            if (imageExtentions.Contains(extention)) return DocumentTypes.Image;
            else if (videoExtentions.Contains(extention)) return DocumentTypes.Video;
            return DocumentTypes.File;
        }
        #endregion

        #region Base64 

        public static string ProcessUploadedFile(this string base64Image, string uploadsFolderName, string webRootPath)
        {

            string photoPath = null;

            (string base64, string extension) file = FormatFile(base64Image);

            var bytes = !String.IsNullOrEmpty(file.base64) ? Convert.FromBase64String(file.base64) : new byte[0];

            byte[] decodedBytes = Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(base64Image)));

            string uniqueFileName = "";

            if (base64Image != null)
            {
                var uploadsFolder = Path.Combine(webRootPath, uploadsFolderName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "." + file.extension;
                photoPath = Path.Combine(uploadsFolderName, uniqueFileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //uniqueFileName = filePath;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }

            return photoPath;
        }
        public static bool DeleteUploadedFile(this string path, string webRootPath)
        {
            if (path != null)
            {
                path = Path.Combine(webRootPath, path);
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
        public static (string base64, string extension) FormatFile(this string fileBase64)
        {
            if (!string.IsNullOrEmpty(fileBase64))
            {
                var base64 = fileBase64.Substring(fileBase64.IndexOf(",") + 1);
                var extentionLength = fileBase64.IndexOf(";") - fileBase64.IndexOf("/");
                var fileExtention = fileBase64.Substring(fileBase64.IndexOf("/") + 1, extentionLength - 1);
                return (base64, fileExtention);
            }
            return (string.Empty, string.Empty);
        }

        #endregion
    }
}
