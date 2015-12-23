using FlowSystem.Common.Components;

namespace FlowSystem.Data.Files
{
    public class PipeFile
    {
        public PipeEntity Pipe { get; set; }
        public int StartComponent { get; set; }
        public int EndComponent { get; set; }
    }
}