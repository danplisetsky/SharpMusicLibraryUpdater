using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMusicLibraryUpdater.App.Services;
using SharpMusicLibraryUpdater.App.ViewModels;

namespace SharpMusicLibraryUpdater.Tests
{
    [TestClass]
    public class SettingsSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPathInConstructor_ThrowException()
        {
            var settingsSerializer = new SettingsSerializer(null);
        }

        [TestMethod]
        public void NoSettingsFile_ReturnsSettingsInstance()
        {
            string path = Path.GetRandomFileName();
            var settingsSerializer = new SettingsSerializer(path);

            var settings = settingsSerializer.LoadSettings();

            Assert.IsInstanceOfType(settings, typeof(Settings));
        }

        [TestMethod]
        public void CorruptedSettingsFile_ReturnsSettingsInstance()
        {
            string path = Path.GetTempFileName();
            var settingsSerializer = new SettingsSerializer(path);

            var settings = settingsSerializer.LoadSettings();

            Assert.IsInstanceOfType(settings, typeof(Settings));
        }

        [TestMethod]
        public void SaveSettings_FileExists()
        {
            var settings = new Settings { MusicLibraryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
            string settingsFile = Path.GetTempFileName();
            var settingsSerializer = new SettingsSerializer(settingsFile);

            settingsSerializer.SaveSettings(settings);

            Assert.IsTrue(File.Exists(settingsFile));
        }

        [TestMethod]
        public void SaveSettingsReadSettings_ReturnValidDeserializedSettingsObject()
        {
            var settings = new Settings { MusicLibraryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
            string settingsFile = Path.GetTempFileName();
            var settingsSerializer = new SettingsSerializer(settingsFile);

            settingsSerializer.SaveSettings(settings);

            var deserializedSettings = settingsSerializer.LoadSettings();

            Assert.IsTrue(settings.MusicLibraryFolder == deserializedSettings.MusicLibraryFolder);
        }
    }
}
