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
        public void SaveSettings_FileExists()
        {
            var settings = new Settings { MusicLibraryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
            string settingsFile = Path.GetRandomFileName();
            var settingsSerializer = new SettingsSerializer(settingsFile);

            settingsSerializer.SaveSettings(settings);

            Assert.IsTrue(File.Exists(settingsFile));
        }

        [TestMethod]
        public void SaveSettingsReadSettings_ReturnValidDeserializedObject()
        {
            var settings = new Settings { MusicLibraryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
            string settingsFile = Path.GetRandomFileName();
            var settingsSerializer = new SettingsSerializer(settingsFile);

            settingsSerializer.SaveSettings(settings);

            var deserializedSettings = settingsSerializer.LoadSettings();

            Assert.IsTrue(settings.MusicLibraryFolder == deserializedSettings.MusicLibraryFolder);
        }
    }
}
