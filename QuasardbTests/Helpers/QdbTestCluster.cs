using Quasardb;

namespace QuasardbTests.Helpers
{
    class QdbTestCluster
    {
        static QdbTestCluster()
        {
            Instance = new QdbCluster(DaemonRunner.ClusterUrl);
        }
        
        public static QdbCluster Instance { get; private set; }

        #region Blob

        public static QdbBlob CreateEmptyBlob()
        {
            return Instance.Blob(RandomGenerator.CreateUniqueAlias());
        }

        public static QdbBlob CreateBlob()
        {
            var blob = CreateEmptyBlob();
            blob.Put(RandomGenerator.CreateRandomContent());
            return blob;
        }

        public static QdbBlob CreateTaggedBlob(QdbTag tag)
        {
            var blob = CreateBlob();
            blob.AddTag(tag);
            return blob;
        }

        #endregion

        #region HashSet

        public static QdbHashSet CreateEmptyHashSet()
        {
            return Instance.HashSet(RandomGenerator.CreateUniqueAlias());
        }

        public static QdbHashSet CreateHashSet()
        {
            var hashSet = CreateEmptyHashSet();
            hashSet.Insert(RandomGenerator.CreateRandomContent());
            return hashSet;
        }

        public static QdbHashSet CreateTaggedHashSet(QdbTag tag)
        {
            var hashSet = CreateHashSet();
            hashSet.AddTag(tag);
            return hashSet;
        }

        #endregion

        #region Queue

        public static QdbQueue CreateEmptyQueue()
        {
            return Instance.Queue(RandomGenerator.CreateUniqueAlias());
        }

        public static QdbQueue CreateQueue()
        {
            var queue = CreateEmptyQueue();
            queue.PushBack(RandomGenerator.CreateRandomContent());
            return queue;
        }

        public static QdbQueue CreateTaggedQueue(QdbTag tag)
        {
            var queue = CreateQueue();
            queue.AddTag(tag);
            return queue;
        }

        #endregion

        #region Integer

        public static QdbInteger CreateEmptyInteger()
        {
            return Instance.Integer(RandomGenerator.CreateUniqueAlias());
        }

        public static QdbInteger CreateInteger()
        {
            var integer = CreateEmptyInteger();
            integer.Put(42);
            return integer;
        }

        public static QdbInteger CreateTaggedInteger(QdbTag tag)
        {
            var integer = CreateInteger();
            integer.AddTag(tag);
            return integer;
        }

        #endregion

        #region Tag

        public static QdbTag CreateEmptyTag()
        {
            return Instance.Tag(RandomGenerator.CreateUniqueAlias());
        }

        public static QdbTag CreateTag()
        {
            var tag = CreateEmptyTag();
            tag.AddEntry(CreateBlob());
            return tag;
        }

        public static QdbTag CreateTaggedTag(QdbTag tag)
        {
            var newTag = CreateTag();
            newTag.AddTag(tag);
            return newTag;
        }

        #endregion
    }
}
