using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Double
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            try
            {
                d.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var d = QdbTestCluster.CreateEmptyDouble(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                d.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsSum_AfterAdd()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Put(1934.33);
            d.Add(22.11);
            var sum = d.Get();

            Assert.AreEqual(1956.44, sum, 0.001);
        }

        [TestMethod]
        public void ReturnsValue_AfterPut()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Put(42.33);
            var value = d.Get();
            
            Assert.AreEqual(42.33, value);
        }

        [TestMethod]
        public void ReturnsNewValue_AfterUpdate()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Put(1934.33);
            d.Update(1956.22);
            var value = d.Get();

            Assert.AreEqual(1956.22, value, 0.001);
        }
    }
}
