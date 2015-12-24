using System.Collections.Generic;
using FlowSystem.Business;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace FlowSystem.UnitTest
{
    /// <summary>
    /// Note that this is an unit test, so only the flow calculator is tested, the rules of flowmodel don't apply here :)
    /// </summary>
    [TestClass]
    public class FlowCalculatorTest
    {
        private IKernel _container;
        private IFlowCalculator _flowCalculator;
        private FlowNetworkEntity _flowNetwork;

        private PumpEntity _pump1 = new PumpEntity { CurrentFlow = 4.0, FlowOutput = new[] { 4.0 } };
        private PumpEntity _pump2 = new PumpEntity { CurrentFlow = 6.0, FlowOutput = new[] { 6.0 } };
        private MergerEntity _merger = new MergerEntity { FlowOutput = new[] { 0.0 }, FlowInput = new[] { 0.0, 0.0 } };
        private SplitterEntity _splitter = new SplitterEntity { FlowOutput = new[] { 0.0, 0.0 }, FlowInput = new[] { 0.0 } , Distrubution = 70};
        private SinkEntity _sink1 = new SinkEntity { FlowInput = new double[] { 0.0 } };
        private SinkEntity _sink2 = new SinkEntity { FlowInput = new double[] { 0.0 } };

        private void MakeTestFlowNetwork()
        {
            _flowNetwork = new FlowNetworkEntity
            {
                Components = new IComponentEntity[]
                {
                    _pump1, _pump2, _merger, _splitter, _sink1, _sink2
                },
                Pipes = new []
                {
                    new PipeEntity { EndComponent = _merger, EndComponentIndex = 0, StartComponent = _pump1, StartComponentIndex = 0},
                    new PipeEntity { EndComponent = _merger, EndComponentIndex = 1, StartComponent = _pump2, StartComponentIndex = 0},
                    new PipeEntity { EndComponent = _splitter, EndComponentIndex = 0, StartComponent = _merger, StartComponentIndex = 0},
                    new PipeEntity { EndComponent = _sink1,  EndComponentIndex = 0, StartComponent = _splitter, StartComponentIndex = 0}, // Splitter needs index to device flow
                    new PipeEntity { EndComponent = _sink2, EndComponentIndex = 0, StartComponent = _splitter, StartComponentIndex = 1}
                }
            };
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _container = new StandardKernel();
            _container.Bind<IFlowCalculator>().To<FlowCalculator>().InTransientScope();
            _flowCalculator = _container.Get<IFlowCalculator>();
            MakeTestFlowNetwork();
        }

        [TestMethod]
        public void TestCalculation()
        {
            _flowCalculator.UpdateAll(_flowNetwork);

            Assert.AreEqual(_sink1.FlowInput[0], 7.0);
            Assert.AreEqual(_sink2.FlowInput[0], 3.0);
        }

        [TestMethod]
        public void TestCalculationFromComponent()
        {
            _flowCalculator.UpdateAll(_flowNetwork);

            _pump2.CurrentFlow = 16;

            _flowCalculator.UpdateFrom(_flowNetwork,_pump2);

            Assert.AreEqual(_sink1.FlowInput[0], 14.0);
            Assert.AreEqual(_sink2.FlowInput[0], 6.0);
        }
    }
}