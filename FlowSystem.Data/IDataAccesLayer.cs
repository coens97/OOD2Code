using FlowSystem.Common;

namespace FlowSystem.Data
{
    public interface IDataAccesLayer
    {
        bool FileAlreadyExist(string path);
        FlowNetworkEntity OpenFile(string path);
        void SaveFile(FlowNetworkEntity flowNetwork, string path);
    }
}