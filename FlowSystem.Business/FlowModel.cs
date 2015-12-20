using System.Collections.Generic;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business
{
    public class FlowModel : IFlowModel
    {
        public FlowNetworkEntity FlowNetwork { get; set; }
        public void AddMerger(PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public void AddPipe(IFlowOutput start, IFlowInput end, IList<PointEntity> path)
        {
            throw new System.NotImplementedException();
        }

        public void AddPump(PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public void AddSink(PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public void AddSplitter(PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public void ComponentPropertyChanged(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteComponent(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public void DuplicateComponent(IComponent component, PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public bool FileAlreadyExist(string path)
        {
            throw new System.NotImplementedException();
        }

        public void MoveComponent(IComponent component, PointEntity point)
        {
            throw new System.NotImplementedException();
        }

        public void OpenFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void SaveFile(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}