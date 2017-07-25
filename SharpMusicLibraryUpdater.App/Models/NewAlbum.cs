using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace SharpMusicLibraryUpdater.App.Models
{
    public class NewAlbum : Album
    {
        public LocalDate ReleaseDate { get; }
        public bool MarkAsSeen { get; set; }

        public NewAlbum(string name, LocalDate releaseDate, bool markAsSeen = false) : base(name)
        {
            this.MarkAsSeen = markAsSeen;
            this.ReleaseDate = releaseDate;
        }
    }
}
