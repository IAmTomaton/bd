using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fridge
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var fridge = new FridgeProgram(null);
            fridge.RunAlgo("2\n\n2 3\n0 1 1\n1 0 0\n\n2 4\n1 1 1 1\n0 0 0 -\n");
        }
    }
}
