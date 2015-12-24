namespace FlowSystem.Common.Interfaces
{
    public interface IFlowInput : IComponentEntity
    {
        double[] FlowInput { get; set; }
    }
}