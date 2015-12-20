namespace FlowSystem.Common.Interfaces
{
    public interface IFlowInput : IComponent
    {
        double[] FlowInput { get; set; }
    }
}