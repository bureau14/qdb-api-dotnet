using System;
using System.IO;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Helpers
{
    class QdbTestCluster
    {
        static QdbTestCluster()
        {
            Instance = new QdbCluster(DaemonRunner.ClusterUrl);
        }

        public static QdbCluster Instance { get; }

        #region Blob

        public static QdbBlob CreateEmptyBlob(string alias = null)
        {
            return Instance.Blob(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbBlob CreateBlob(string alias=null)
        {
            var blob = CreateEmptyBlob(alias);
            blob.Put(RandomGenerator.CreateRandomContent());
            return blob;
        }

        public static QdbBlob CreateTaggedBlob(QdbTag tag)
        {
            var blob = CreateBlob();
            blob.AttachTag(tag);
            return blob;
        }

        #endregion

        #region HashSet

        public static QdbHashSet CreateEmptyHashSet(string alias = null)
        {
            return Instance.HashSet(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbHashSet CreateHashSet(string alias = null)
        {
            var hashSet = CreateEmptyHashSet(alias);
            hashSet.Insert(RandomGenerator.CreateRandomContent());
            return hashSet;
        }

        public static QdbHashSet CreateTaggedHashSet(QdbTag tag)
        {
            var hashSet = CreateHashSet();
            hashSet.AttachTag(tag);
            return hashSet;
        }

        #endregion

        #region Deque

        public static QdbDeque CreateEmptyQueue(string alias=null)
        {
            return Instance.Deque(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbDeque CreateQueue(string alias = null)
        {
            var queue = CreateEmptyQueue(alias);
            queue.PushBack(RandomGenerator.CreateRandomContent());
            return queue;
        }

        public static QdbDeque CreateTaggedQueue(QdbTag tag)
        {
            var queue = CreateQueue();
            queue.AttachTag(tag);
            return queue;
        }

        #endregion

        #region Integer

        public static QdbInteger CreateEmptyInteger(string alias=null)
        {
            return Instance.Integer(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbInteger CreateInteger(string alias = null)
        {
            var integer = CreateEmptyInteger(alias);
            integer.Put(42);
            return integer;
        }

        public static QdbInteger CreateTaggedInteger(QdbTag tag)
        {
            var integer = CreateInteger();
            integer.AttachTag(tag);
            return integer;
        }

        #endregion

        #region Tag

        public static QdbTag CreateEmptyTag(string alias = null)
        {
            return Instance.Tag(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbTag CreateTag(string alias = null)
        {
            var tag = CreateEmptyTag(alias);
            tag.AttachEntry(CreateBlob());
            return tag;
        }

        public static QdbTag CreateTaggedTag(QdbTag tag)
        {
            var newTag = CreateTag();
            newTag.AttachTag(tag);
            return newTag;
        }

        #endregion

        #region Stream

        public static Stream CreateAndOpenStream()
        {
            return CreateEmptyStream().Open(QdbStreamMode.Append);
        }

        public static QdbStream CreateEmptyStream(string alias = null)
        {
            return Instance.Stream(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbStream CreateStream(string alias = null)
        {
            var qdbStream = CreateEmptyStream(alias);
            using (var stream = qdbStream.Open(QdbStreamMode.Append))
            {
                stream.WriteByte(1);
            }
            return qdbStream;
        }

        #endregion
        
        #region TimeSeries

        public static QdbDoubleColumn CreateEmptyTimeSeries(string alias = null)
        {
            return Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias()).Columns[RandomGenerator.CreateUniqueAlias()] as QdbDoubleColumn;
        }

        public static QdbDoubleColumn CreateTimeSeries(string alias = null)
        {
            var ts = CreateEmptyTimeSeries(alias);
            ts.Insert(DateTime.Now, 666);
            return ts;
        }

        #endregion
    }
}
