using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;
using FlowSystem.Business.Utility;

namespace FlowSystem.Business
{
    public class FlowCalculator : IFlowCalculator
    {
        private static void ProcessPipequeue(FlowNetworkEntity flowNetwork, Queue<PipeEntity> pipeQueue)
        {
            if (!pipeQueue.Any())
                return;

            var pipe = pipeQueue.Dequeue();
            while (pipe != null)
            {
                pipe.CurrentFlow = pipe.StartComponent.FlowOutput[pipe.StartComponentIndex];
                pipe.EndComponent.FlowInput[pipe.EndComponentIndex] = pipe.CurrentFlow;

                ProcessComponent(pipe.EndComponent);
                // push the pipes connected to the endcomponent to the queue
                pipeQueue.EnqueueRange(flowNetwork.Pipes.Where(x => x.StartComponent == pipe.EndComponent));

                pipe = pipeQueue.Any() ? pipeQueue.Dequeue() : null;
            }
        }
        public void UpdateAll(FlowNetworkEntity flowNetwork, Action done)
        {
            // Get all the pumps and set the flowOutput right
            var pumps = flowNetwork.Components.OfType<PumpEntity>().ToList();
            pumps.ForEach(ProcessComponent);

            // Create a queue of pipes connectsed to the pumps to it
            var pipeQueue = new Queue<PipeEntity>();
            pipeQueue.EnqueueRange(flowNetwork.Pipes.Where(x => pumps.Contains(x.StartComponent)));

            ProcessPipequeue(flowNetwork, pipeQueue);

            done?.Invoke();
        }

        private static void ProcessComponent(IComponentEntity component)
        {
            var merger = component as MergerEntity;
            var splitter = component as SplitterEntity;
            var pump = component as PumpEntity;

            if (merger != null)
            {
                merger.FlowOutput[0] = merger.FlowInput[0] + merger.FlowInput[1];
            }
            else if (splitter != null)
            {
                splitter.FlowOutput[0] = splitter.FlowInput[0]*(splitter.Distrubution/100.0);
                splitter.FlowOutput[1] = splitter.FlowInput[0]*((100 - splitter.Distrubution)/100.0);
            }
            else if (pump != null)
            {
                pump.FlowOutput[0] = pump.CurrentFlow;
            }
        }

        public void UpdateFrom(FlowNetworkEntity flowNetwork, IComponentEntity component, Action done)
        {
            ProcessComponent(component);

            var pipeQueue = new Queue<PipeEntity>();
            pipeQueue.EnqueueRange(flowNetwork.Pipes.Where(x => x.StartComponent == component));
            ProcessPipequeue(flowNetwork, pipeQueue);

            done?.Invoke();
        }
    }

}