using System.ComponentModel.DataAnnotations;

namespace AzurePJ.Models
{
    public class AlbumPhoto
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}
