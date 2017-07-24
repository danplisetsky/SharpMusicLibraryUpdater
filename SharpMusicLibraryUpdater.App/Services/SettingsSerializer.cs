using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpMusicLibraryUpdater.App.Services
{
    public static class SettingsSerializer
    {
        private static Stream fileStream;
        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Settings));
        private static readonly string settingsFilename = "settings.bin";
        private static readonly string settingsFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFilename);

        public static void Serialize(Settings settings)
        {
            if (settings == null)
            {
                throw new NotImplementedException(); // TODO: Clean this up
            }
            using (fileStream = new FileStream(settingsFilename, FileMode.Create))
            {
                serializer.WriteObject(fileStream, settings);
            }
        }

        public static Settings Deserialize()
        {
            if (File.Exists(settingsFullPath))
            {
                using (fileStream = new FileStream(settingsFilename, FileMode.Open))
                {
                    return (Settings)serializer.ReadObject(fileStream);
                }
            }
            return new Settings();
        }
    }
}
