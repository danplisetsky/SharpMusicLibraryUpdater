using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMusicLibraryUpdater.App.Services;

namespace SharpMusicLibraryUpdater.Tests
{
    [TestClass]
    public class MusicLibraryReaderTests
    {
        [TestMethod]
        public void ReadMusciLibraryFolder_ReturnAllFolders()
        {
            var mlr = new MusicLibraryReader();
            var artists = new[] { "Nicola Benedetti", "R.E.M", "The Posies" };
            string musicLibraryFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "TestMusicLibraryFolder")).FullName;
            foreach (string item in artists)
            {
                Directory.CreateDirectory(Path.Combine(musicLibraryFolder, item));
            }

            var artistsFolderNames = mlr.ReadMusicLibrary(musicLibraryFolder);

            Assert.IsTrue(artists.SequenceEqual(artistsFolderNames.Select(path => new DirectoryInfo(path).Name)));
        }
    }
}
