using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FlowSystem.Common;
using FlowSystem.Data.Files;
using FlowSystem.Data.Interfaces;

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
            throw new System.NotImplementedException();
        }

        public void SaveFile(FlowFile flowNetwork, string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(typeof(FlowFile));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, flowNetwork);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(path);
                stream.Close();
            }
        }
    }
}