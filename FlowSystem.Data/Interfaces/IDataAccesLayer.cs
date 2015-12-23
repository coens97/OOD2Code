using FlowSystem.Common;
using FlowSystem.Data.Files;

namespace FlowSystem.Data.Interfaces
{
    public interface IDataAccesLayer
    {
        bool FileAlreadyExist(string path);
        FlowNetworkEntity OpenFile(string path);
        void SaveFile(FlowFile flowNetwork, string path);
    }
}