namespace AzurePJ.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int? AlbumId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

        public Album? Album { get; set; }
    }
}
