using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// A tag in a quasardb database.
    /// </summary>
    /// <remarks>
    /// QdbTag can be constructed via <see cref="QdbCluster.Tag" />.
    /// </remarks>
    /// <example>
    /// How to tag a blob:
    /// <code language="c#">
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// cluster.Blob("some name").AttachTag("some tag");
    /// </code>
    /// <code language="vb">
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// cluster.Blob("some name").AttachTag("some tag")
    /// </code>
    /// How to get tagged entries:
    /// <code language="c#">
    /// var cluster = New QdbCluster("qdb://127.0.0.1:2836");
    /// IEnumerable&lt;QdbEntry&gt; tagged = cluster.Tag("some tag").GetEntries();
    /// </code>
    /// <code language="vb">
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// Dim tagged = cluster.Tag("some tag").GetEntries()
    /// </code>
    /// </example>
    /// <seealso cref="QdbCluster"/>
    /// <seealso cref="QdbEntry"/>
    public sealed class QdbTag : QdbEntry
    {
        internal QdbTag(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Gets database entries which are tagged with the current tag.
        /// </summary>
        /// <returns>A collection of entry.</returns>
        /// <seealso cref="QdbEntry"/>
        public IEnumerable<QdbEntry> GetEntries()
        {
            var factory = new QdbEntryFactory(Handle);
            
            using (var result = new qdb_buffer<qdb_string>(Handle))
            { 
                var error = qdb_api.qdb_get_tagged(Handle, Alias, out result.Pointer, out result.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);

                // ReSharper disable once LoopCanBeConvertedToQuery
                // To stay compatible with .NET Framework 2.0
                foreach (var alias in result)
                    yield return factory.Create(alias.ToString());
            }
        }

        /// <summary>
        /// Adds a tag to a database entry.
        /// </summary>
        /// <param name="entry">The entry to add the tag to.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with an existing entry.</exception>
        public bool AttachEntry(QdbEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return AttachTag(entry.Alias, Alias);
        }

        /// <summary>
        /// Adds a tag to a database entry.
        /// </summary>
        /// <param name="entry">The alias of the entry to add the tag to.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with an existing entry.</exception>
        public bool AttachEntry(string entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return AttachTag(entry, Alias);
        }

        /// <summary>
        /// Checks if an entry has this tag.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        public bool HasEntry(QdbEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return HasTag(entry.Alias, Alias);
        }

        /// <summary>
        /// Checks if an entry has this tag.
        /// </summary>
        /// <param name="entry">The alias of the entry to check.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        public bool HasEntry(string entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return HasTag(entry, Alias);
        }

        /// <summary>
        /// Removes a tag from a database entry.
        /// </summary>
        /// <param name="entry">The entry to remove the tag from.</param>
        /// <returns><c>true</c> if the tag was removed, <c>false</c> if the entry didn't have this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public bool DetachEntry(QdbEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return DetachTag(entry.Alias, Alias);
        }

        /// <summary>
        /// Removes a tag from a database entry.
        /// </summary>
        /// <param name="entry">The alias of the entry to remove the tag from.</param>
        /// <returns><c>true</c> if the tag was removed, <c>false</c> if the entry didn't have this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public bool DetachEntry(string entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return DetachTag(entry, Alias);
        }
    }
}
