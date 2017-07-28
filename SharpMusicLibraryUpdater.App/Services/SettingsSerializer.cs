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
    public class SettingsSerializer
    {
        private DataContractSerializer serializer = new DataContractSerializer(typeof(Settings));
        private readonly string settingsFileFullPath;

        public SettingsSerializer(String settingsFileFullPath)
        {
            this.settingsFileFullPath = settingsFileFullPath ?? throw new ArgumentNullException(nameof(settingsFileFullPath));
        }

        public void SaveSettings(Settings settings)
        {
            var stream = GetStream(FileMode.Create);
            using (var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(binaryWriter, settings);
            }
        }

        public Settings LoadSettings()
        {
            var stream = GetStream(FileMode.Open);
            if (stream == null)
                return new Settings();
            using (var binaryReader = XmlDictionaryReader.CreateBinaryReader(stream, new XmlDictionaryReaderQuotas()))
            {
                try
                {
                    return (Settings)serializer.ReadObject(binaryReader);
                }
                catch (SerializationException)
                {
                    return new Settings();
                }
            }
        }

        private FileStream GetStream(FileMode fileMode)
        {
            try
            {
                return new FileStream(this.settingsFileFullPath, fileMode);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

    }
}
