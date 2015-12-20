﻿using System;
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
    public class UnitTest
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
            var x = 100;
            var y = 200;
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
            var y = 200;
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

        [TestMethod] [ExpectedException(typeof (ArgumentException), "Pipe can't connect to output which is not there")]
        public void TestAddPipeFailNull()
        {
            _flowModel.AddPipe(null, null, new List<PointEntity>(), 0, 0);

            Assert.AreEqual(_flowModel.FlowNetwork.Pipes.Count, 0);
        }
    }
}