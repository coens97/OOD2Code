using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    public class MergerEntity : IComponent, IFlowOutput, IFlowInput
    {
        public PointEntity Position { get; set; }
        public double[] FlowOutput { get; set; }
        public double[] FlowInput { get; set; }
    
    }
}