using System;
using FlowSystem.Business;
using FlowSystem.Business.Interfaces;
using FlowSystem.Data;
using FlowSystem.Data.Interfaces;
using FlowSystem.Presentation;
using Ninject;

namespace FlowSystem.Launcher
{
    public static class Startup
    {
        [STAThread]
        public static void Main()
        {
            // Handle the dependencies
            var container = new StandardKernel();
            container.Bind<IFlowModel>().To<FlowModel>().InTransientScope();
            container.Bind<IDataAccesLayer>().To<DataAccesLayer>().InTransientScope();
            container.Bind<IFlowCalculator>().To<FlowCalculator>().InSingletonScope();
            container.Bind<MainWindow>().ToSelf().InSingletonScope();

            // Start application
            var window = container.Get<MainWindow>();
            window.ShowDialog();
        }
    }
}