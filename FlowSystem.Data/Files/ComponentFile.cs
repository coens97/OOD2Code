using System;

namespace FlowSystem.Data.Files
{
    [Serializable]
    public class ComponentFile <T>
    {
        public int Id { get; set; }
        public T Component { get; set; }
    }
}