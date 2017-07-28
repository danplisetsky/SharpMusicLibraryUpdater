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
        private readonly string[] artists = new[] { "Nicola Benedetti", "R.E.M", "The Posies" };
        private string musicLibraryFolder;

        [TestInitialize]
        public void Initialize()
        {
            musicLibraryFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "TestMusicLibraryFolder")).FullName;
            foreach (string item in artists)
            {
                Directory.CreateDirectory(Path.Combine(musicLibraryFolder, item));
            }
        }

        [TestMethod]
        public void ReadMusciLibraryFolder_ReturnAllFolders()
        {
            var mlr = new MusicLibraryReader();

            var artistsFolderNames = mlr.ReadMusicLibrary(musicLibraryFolder);

            Assert.IsTrue(artists.SequenceEqual(artistsFolderNames.Select(path => new DirectoryInfo(path).Name)));
        }

        [TestCleanup]
        public void Cleanup() => Directory.Delete(musicLibraryFolder, recursive: true);
    }
}
