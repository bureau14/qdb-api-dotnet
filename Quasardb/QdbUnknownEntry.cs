using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// An entry whose type is not supported by the .NET API
    /// </summary>
    public class QdbUnknownEntry : QdbEntry
    {
        internal QdbUnknownEntry(qdb_handle handle, string alias, qdb_entry_type type) : base(handle, alias)
        {
            EntryType = (int)type;
        }
        
        /// <summary>
        /// The identifier of the type.
        /// </summary>
        public int EntryType { get; }
    }
}