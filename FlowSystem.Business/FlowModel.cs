using System;
using System.Collections.Generic;
using System.Linq;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business
{
    public class FlowModel : IFlowModel
    {
        private const int ComponentWidth = 64;
        private const int ComponentHeight = 64;

        public FlowNetworkEntity FlowNetwork { get; set; }

        public FlowModel()
        {
            FlowNetwork = new FlowNetworkEntity
            {
                Components = new List<IComponent>(),
                Pipes = new List<PipeEntity>()
            };
        }

        private bool PositionFree(PointEntity point)
        {
            return !FlowNetwork.Components.Any(x =>
                x.Position.X <= point.X &&
                x.Position.X + ComponentWidth > point.X &&
                x.Position.Y <= point.Y &&
                x.Position.Y + ComponentHeight > point.Y
                );
        }

#region Add
        public void AddMerger(PointEntity point)
        {
            if (!PositionFree(point))
                throw new Exception("Position where component is place is not free.");

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

            // Check if the input or output is already used
            if (FlowNetwork.Pipes.Any(x =>
                (x.StartComponent == start && x.StartComponentIndex == startIndex) ||
                (x.EndComponent == end && x.EndComponentIndex == endIndex)))
                throw new Exception("Can't connect a pipe to the same component twice.");

            FlowNetwork.Pipes.Add(
                new PipeEntity
                { 
                    CurrentFlow = 0,
                    EndComponent = end,
                    EndComponentIndex = endIndex,
                    MaximumFlow = 0,
                    Path = path,
                    StartComponent = start,
                    StartComponentIndex = startIndex
                });
        }

        public void AddPump(PointEntity point)
        {
            if (!PositionFree(point))
                throw new Exception("Position where component is place is not free.");

            FlowNetwork.Components.Add(
                new PumpEntity
                {
                    FlowOutput = new double[1],
                    Position = point
                });
        }

        public void AddSink(PointEntity point)
        {
            if (!PositionFree(point))
                throw new Exception("Position where component is place is not free.");

            FlowNetwork.Components.Add(
                new SinkEntity
                {
                    FlowInput = new double[1],
                    Position = point
                });
        }

        public void AddSplitter(PointEntity point)
        {
            if (!PositionFree(point))
                throw new Exception("Position where component is place is not free.");

            FlowNetwork.Components.Add(
                new SplitterEntity
                {
                    FlowInput = new double[1],
                    FlowOutput = new double[2],
                    Position = point
                });
        }
#endregion

        public void ComponentPropertyChanged(IComponent component)
        {
            throw new System.NotImplementedException();
        }
#region Delete
        public void DeleteComponent(IComponent component)
        {
            // get all pipes connected to component
            var pipes = FlowNetwork.Pipes.Where(x =>
                x.StartComponent == component ||
                x.EndComponent == component);
            
            // delete those pipes
            pipes.ToList().ForEach(x =>
                FlowNetwork.Pipes.Remove(x));

            FlowNetwork.Components.Remove(component);
        }

        public void DeletePipe(PipeEntity pipe)
        {
            FlowNetwork.Pipes.Remove(pipe);
        }
#endregion

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