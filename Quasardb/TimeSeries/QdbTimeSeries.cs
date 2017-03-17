using Quasardb.ManagedApi;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of columns
    /// </summary>
    public class QdbColumnCollection
    {
        readonly QdbTimeSeries _series;

        internal QdbColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <summary>
        /// Gets the columns with the specified name
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbColumn this[string name]
        {
            get { return new QdbDoubleColumn(_series, name);}
        }
    }

    /// <summary>
    /// A time series
    /// </summary>
    public class QdbTimeSeries : QdbEntry
    {
        internal QdbTimeSeries(QdbApi api, string alias) : base(api, alias)
        {
            Columns = new QdbColumnCollection(this);
        }

        /// <summary>
        /// The columns of the time series
        /// </summary>
        public QdbColumnCollection Columns { get; }
    }
}
