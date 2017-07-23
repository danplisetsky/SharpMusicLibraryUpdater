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
        public bool IgnoreInFutureChecks { get; set; }
        public LocalDate ReleaseDate { get; }
        public NewAlbum(string name, LocalDate releaseDate, bool ignore = false) : base(name)
        {
            this.IgnoreInFutureChecks = ignore;
            this.ReleaseDate = releaseDate;
        }
    }
}
