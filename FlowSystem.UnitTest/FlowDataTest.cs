using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FlowSystem.Business;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Common.Interfaces;
using FlowSystem.Data;
using FlowSystem.Data.Interfaces;
using FlowSystem.Data.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace FlowSystem.UnitTest
{
    [TestClass]
    public class FlowDataTest
    {
        private IKernel _container;
        private IDataAccesLayer _dataAccesLayer;
        private FlowNetworkEntity _flowNetwork;

        private PumpEntity _pump1 = new PumpEntity { CurrentFlow = 4.0, FlowOutput = new[] { 4.0 } };
        private PumpEntity _pump2 = new PumpEntity { CurrentFlow = 6.0, FlowOutput = new[] { 6.0 } };
        private MergerEntity _merger = new MergerEntity { FlowOutput = new[] { 0.0 }, FlowInput = new[] { 0.0, 0.0 } };
        private SplitterEntity _splitter = new SplitterEntity { FlowOutput = new[] { 0.0, 0.0 }, FlowInput = new[] { 0.0 }, Distrubution = 70 };
        private SinkEntity _sink1 = new SinkEntity { FlowInput = new double[] { 0.0 } };
        private SinkEntity _sink2 = new SinkEntity { FlowInput = new double[] { 0.0 } };

        private void MakeTestFlowNetwork()
        {
            _flowNetwork = new FlowNetworkEntity
            {
                Components = new IComponent[]
                {
                    _pump1, _pump2, _merger, _splitter, _sink1, _sink2
                },
                Pipes = new[]
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
            _container.Bind<IDataAccesLayer>().To<DataAccesLayer>().InTransientScope();
            _dataAccesLayer = _container.Get<IDataAccesLayer>();
            MakeTestFlowNetwork();
        }

        [TestMethod]
        public void TestSave()
        {
            _dataAccesLayer.SaveFile(_flowNetwork, "test.xml");
        }

        [TestMethod]
        public void TestOpen()
        {
            TestSave();

            var opened = _dataAccesLayer.OpenFile("test.xml");

            Assert.AreEqual(opened.Components.OfType<SplitterEntity>().First().Distrubution, 70);
            Assert.AreEqual(opened.Pipes.Count(x => x.EndComponent is MergerEntity && x.StartComponent is PumpEntity),2);
        }
    }
}