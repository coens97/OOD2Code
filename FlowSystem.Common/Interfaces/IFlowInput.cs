namespace FlowSystem.Common.Interfaces
{
    public interface IFlowInput : IComponentEntityEntity
    {
        double[] FlowInput { get; set; }
    }
}