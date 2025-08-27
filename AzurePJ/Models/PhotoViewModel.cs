namespace AzurePJ.Models
{
    public class PhotoViewModel
    {
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class PhotoGroupViewModel
    {
        public DateTime Date { get; set; }
        public List<PhotoViewModel> Photos { get; set; } = new();
    }

}
