using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpMusicLibraryUpdater.App.Services
{
    public static class SettingsSerializer
    {
        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Settings));
        private static readonly string settingsFilename = "settings.bin";
        private static readonly string settingsFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFilename);

        public static void SaveSettings(Settings settings)
        {
            using (var fileStream = new FileStream(settingsFilename, FileMode.Create))
            using (var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(fileStream))
            {
                serializer.WriteObject(binaryWriter, settings);
            }
        }

        public static Settings LoadSettings()
        {
            if (File.Exists(settingsFullPath))
            {
                using (var fileStream = new FileStream(settingsFilename, FileMode.Open))
                using (var binaryReader = XmlDictionaryReader.CreateBinaryReader(fileStream, new XmlDictionaryReaderQuotas()))
                {
                    return (Settings)serializer.ReadObject(binaryReader);
                }
            }
            return new Settings();
        }
    }
}
