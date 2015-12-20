using System;
using System.Collections.Generic;
using System.Linq;
using FlowSystem.Business;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace FlowSystem.UnitTest
{
    [TestClass]
    public class FlowModelTest
    {
        private IKernel _container;
        private IFlowModel _flowModel;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _container = new StandardKernel();
            _container.Bind<IFlowModel>().To<FlowModel>().InTransientScope();
            _flowModel = _container.Get<IFlowModel>();
        }

        [TestMethod]
        public void TestAddMerger()
        {
            var x = 300;
            var y = 400;
            _flowModel.AddMerger(new PointEntity {X = x, Y = y});

            var entity = _flowModel.FlowNetwork.Components.OfType<MergerEntity>().First();

            Assert.AreEqual(entity.Position.X, x);
            Assert.AreEqual(entity.Position.Y, y);
            Assert.AreEqual(entity.FlowOutput.Length, 1);
            Assert.AreEqual(entity.FlowInput.Length, 2);
        }

        [TestMethod]
        public void TestAddPump()
        {
            var x = 100;
            var y = 200;
            _flowModel.AddPump(new PointEntity { X = x, Y = y });

            var entity = _flowModel.FlowNetwork.Components.OfType<PumpEntity>().First();

            Assert.AreEqual(entity.Position.X, x);
            Assert.AreEqual(entity.Position.Y, y);
            Assert.AreEqual(entity.FlowOutput.Length, 1);
        }

        [TestMethod]
        public void TestAddSink()
        {
            var x = 100;
            var y = 200;
            _flowModel.AddSink(new PointEntity { X = x, Y = y });

            var entity = _flowModel.FlowNetwork.Components.OfType<SinkEntity>().First();

            Assert.AreEqual(entity.Position.X, x);
            Assert.AreEqual(entity.Position.Y, y);
            Assert.AreEqual(entity.FlowInput.Length, 1);
        }

        [TestMethod]
        public void TestAddSplitter()
        {
            var x = 100;
            var y = 600;
            _flowModel.AddSplitter(new PointEntity { X = x, Y = y });

            var entity = _flowModel.FlowNetwork.Components.OfType<SplitterEntity>().First();

            Assert.AreEqual(entity.Position.X, x);
            Assert.AreEqual(entity.Position.Y, y);
            Assert.AreEqual(entity.FlowInput.Length, 1);
            Assert.AreEqual(entity.FlowOutput.Length, 2);
        }

        [TestMethod]
        public void TestAddPipe()
        {
            TestAddPump();
            TestAddMerger();

            var pump = _flowModel.FlowNetwork.Components.OfType<PumpEntity>().First();
            var merger = _flowModel.FlowNetwork.Components.OfType<MergerEntity>().First();

            _flowModel.AddPipe(pump, merger, new List<PointEntity>() , 0 , 1);

            var pipe = _flowModel.FlowNetwork.Pipes.First();

            Assert.AreEqual(pipe.StartComponentIndex, 0);
            Assert.AreEqual(pipe.EndComponentIndex, 1);
            Assert.IsInstanceOfType(pipe.StartComponent, typeof(PumpEntity));
            Assert.IsInstanceOfType(pipe.EndComponent, typeof(MergerEntity));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Pipe can't connect to output which is not there")]
        public void TestAddPipeFailIndex()
        {
            TestAddPump();
            TestAddMerger();

            var pump = _flowModel.FlowNetwork.Components.OfType<PumpEntity>().First();
            var merger = _flowModel.FlowNetwork.Components.OfType<MergerEntity>().First();

            _flowModel.AddPipe(pump, merger, new List<PointEntity>(), 2, 1);

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException), "Pipe can't connect to output which is not there")]
        public void TestAddPipeFailNull()
        {
            _flowModel.AddPipe(null, null, new List<PointEntity>(), 0, 0);

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Can't place two components on the same spot")]
        public void TestOverlappingComponents()
        {
            TestAddPump();
            TestAddPump();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Can't connect two pipes to an input or output")]
        public void TestPipeFailConnectToUsedOutput()
        {
            TestAddPipe();
            var pump = _flowModel.FlowNetwork.Components.OfType<PumpEntity>().First();
            var merger = _flowModel.FlowNetwork.Components.OfType<MergerEntity>().First();
            _flowModel.AddPipe(pump, merger, new List<PointEntity>(), 0, 0);
        }

        /// <summary>
        /// Important for this test is to make sure that the pipes connected to the component are also removed
        /// </summary>
        [TestMethod]
        public void TestDeleteComponentWithPipes()
        {
            TestAddPipe();

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 1);

            TestAddSplitter();
            var splitter = _flowModel.FlowNetwork.Components.OfType<SplitterEntity>().First();
            var merger = _flowModel.FlowNetwork.Components.OfType<MergerEntity>().First();
            _flowModel.AddPipe(splitter, merger, new List<PointEntity>(), 0, 0);

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 2);

            _flowModel.DeleteComponent(splitter);

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 1);
            Assert.IsFalse(_flowModel.FlowNetwork.Components.Contains(splitter));
        }

        /// <summary>
        /// Important for this test is to make sure that the pipes connected to the component are also removed
        /// </summary>
        [TestMethod]
        public void TestDuplicate()
        {
            var x = 100;
            var y1 = 100;
            var y2 = 500;

            _flowModel.AddPump(new PointEntity {X = x, Y = y1});

            var pump = _flowModel.FlowNetwork.Components.First();

            _flowModel.DuplicateComponent(pump, new PointEntity {X = x, Y= y2});

            Assert.AreEqual(_flowModel.FlowNetwork.Components.Count(c => c.Position.Y == y1), 1);
            Assert.AreEqual(_flowModel.FlowNetwork.Components.Count(c => c.Position.Y == y2), 1);
        }

        [TestMethod]
        public void TestMoveToSameSpot()
        {
            TestAddPump();
            var pump = _flowModel.FlowNetwork.Components.First();
            _flowModel.MoveComponent(pump, pump.Position);
        }

        [TestMethod]
        public void TestMoveComponent()
        {
            TestAddPump();
            var pump = _flowModel.FlowNetwork.Components.First();
            _flowModel.MoveComponent(pump, new PointEntity {X = 999, Y = 666});

            Assert.AreEqual(pump.Position.X, 999);
            Assert.AreEqual(pump.Position.Y, 666);
        }
    }
}
