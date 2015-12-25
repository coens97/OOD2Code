using System;
using FlowSystem.Common;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Business.Interfaces
{
    public interface IFlowCalculator
    {
        void UpdateAll(FlowNetworkEntity flowNetwork, Action done);
        void UpdateFrom(FlowNetworkEntity flowNetwork, IComponentEntity component, Action done);
    }
}