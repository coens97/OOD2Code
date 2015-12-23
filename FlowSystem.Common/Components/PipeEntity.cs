using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class PipeEntity
    {
        public double MaximumFlow { get; set; }
        public double CurrentFlow { get; set; }

        [XmlIgnore]
        public IFlowOutput StartComponent { get; set; }
        [XmlIgnore]
        public IFlowInput EndComponent { get; set; }
        public int StartComponentIndex { get; set; }
        public int EndComponentIndex { get; set; }
        public List<PointEntity> Path { get; set; } 
    }
}
