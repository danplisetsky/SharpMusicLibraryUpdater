using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SharpMusicLibraryUpdater.App.Models;

namespace SharpMusicLibraryUpdater.App.Services
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public string MusicLibraryFolder { get; set; }
        [DataMember]
        public List<Artist> Artists { get; set; }
    }
}
