using System;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class SplitterEntity : IComponentEntity, IFlowInput, IFlowOutput
    {
        public int Distrubution { get; set; }
        public double[] FlowInput { get; set; }
        public double[] FlowOutput { get; set; }
        public PointEntity Position { get; set; }
    }
}