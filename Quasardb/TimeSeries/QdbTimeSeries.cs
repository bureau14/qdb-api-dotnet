using Quasardb.ManagedApi;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of columns
    /// </summary>
    public class QdbDoubleColumnCollection
    {
        readonly QdbTimeSeries _series;

        internal QdbDoubleColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <summary>
        /// Gets the columns with the specified name
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbDoubleColumn this[string name]
        {
            get { return new QdbDoubleColumn(_series, name);}
        }
    }

    /// <summary>
    /// A collection of columns of blobs
    /// </summary>
    public class QdbBlobColumnCollection
    {
        readonly QdbTimeSeries _series;

        internal QdbBlobColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <summary>
        /// Gets the columns with the specified name
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbBlobColumn this[string name]
        {
            get { return new QdbBlobColumn(_series, name); }
        }
    }

    /// <summary>
    /// A time series
    /// </summary>
    public class QdbTimeSeries : QdbEntry
    {
        internal QdbTimeSeries(QdbApi api, string alias) : base(api, alias)
        {
            DoubleColumns = new QdbDoubleColumnCollection(this);
            BlobColumns = new QdbBlobColumnCollection(this);
        }

        /// <summary>
        /// The columns of the time series
        /// </summary>
        public QdbDoubleColumnCollection DoubleColumns { get; }

        /// <summary>
        /// The columns of the time series that contains blobs
        /// </summary>
        public QdbBlobColumnCollection BlobColumns { get; }
    }
}
