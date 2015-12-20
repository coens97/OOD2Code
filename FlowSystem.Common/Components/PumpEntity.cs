using System;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class PumpEntity : IComponent, IFlowOutput
    {
        public double CurrentFlow { get; set; }
        public double[] FlowOutput { get; set; }
        public double MaximumFlow { get; set; }
        public PointEntity Position { get; set; }
    }
}