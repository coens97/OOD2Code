using System;
using System.Drawing;
using FlowSystem.Common.Interfaces;
using FontAwesome.WPF;

namespace FlowSystem.Presentation
{
    public class ComponentControl : ImageAwesome
    {
        public IComponent Component { get; set; }
        public ComponentControl(IComponent component)
        {
            Width = 32;
            Height = 32;
            Component = component;

            switch (component.GetType().ToString())
            {
                case "FlowSystem.Common.Components.MergerEntity":
                    Icon = FontAwesomeIcon.ChevronRight;
                    break;
                case "FlowSystem.Common.Components.PumpEntity":
                    Icon = FontAwesomeIcon.SignIn;
                    break;
                case "FlowSystem.Common.Components.SinkEntity":
                    Icon = FontAwesomeIcon.SignOut;
                    break;
                case "FlowSystem.Common.Components.SplitterEntity":
                    Icon = FontAwesomeIcon.ChevronLeft;
                    break;
                default:
                    throw new Exception("Error happened, the developer forgot to implement something...sorry :(");
            }
        }
    }
}