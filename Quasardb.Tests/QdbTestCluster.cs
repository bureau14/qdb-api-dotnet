using System;
using System.IO;
using Quasardb.TimeSeries;

namespace Quasardb.Tests
{
    class QdbTestCluster
    {
        static QdbTestCluster()
        {
            if (DaemonRunner.UseSecurity)
            {
                Instance = new QdbCluster(
                    DaemonRunner.ClusterUrl, DaemonRunner.ClusterPublicKey,
                    DaemonRunner.UserName, DaemonRunner.UserPrivateKey);
            }
            else
            {
                Instance = new QdbCluster(DaemonRunner.ClusterUrl);
            }
        }

        public static QdbCluster Instance { get; }

        #region Blob

        public static QdbBlob CreateEmptyBlob(string alias = null)
        {
            return Instance.Blob(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbBlob CreateBlob(string alias = null)
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

        #region Integer

        public static QdbInteger CreateEmptyInteger(string alias = null)
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

        #region Double

        public static QdbDouble CreateEmptyDouble(string alias = null)
        {
            return Instance.Double(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbDouble CreateQdbDouble(string alias = null)
        {
            var d = CreateEmptyDouble(alias);
            d.Put(42.34);
            return d;
        }

        public static QdbDouble CreateTaggedQdbDouble(QdbTag tag)
        {
            var d = CreateQdbDouble();
            d.AttachTag(tag);
            return d;
        }

        #endregion

        #region String

        public static QdbString CreateEmptyString(string alias = null)
        {
            return Instance.String(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbString CreateString(string alias = null)
        {
            var s = CreateEmptyString(alias);
            s.Put(RandomGenerator.CreateRandomString());
            return s;
        }

        public static QdbString CreateTaggedString(QdbTag tag)
        {
            var s = CreateString();
            s.AttachTag(tag);
            return s;
        }

        #endregion

        #region Timestamp

        public static QdbTimestamp CreateEmptyTimestamp(string alias = null)
        {
            return Instance.Timestamp(alias ?? RandomGenerator.CreateUniqueAlias());
        }

        public static QdbTimestamp CreateTimestamp(string alias = null)
        {
            var t = CreateEmptyTimestamp(alias);
            t.Put(DateTime.UtcNow);
            return t;
        }

        public static QdbTimestamp CreateTaggedTimestamp(QdbTag tag)
        {
            var ts = CreateTimestamp();
            ts.AttachTag(tag);
            return ts;
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

        #region Table

        public static QdbDoubleColumn CreateEmptyDoubleColumn(string alias = null, string name = null)
        {
            var ts = Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbDoubleColumnDefinition(colName));
            return ts.DoubleColumns[colName];
        }

        public static QdbDoubleColumn CreateDoubleColumn(string alias = null, string name = null)
        {
            var column = CreateEmptyDoubleColumn(alias, null);
            column.Insert(DateTime.Now, 666);
            return column;
        }

        public static QdbInt64Column CreateEmptyInt64Column(string alias = null, string name = null)
        {
            var ts = Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbInt64ColumnDefinition(colName));
            return ts.Int64Columns[colName];
        }

        public static QdbInt64Column CreateInt64Column(string alias = null, string name = null)
        {
            var column = CreateEmptyInt64Column(alias, null);
            column.Insert(DateTime.Now, 666);
            return column;
        }
        public static QdbTimestampColumn CreateEmptyTimestampColumn(string alias = null, string name = null)
        {
            var ts = Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbTimestampColumnDefinition(colName));
            return ts.TimestampColumns[colName];
        }

        public static QdbTimestampColumn CreateTimestampColumn(string alias = null, string name = null)
        {
            var column = CreateEmptyTimestampColumn(alias, null);
            column.Insert(DateTime.Now, DateTime.Now);
            return column;
        }

        public static QdbBlobColumn CreateEmptyBlobColumn(string alias = null)
        {
            var ts = Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbBlobColumnDefinition(colName));
            return ts.BlobColumns[colName];
        }

        public static QdbBlobColumn CreateBlobColumn(string alias = null)
        {
            var column = CreateEmptyBlobColumn(alias);
            column.Insert(DateTime.Now, RandomGenerator.CreateRandomContent());
            return column;
        }

        public static QdbDoubleColumn GetNonExistingDoubleColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = Instance.Table(alias);
            ts.Create(new QdbBlobColumnDefinition("existing"));
            return ts.DoubleColumns["non-existing"];
        }

        public static QdbInt64Column GetNonExistingInt64Column()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = Instance.Table(alias);
            ts.Create(new QdbBlobColumnDefinition("existing"));
            return ts.Int64Columns["non-existing"];
        }

        public static QdbTimestampColumn GetNonExistingTimestampColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = Instance.Table(alias);
            ts.Create(new QdbBlobColumnDefinition("existing"));
            return ts.TimestampColumns["non-existing"];
        }

        public static QdbBlobColumn GetNonExistingBlobColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = Instance.Table(alias);
            ts.Create(new QdbBlobColumnDefinition("existing"));
            return ts.BlobColumns["non-existing"];
        }

        #endregion
    }
}
