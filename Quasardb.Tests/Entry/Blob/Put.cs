﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Blob
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Put(null);
        }

        [TestMethod]
        public void NoError_WhenCalledOnce()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
        }


        [TestMethod]
        public void NoError_WithZeroLengthContent()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Put(Array.Empty<byte>());
            var result = blob.Get();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ThrowsAliasAlreadyExists_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
        
            try
            {
                blob.Put(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void NoError_AfterRemove()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.Remove();
            blob.Put(content);
        }

        [TestMethod]
        public void NoError_AfterRemoveIf()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.RemoveIf(content);
            blob.Put(content);
        }

        [TestMethod]
        public void NoError_AfterGetAndRemove()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.GetAndRemove();
            blob.Put(content);
        }
    }
}
