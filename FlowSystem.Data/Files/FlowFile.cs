using System;
using System.Collections.Generic;
using FlowSystem.Common.Components;

namespace FlowSystem.Data.Files
{
    [Serializable]
    public class FlowFile
    {
        public List<ComponentFile<MergerEntity>> Mergers { get; set; }
        public List<PipeFile> Pipes { get; set; }
        public List<ComponentFile<PumpEntity>> Pumps { get; set; }
        public List<ComponentFile<SinkEntity>> Sinks { get; set; }
        public List<ComponentFile<SplitterEntity>> Splitters { get; set; } 
    }
}