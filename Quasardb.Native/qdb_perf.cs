using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_time_t = System.Int64;

namespace Quasardb.Native
{
    public enum qdb_perf_label : int
    {
        qdb_pl_undefined = 0,
        qdb_pl_accepted = 1,
        qdb_pl_received = 2,
        qdb_pl_secured = 3,
        qdb_pl_deserialization_starts = 4,
        qdb_pl_deserialization_ends = 5,
        qdb_pl_entering_chord = 6,
        qdb_pl_processing_starts = 7,
        qdb_pl_dispatch = 8,
        qdb_pl_serialization_starts = 9,
        qdb_pl_serialization_ends = 10,
        qdb_pl_processing_ends = 11,
        qdb_pl_replying = 12,
        qdb_pl_replied = 13,
        qdb_pl_entry_writing_starts = 14,
        qdb_pl_entry_writing_ends = 15,
        qdb_pl_content_reading_starts = 16,
        qdb_pl_content_reading_ends = 17,
        qdb_pl_content_writing_starts = 18,
        qdb_pl_content_writing_ends = 19,
        qdb_pl_directory_reading_starts = 20,
        qdb_pl_directory_reading_ends = 21,
        qdb_pl_directory_writing_starts = 22,
        qdb_pl_directory_writing_ends = 23,
        qdb_pl_entry_trimming_starts = 24,
        qdb_pl_entry_trimming_ends = 25,
        qdb_pl_ts_evaluating_starts = 26,
        qdb_pl_ts_evaluating_ends = 27,
        qdb_pl_ts_bucket_updating_starts = 28,
        qdb_pl_ts_bucket_updating_ends = 29,
        qdb_pl_affix_search_starts = 30,
        qdb_pl_affix_search_ends = 31,
        qdb_pl_unknown = 255
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_perf_measurement
    {
        qdb_perf_label label;
        qdb_time_t elapsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_perf_profile
    {
        qdb_sized_string name;
        qdb_perf_measurement* measurements;
        qdb_size_t count;
    }
}
