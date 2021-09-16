using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_time_t = System.Int64;

namespace Quasardb.Native
{
    internal enum qdb_perf_label : int
    {
        undefined = 0,
        accepted = 1,
        received = 2,
        secured = 3,
        deserialization_starts = 4,
        deserialization_ends = 5,
        entering_chord = 6,
        processing_starts = 7,
        dispatch = 8,
        serialization_starts = 9,
        serialization_ends = 10,
        processing_ends = 11,
        replying = 12,
        replied = 13,
        entry_writing_starts = 14,
        entry_writing_ends = 15,
        content_reading_starts = 16,
        content_reading_ends = 17,
        content_writing_starts = 18,
        content_writing_ends = 19,
        directory_reading_starts = 20,
        directory_reading_ends = 21,
        directory_writing_starts = 22,
        directory_writing_ends = 23,
        entry_trimming_starts = 24,
        entry_trimming_ends = 25,
        ts_evaluating_starts = 26,
        ts_evaluating_ends = 27,
        ts_bucket_updating_starts = 28,
        ts_bucket_updating_ends = 29,
        affix_search_starts = 30,
        affix_search_ends = 31,
        unknown = 255
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_perf_measurement
    {
        internal qdb_perf_label label;
        internal qdb_time_t elapsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_perf_profile
    {
        internal qdb_sized_string name;
        internal qdb_perf_measurement* measurements;
        internal qdb_size_t count;
    }
}
