using System;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class SinkEntity : IComponentEntity, IFlowInput
    {
        public PointEntity Position { get; set; }
        public double[] FlowInput { get; set; }
    }
}