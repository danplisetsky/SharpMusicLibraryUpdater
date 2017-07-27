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
        private readonly Stream stream;

        public SettingsSerializer(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void SaveSettings(Settings settings)
        {
            using (var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(binaryWriter, settings);
            }
        }

        public Settings LoadSettings()
        {
            using (var binaryReader = XmlDictionaryReader.CreateBinaryReader(stream, new XmlDictionaryReaderQuotas()))
            using (var xmlReader = XmlReader.Create(binaryReader, new XmlReaderSettings { CloseInput = false }))
            {
                try
                {
                    return (Settings)serializer.ReadObject(xmlReader);
                }
                catch (Exception ex) when (ex is SerializationException || ex is FileNotFoundException)
                {
                    return new Settings();
                }
            }
        }

    }
}
