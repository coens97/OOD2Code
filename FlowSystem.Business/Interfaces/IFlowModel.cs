using System.Collections.Generic;
using FlowSystem.Common;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business.Interfaces
{
    public interface IFlowModel
    {
        FlowNetworkEntity FlowNetwork { get; set; }
        void AddMerger(PointEntity point);
        void AddPipe(IFlowOutput start, IFlowInput end, IList<PointEntity> path, int startIndex, int endIndex);
        void AddPump(PointEntity point);
        void AddSink(PointEntity point);
        void AddSplitter(PointEntity point);
        void ComponentPropertyChanged(IComponent component);
        void DeleteComponent(IComponent component);
        void DuplicateComponent(IComponent component, PointEntity point);
        bool FileAlreadyExist(string path);
        void MoveComponent(IComponent component, PointEntity point);
        void OpenFile(string path);
        void SaveFile(string path);
    }
}