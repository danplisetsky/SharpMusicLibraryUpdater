using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharpMusicLibraryUpdater.App.Services
{
    public class MusicLibraryReader
    {
        public string[] ReadMusicLibrary(string musicLibraryPath) => Directory.GetDirectories(musicLibraryPath);
    }
}
