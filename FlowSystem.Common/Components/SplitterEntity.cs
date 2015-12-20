using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    public class SplitterEntity : IComponent, IFlowInput, IFlowOutput
    {
        public int Distrubution { get; set; }
        public double[] FlowInput { get; set; }
        public double[] FlowOutput { get; set; }
        public PointEntity Position { get; set; }
    }
}