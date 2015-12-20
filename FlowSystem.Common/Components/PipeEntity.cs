using System;
using System.Collections.Generic;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class PipeEntity
    {
        public double MaximumFlow { get; set; }
        public double CurrentFlow { get; set; }
        public IFlowOutput StartComponent { get; set; }
        public IFlowInput EndComponent { get; set; }
        public int StartComponentIndex { get; set; }
        public int EndComponentIndex { get; set; }
        public IList<PointEntity> Path { get; set; } 
    }
}
