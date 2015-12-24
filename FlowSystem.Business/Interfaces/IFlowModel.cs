using System.Collections.Generic;
using System.ComponentModel;
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
        void DeleteComponent(IComponentEntity component);
        void DeletePipe(PipeEntity pipe);
        void DuplicateComponent(IComponentEntity component, PointEntity point);
        bool FileAlreadyExist(string path);
        void MoveComponent(IComponentEntity component, PointEntity point);
        void OpenFile(string path);
        void SaveFile(string path);
        void PumpPropertyChanged(PumpEntity pump, PropertyChangedEventArgs e, PumpEntity newPump);
    }
}