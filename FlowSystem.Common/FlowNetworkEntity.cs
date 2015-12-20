using System.Collections.Generic;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common
{
    public class FlowNetworkEntity
    {
        public IList<IComponent> Components { get; set; }
        public IList<PipeEntity> Pipes { get; set; }
    }
}