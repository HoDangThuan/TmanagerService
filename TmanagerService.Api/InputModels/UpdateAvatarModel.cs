using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class UpdateAvatarModel
    {
        [Required]
        public IFormFile FileAvatar { get; set; }
    }
}
