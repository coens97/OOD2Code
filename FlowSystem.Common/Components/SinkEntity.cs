using System;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class SinkEntity : IComponent, IFlowInput
    {
        public PointEntity Position { get; set; }
        public double[] FlowInput { get; set; }
    }
}