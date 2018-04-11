using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class Misc
    {
        [TestMethod, TestCategory("Insecure")]
        public void CreateAndDispose_Insecure()
        {
            if (DaemonRunner.UseSecurity) Assert.Inconclusive();

            new QdbCluster(DaemonRunner.ClusterUrl).Dispose();
        }

        [TestMethod, TestCategory("Secure")]
        public void CreateAndDispose_Secure()
        {
            if (!DaemonRunner.UseSecurity) Assert.Inconclusive();

            new QdbCluster(DaemonRunner.ClusterUrl,
                           DaemonRunner.ClusterPublicKey, DaemonRunner.UserName,
                           DaemonRunner.UserPrivateKey)
                .Dispose();
        }

        [TestMethod, TestCategory("Secure")]
        [ExpectedException(typeof(QdbRemoteSystemException))]
        public void WrongPublicKey()
        {
            if (!DaemonRunner.UseSecurity) Assert.Inconclusive();

            new QdbCluster(DaemonRunner.ClusterUrl,
                           "PK1VnpzfwPCfB4pYqqUiRD5TP7gHOOUHdKTJGAKRStGc=",
                           DaemonRunner.UserName, DaemonRunner.UserPrivateKey)
                .Dispose();
        }

        [TestMethod, TestCategory("Secure")]
        [ExpectedException(typeof(QdbRemoteSystemException))]
        public void WrongUserName()
        {
            if (!DaemonRunner.UseSecurity) Assert.Inconclusive();

            new QdbCluster(DaemonRunner.ClusterUrl,
                           DaemonRunner.ClusterPublicKey, "unexisting-user",
                           DaemonRunner.UserPrivateKey)
                .Dispose();
        }

        [TestMethod, TestCategory("Secure")]
        [ExpectedException(typeof(QdbRemoteSystemException))]
        public void WrongUserPrivateKey()
        {
            if (!DaemonRunner.UseSecurity) Assert.Inconclusive();

            new QdbCluster(DaemonRunner.ClusterUrl,
                           DaemonRunner.ClusterPublicKey, DaemonRunner.UserName,
                           "SCTTE1TmTm8nz98ELpV/CSfqoaunupg5D0aQ20YIWAaE=")
                .Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void Constructor_ThrowsInvalidArgument()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster("clearly this is not a uri");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsArgumentNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Blob_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Blob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Integer_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Integer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Queue_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Deque(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Stream_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Stream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Tag_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Tag(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TimeSeries_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.TimeSeries(null);
        }

        [TestMethod]
        public void SetCompression_None_ThrowsNothing()
        {
            QdbTestCluster.Instance.SetCompression(Quasardb.QdbCompression.None);
        }

        [TestMethod]
        public void SetCompression_Fast_ThrowsNothing()
        {
            QdbTestCluster.Instance.SetCompression(Quasardb.QdbCompression.Fast);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbRemoteSystemException))]
        public void SetCompression_Best_ThrowsRemoteSystemException_NotImplemented()
        {
            QdbTestCluster.Instance.SetCompression(Quasardb.QdbCompression.Best);
        }
    }
}
