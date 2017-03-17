using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Stream
{
    [TestClass]
    public class Open
    {
        [TestMethod]
        public void ThrowsAliasNotFoundException_OnRandomAlias()
        {
            var stream = QdbTestCluster.Instance.Stream(RandomGenerator.CreateUniqueAlias());
            
            try
            {
                stream.Open(QdbStreamMode.Open);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(stream.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsAliasNotFoundException_AfterRemove()
        {
            var stream = QdbTestCluster.CreateStream();
            stream.Remove();

            try
            {
                stream.Open(QdbStreamMode.Open);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(stream.Alias, e.Alias);
            }
        }
        
        [TestMethod]
        public void ThrowsIncompatibleTypeException_OpenMode()
        {
            var blob = QdbTestCluster.CreateBlob();
            
            try
            {
                QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Open);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleTypeException_AppendMode()
        {
            var blob = QdbTestCluster.CreateBlob();

            try
            {
                QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Append);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }
    }
}
