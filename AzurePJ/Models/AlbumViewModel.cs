namespace AzurePJ.Models
{
    public class AlbumViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CoverUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }



}
