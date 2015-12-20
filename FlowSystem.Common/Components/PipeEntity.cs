using System.Collections.Generic;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    public class PipeEntity
    {
        public double MaximumFlow { get; set; }
        public double CurrentFlow { get; set; }
        public IFlowInput StartComponent { get; set; }
        public IFlowOutput EndComponent { get; set; }
        public int StartComponentIndex { get; set; }
        public int EndComponentIndex { get; set; }
        public IList<PointEntity> Path { get; set; } 
    }
}
