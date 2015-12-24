namespace FlowSystem.Common.Interfaces
{
    public interface IFlowOutput : IComponentEntityEntity
    {
        double[] FlowOutput { get; set; }
    }
}
