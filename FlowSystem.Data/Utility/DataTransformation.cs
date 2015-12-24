using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;
using FlowSystem.Data.Files;

namespace FlowSystem.Data.Utility
{
    public static class DataTransformation
    {
        private static int _componentNr;

        private static IEnumerable<ComponentFile<T>> GetComponentsFileOfType<T>(this FlowNetworkEntity source)
        {
            return source.Components.OfType<T>().Select(x => x.ToComponentFile());
        }

        private static IEnumerable<PipeFile> GetPipesFile(this FlowNetworkEntity source, IReadOnlyDictionary<IComponentEntityEntity, int> components)
        {
            return source.Pipes.Select(pipeEntity => new PipeFile
            {
                Pipe = pipeEntity,
                StartComponent = components[pipeEntity.StartComponent],
                EndComponent = components[pipeEntity.EndComponent]
            });
        }

        public static FlowFile ToFlowFile(this FlowNetworkEntity source)
        {
            // Add to each component an Id
            var mergers = source.GetComponentsFileOfType<MergerEntity>().ToList();
            var pumps = source.GetComponentsFileOfType<PumpEntity>().ToList();
            var sinks = source.GetComponentsFileOfType<SinkEntity>().ToList();
            var splitters = source.GetComponentsFileOfType<SplitterEntity>().ToList();

            // Put all components in dictionary with the key to find the Id's
            var components = new Dictionary<IComponentEntityEntity, int>();
            mergers.ForEach(x => components[x.Component] = x.Id);
            pumps.ForEach(x => components[x.Component] = x.Id);
            sinks.ForEach(x => components[x.Component] = x.Id);
            splitters.ForEach(x => components[x.Component] = x.Id);

            // Create the pipes, the relation between the components is through the Id's previously generated.
            var pipes = source.GetPipesFile(components);

            return new FlowFile
            {
                Mergers = mergers,
                Pipes = pipes.ToList(),
                Pumps = pumps,
                Sinks = sinks,
                Splitters = splitters
            };
        }

        public static FlowNetworkEntity FromFlowFile(this FlowFile source)
        {
            // Combine all components
            var components = new List<IComponentEntityEntity>();
            components.AddRange(source.Mergers.Select(x => x.Component));
            components.AddRange(source.Pumps.Select(x => x.Component));
            components.AddRange(source.Sinks.Select(x => x.Component));
            components.AddRange(source.Splitters.Select(x => x.Component));

            // Put Id's and components in dictionary so relation can be added again
            var ids = new Dictionary<int, IComponentEntityEntity>();
            source.Mergers.ForEach(x => ids[x.Id] = x.Component);
            source.Pumps.ForEach(x => ids[x.Id] = x.Component);
            source.Sinks.ForEach(x => ids[x.Id] = x.Component);
            source.Splitters.ForEach(x => ids[x.Id] = x.Component);

            // Make pipes and add the relation
            var pipes = source.Pipes.Select(x =>
            {
                var pipe = x.Pipe;
                pipe.StartComponent = ids[x.StartComponent] as IFlowOutput;
                pipe.EndComponent = ids[x.EndComponent] as IFlowInput;
                return pipe;
            }).ToList();

            return new FlowNetworkEntity
            {
                Components = components,
                Pipes = pipes
            };
        }

        public static ComponentFile<T> ToComponentFile<T>(this T source)
        {
            return new ComponentFile<T>
            {
                Component = source,
                Id = _componentNr++
            };
        }
    }
}