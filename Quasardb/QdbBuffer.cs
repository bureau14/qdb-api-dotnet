using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbBuffer : IDisposable, ICollection, IReadOnlyList<byte>
    {
        readonly qdb_handle _handle;
        readonly object _syncRoot;

        internal IntPtr Pointer;
        internal IntPtr Size;
        
        internal QdbBuffer(qdb_handle handle)
        {
            _handle = handle;
        }

        ~QdbBuffer()
        {
            Dispose(false);
        }

        public byte[] GetBytes()
        {
            if (Length > int.MaxValue)
                throw new InsufficientMemoryException();

            var buffer = new byte[Size.ToInt64()];
            Marshal.Copy(Pointer, buffer, 0, Size.ToInt32()); // TODO: find how to avoid the cast to int32
            return buffer;
        }

        #region byte[]

        public int Length
        {
            get { return Size.ToInt32(); }
        }

        public long LongLength
        {
            get { return Size.ToInt32(); }
        }
        
        public byte this[int index]
        {
            get
            {
                ThrowIfDisposed();
                ThrowIfIndexOutOfRange(index);
                return Marshal.ReadByte(Pointer, index);
            }
            set
            {
                ThrowIfDisposed();
                ThrowIfIndexOutOfRange(index);
                Marshal.WriteByte(Pointer, index, value);
            }
        }
        
        public byte this[long index]
        {
            get
            {
                ThrowIfDisposed();
                ThrowIfIndexOutOfRange(index);
                return Marshal.ReadByte(new IntPtr((long)Pointer + index));
            }
            set
            {
                ThrowIfDisposed();
                ThrowIfIndexOutOfRange(index);
                Marshal.WriteByte(new IntPtr((long) Pointer + index), value);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
            Pointer = IntPtr.Zero;
            Size = IntPtr.Zero;
        }

        #endregion

        #region ICollection

        int ICollection.Count
        {
            get { return Size.ToInt32(); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfDisposed();
            Marshal.Copy(Pointer, (byte[])array, index, Size.ToInt32());
        }

        object ICollection.SyncRoot
        {
            get { return _syncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        #endregion

        #region IReadOnlyCollection

        int IReadOnlyCollection<byte>.Count
        {
            get { return Length; }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            ThrowIfDisposed();

            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Helpers

        void ThrowIfDisposed()
        {
            if (Pointer != IntPtr.Zero) return;
            if (Size == IntPtr.Zero) return;
            throw new ObjectDisposedException("The QdbBuffer has been disposed", (Exception)null);
        }

        void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException("Index can't be negative.");
            if (index >= Length) throw new IndexOutOfRangeException("Index is bigger than QdbBuffer's size");
        }

        void ThrowIfIndexOutOfRange(long index)
        {
            if (index < 0) throw new IndexOutOfRangeException("Index can't be negative.");
            if (index >= LongLength) throw new IndexOutOfRangeException("Index is bigger than QdbBuffer's size");
        }

        #endregion
    }
}
