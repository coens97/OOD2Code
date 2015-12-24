using System.Collections.Generic;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business.Interfaces
{
    public interface IFlowModel
    {
        FlowNetworkEntity FlowNetwork { get; set; }
        MergerEntity AddMerger(PointEntity point);
        PipeEntity AddPipe(IFlowOutput start, IFlowInput end, IList<PointEntity> path, int startIndex, int endIndex);
        PumpEntity AddPump(PointEntity point);
        SinkEntity AddSink(PointEntity point);
        SplitterEntity AddSplitter(PointEntity point);
        void ComponentPropertyChanged(IComponent component);
        void DeleteComponent(IComponent component);
        void DeletePipe(PipeEntity pipe);
        void DuplicateComponent(IComponent component, PointEntity point);
        bool FileAlreadyExist(string path);
        void MoveComponent(IComponent component, PointEntity point);
        void OpenFile(string path);
        void SaveFile(string path);
    }
}