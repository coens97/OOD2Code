using System;
using System.Collections.Generic;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common
{
    [Serializable]
    public class FlowNetworkEntity
    {
        public IList<IComponentEntityEntity> Components { get; set; }
        public IList<PipeEntity> Pipes { get; set; }
    }
}