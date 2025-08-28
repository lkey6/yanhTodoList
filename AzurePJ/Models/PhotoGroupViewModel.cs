namespace AzurePJ.Models
{
    public class PhotoGroupViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public List<PhotoViewModel> Photos { get; set; } = new();
    }

    public class PhotoViewModel
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }



}
