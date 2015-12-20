using System;
using System.Collections.Generic;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business
{
    public class FlowModel : IFlowModel
    {
        public FlowNetworkEntity FlowNetwork { get; set; }

        public FlowModel()
        {
            FlowNetwork = new FlowNetworkEntity
            {
                Components = new List<IComponent>(),
                Pipes = new List<PipeEntity>()
            };
        }

        public void AddMerger(PointEntity point)
        {
            FlowNetwork.Components.Add(
                new MergerEntity
                {
                    FlowInput = new double[2],
                    FlowOutput = new double[1],
                    Position = point
                });
        }

        public void AddPipe(IFlowOutput start, IFlowInput end, IList<PointEntity> path, int startIndex, int endIndex)
        {
            if (start == null || end == null)
                throw new ArgumentException("Start component or end component not given");

            if (startIndex < 0 || startIndex >= start.FlowOutput.Length
                || endIndex < 0 || endIndex >= end.FlowInput.Length)
                throw new ArgumentException("Unable to connect pipe to component, index is out of range");

            FlowNetwork.Pipes.Add(
                new PipeEntity
                { 
                    CurrentFlow = 0,
                    EndComponent = start,
                    EndComponentIndex = endIndex,
                    MaximumFlow = 0,
                    Path = path,
                    StartComponent = end,
                    StartComponentIndex = startIndex
                });
        }

        public void AddPump(PointEntity point)
        {
            FlowNetwork.Components.Add(
                new PumpEntity
                {
                    FlowOutput = new double[1],
                    Position = point
                });
        }

        public void AddSink(PointEntity point)
        {
            FlowNetwork.Components.Add(
                new SinkEntity
                {
                    FlowInput = new double[1],
                    Position = point
                });
        }

        public void AddSplitter(PointEntity point)
        {
            FlowNetwork.Components.Add(
                new SplitterEntity
                {
                    FlowInput = new double[1],
                    FlowOutput = new double[2],
                    Position = point
                });
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