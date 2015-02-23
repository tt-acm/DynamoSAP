using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTestServices;
using NUnit.Framework;

namespace DynamoSAP_Tests
{
    [TestFixture]
    public class HelloTester : SystemTestBase
    {   
        [Test]
        public void ian() 
        {
            AssertNoDummyNodes();
        }
    }
}
