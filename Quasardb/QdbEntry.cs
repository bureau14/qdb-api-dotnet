﻿using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// An entry in a quasardb database.
    /// </summary>
    public abstract class QdbEntry
    {
        internal QdbEntry(QdbApi api, string alias)
        {
            Alias = alias;
            Api = api;
        }

        /// <summary>
        /// The alias of the entry in the database.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The Managed API wrapper.
        /// </summary>
        internal QdbApi Api { get; private set; }

        /// <summary>
        /// Removes the entry from the database.
        /// </summary>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public virtual void Remove()
        {
            Api.Remove(Alias);
        }

        /// <summary>
        /// Adds a tag to the entry.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with another entry.</exception>
        /// <seealso cref="QdbTag"/>
        public bool AddTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.AddTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Adds a tag to the entry.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with another entry.</exception>
        /// <seealso cref="QdbTag"/>
        public bool AddTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.AddTag(Alias, tag);
        }

        /// <summary>
        /// Gets the tags of the entry
        /// </summary>
        /// <returns>A collection of tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public IEnumerable<QdbTag> GetTags()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery (compatibility with .NET Framework 2.0)
            foreach (var tag in Api.GetTags(Alias))
                yield return new QdbTag(Api, tag);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        /// <seealso cref="QdbTag"/>
        public bool HasTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.HasTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        /// <seealso cref="QdbTag"/>
        public bool HasTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.HasTag(Alias, tag);
        }

        /// <summary>
        /// Removes a tag from the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public bool RemoveTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.RemoveTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Removes a tag from the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public bool RemoveTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            return Api.RemoveTag(Alias, tag);
        }
    }
}
