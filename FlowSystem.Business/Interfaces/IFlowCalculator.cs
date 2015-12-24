using FlowSystem.Common;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business.Interfaces
{
    public interface IFlowCalculator
    {
        void UpdateAll(FlowNetworkEntity flowNetwork);
        void UpdateFrom(FlowNetworkEntity flowNetwork, IComponentEntityEntity component);
    }
}