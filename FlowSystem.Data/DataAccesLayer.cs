using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FlowSystem.Common;
using FlowSystem.Data.Files;
using FlowSystem.Data.Interfaces;
using FlowSystem.Data.Utility;

namespace FlowSystem.Data
{
    public class DataAccesLayer : IDataAccesLayer
    {
        public bool FileAlreadyExist(string path)
        {
            return File.Exists(path);
        }

        public FlowNetworkEntity OpenFile(string path)
        {
            FlowFile flowFile;

            var serializer = new XmlSerializer(typeof(FlowFile));

            var reader = new StreamReader(path);
            flowFile = (FlowFile)serializer.Deserialize(reader);
            reader.Close();

            return flowFile.FromFlowFile();
        }

        public void SaveFile(FlowNetworkEntity flowNetwork, string path)
        {
            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(typeof(FlowFile));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, flowNetwork.ToFlowFile());
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(path);
                stream.Close();
            }
        }
    }
}