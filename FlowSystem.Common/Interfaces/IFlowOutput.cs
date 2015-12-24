namespace FlowSystem.Common.Interfaces
{
    public interface IFlowOutput : IComponentEntity
    {
        double[] FlowOutput { get; set; }
    }
}
