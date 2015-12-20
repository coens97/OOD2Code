namespace FlowSystem.Common.Interfaces
{
    public interface IFlowOutput : IComponent
    {
        double[] FlowOutput { get; set; }
    }
}
