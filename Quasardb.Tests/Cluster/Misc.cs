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
        public void Tag_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Tag(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Table_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Table(null);
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

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void SetMaxInBufferSize_BelowThreshold_ThrowsQdbInvalidArgumentException()
        {
            QdbTestCluster.Instance.SetMaxInBufferSize(1);
        }

        [TestMethod]
        public void SetMaxInBufferSize_Works()
        {
            QdbTestCluster.Instance.SetMaxInBufferSize(128 * 1024 * 1024);
        }

        [TestMethod]
        public void SetMaxParallelism_Works()
        {
            QdbTestCluster.Instance.SetMaxParallelism(8);
        }

        [TestMethod]
        public void SetStabilizationMaxWait_Works()
        {
            QdbTestCluster.Instance.SetStabilizationMaxWait(5 * 60 * 1000);
        }

        [TestMethod]
        public void ClusterTidyMemory_Works()
        {
            QdbTestCluster.Instance.ClusterTidyMemory();
        }

        [TestMethod]
        public void ClientTidyMemory_Works()
        {
            QdbTestCluster.Instance.ClientTidyMemory();
        }

        [TestMethod]
        public void SetSoftMemoryLimit_Works()
        {
            QdbTestCluster.Instance.SetSoftMemoryLimit(128 * 1024 * 1024);
        }

        [TestMethod]
        public void GetClientMemoryInfo_Works()
        {
            var info = QdbTestCluster.Instance.GetClientMemoryInfo();
            StringAssert.Contains(info, "TBB huge threshold bytes");
            StringAssert.Contains(info, "TBB soft limit bytes");
            StringAssert.Contains(info, "TBB total count");
            StringAssert.Contains(info, "TBB max requested bytes");
            StringAssert.Contains(info, "TBB global large object cache bytes");
        }

        [TestMethod]
        public void GetLastErrorForNullQuery()
        {
            try
            {
                QdbTestCluster.Instance.Query(null);
            }
            catch (QdbQueryException)
            {
                Assert.AreEqual("at qdb_query: Got NULL query", QdbTestCluster.Instance.GetLastError());
            }
        }

        [TestMethod]
        public void GetLastErrorForInvalidQuery()
        {
            try
            {
                QdbTestCluster.Instance.SetCompression(Quasardb.QdbCompression.Best);
            }
            catch (QdbRemoteSystemException)
            {
                Assert.AreEqual(
                    "at qdb_option_set_compression: The requested operation is not yet available.", QdbTestCluster.Instance.GetLastError());
            }
        }
    }
} // namespace Cluster
