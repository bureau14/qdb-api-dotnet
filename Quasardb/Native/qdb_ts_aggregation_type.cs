namespace Quasardb.Native
{
    internal enum qdb_ts_aggregation_type
    {
        First = 0,
        Last = 1,
        Min = 2,
        Max = 3,
        Average = 4,
        ArithmeticMean = 4,
        HarmonicMean = 5,
        GeometricMean = 6,
        QuadraticMean = 7,
        Count = 8,
        Sum = 9,
        SumOfSquares = 10,
        Spread = 11,
        SampleVariance = 12,
        SampleStdDev = 13,
        PopulationVariance = 14,
        PopulationStdDev = 15,
        AbsMin = 16,
        AbsMax = 17,
        Product = 18,
        Skewness = 19,
        Kurtosis = 20,
        None = 21,
        DistinctCount = 22,
    }
}
