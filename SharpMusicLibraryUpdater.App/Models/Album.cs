using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMusicLibraryUpdater.App.Models
{
    public class Album
    {
        public string AlbumName { get; }

        public Album (string name)
        {
            this.AlbumName = name;
        }
    }
}
