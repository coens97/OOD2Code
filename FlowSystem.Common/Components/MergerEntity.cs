﻿using System;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Common.Components
{
    [Serializable]
    public class MergerEntity : IComponentEntityEntity, IFlowOutput, IFlowInput
    {
        public PointEntity Position { get; set; }
        public double[] FlowOutput { get; set; }
        public double[] FlowInput { get; set; }
    }
}