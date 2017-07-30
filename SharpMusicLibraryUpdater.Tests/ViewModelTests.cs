using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMusicLibraryUpdater.App.Services;
using SharpMusicLibraryUpdater.App.Models;
using iTunesSearch.Library;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SharpMusicLibraryUpdater.App.ViewModels;
using System.IO;
using System.Text;

namespace SharpMusicLibraryUpdater.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        //[TestMethod]
        //public void NoSettings_EmptyArtistList()
        //{
        //    var vm = new ArtistViewModel(new MusicLibraryReader(), new iTunesSearchManager(),
        //        new SettingsSerializer(Path.GetRandomFileName())
        //        );
        //    Assert.IsTrue(!vm.Artists.Any());
        //}

        //[TestMethod]
        //public void ReadCorruptedSettingsFile_EmptyArtistsList()
        //{
        //    var vm = new ArtistViewModel(new MusicLibraryReader(), new iTunesSearchManager(),
        //       new SettingsSerializer(Path.GetTempFileName())
        //       );
        //    Assert.IsTrue(!vm.Artists.Any());
        //}

    }
}
