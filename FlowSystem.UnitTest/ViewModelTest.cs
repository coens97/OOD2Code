using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowSystem.Presentation.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlowSystem.UnitTest
{
    [TestClass]
    public class ViewModelTest
    {
        [TestMethod]
        public void TestPumpViewModel()
        {
            var pump = new PumpViewModel();

            var changed = false;

            pump.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "CurrentFlow")
                    {
                        changed = true;
                    }
                };
            pump.MaximumFlow = 20;
            Assert.IsFalse(changed);
            pump.CurrentFlow = 16;
            Assert.IsTrue(changed);
        }
    }
}
