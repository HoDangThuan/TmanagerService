using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TmanagerService.Api.Models
{
    public static class UploadFileToCloudinary
    {
        private static readonly Account account = new Account
        {
            Cloud = Startup.Configuration["cloudinary_cloud_name"],
            ApiKey = Startup.Configuration["cloudinary_api_key"],
            ApiSecret = Startup.Configuration["cloudinary_api_secret"]
        };
        private static readonly Cloudinary cloudinary = new Cloudinary(account);

        public static string UploadImage(IFormFile file)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            var imgURI = uploadResult.Uri;
            return imgURI.ToString();
        }

        public static List<string> UploadListImage(List<IFormFile> files)
        {
            List<string> listURI = new List<string>();
            foreach (var file in files)
            {
                listURI.Add(UploadImage(file));
            }
            return listURI;
        }
    }
}
