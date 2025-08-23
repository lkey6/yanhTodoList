using System.ComponentModel.DataAnnotations;

namespace AzurePJ.Models
{
    public class PhotoUploadViewModel
    {
        public int AlbumId { get; set; }

        [Display(Name = "タイトル")]
        public string? Title { get; set; }

        [Display(Name = "説明")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "写真を選択してください♡")]
        [Display(Name = "写真")]
        public IFormFile PhotoFile { get; set; } = null!;
    }
}
