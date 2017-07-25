using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpMusicLibraryUpdater.App.Models
{
    [DataContract]
    public class Album
    {
        [DataMember]
        public string AlbumName { get; private set; }

        public Album (string name)
        {
            this.AlbumName = name;
        }
    }
}
