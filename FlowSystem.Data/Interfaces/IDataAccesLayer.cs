using FlowSystem.Common;
using FlowSystem.Data.Files;

namespace FlowSystem.Data.Interfaces
{
    public interface IDataAccesLayer
    {
        FlowNetworkEntity OpenFile(string path);
        void SaveFile(FlowNetworkEntity flowNetwork, string path);
    }
}