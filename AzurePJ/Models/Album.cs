using System.ComponentModel.DataAnnotations;

namespace AzurePJ.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Photo>? Photos { get; set; }
    }
}
