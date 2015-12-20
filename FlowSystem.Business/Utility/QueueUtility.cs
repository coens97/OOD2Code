using System.Collections.Generic;

namespace FlowSystem.Business.Utility
{
    public static class QueueUtility
    {
        public static void EnqueueRange<T>(this Queue<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection)
                source.Enqueue(item);
        }
    }
}