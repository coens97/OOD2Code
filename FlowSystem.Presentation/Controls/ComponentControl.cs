using System;
using System.Windows.Controls;
using System.Windows.Input;
using FlowSystem.Common.Interfaces;
using FontAwesome.WPF;

namespace FlowSystem.Presentation.Controls
{
    public class ComponentControl : ImageAwesome
    {
        public IComponent Component { get; set; }
        public ComponentControl(IComponent component, MouseButtonEventHandler clickEvent)
        {
            Width = 32;
            Height = 32;

            this.MouseDown += clickEvent;
            
            Component = component;
            Canvas.SetLeft(this, Component.Position.X);
            Canvas.SetTop(this, Component.Position.Y);

            switch (component.GetType().ToString())
            {
                case "FlowSystem.Common.Components.MergerEntity":
                    Icon = FontAwesomeIcon.ChevronRight;
                    break;
                case "FlowSystem.Common.Components.PumpEntity":
                    Icon = FontAwesomeIcon.SignOut;
                    break;
                case "FlowSystem.Common.Components.SinkEntity":
                    Icon = FontAwesomeIcon.SignIn;
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