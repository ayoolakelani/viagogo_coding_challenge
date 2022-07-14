using Microsoft.VisualStudio.TestTools.UnitTesting;
using Viagogo;

namespace StubHub.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDistance_From_Same_City_Should_Be_Zero()
        {


           var distance = Solution.GetDistance("New York", "New York");
            Assert.AreEqual(distance, 0);

        }

        [TestMethod]
        public void GetDistance_From_Different_City_Should_Be_X()
        {
            var distance = Solution.GetDistance("A", "B");
            Assert.AreEqual(distance, 1);
        }
    }
}
