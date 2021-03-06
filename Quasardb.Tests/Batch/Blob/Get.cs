﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Batch.Blob
{
    [TestClass]
    public class Get
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnsContentOfBlob()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();

            _cluster.Blob(alias).Put(content);
            var future = batch.Blob(alias).Get();
            _cluster.RunBatch(batch);

            Assert.AreEqual(content.Length, future.Result.Length);
            CollectionAssert.AreEqual(content, future.Result);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            var future = batch.Blob(alias).Get();
            _cluster.RunBatch(batch);

            try
            {
                var dummy = future.Result;
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }
    }
}