Imports System.Text.Json.Serialization

Namespace org.datavec.api.transform.transforms.normalize

    ''' <summary>
    ''' The normalization types used by the <c>TransformProcess</c> builder's <c>normalize</c> method:
    ''' MinMax (to [0, 1]), MinMax2 (to [-1, 1]), Standardize ((x - mean) / stdev), SubtractMean,
    ''' Log2Mean and Log2MeanExcludingMin (see <c>Log2Normalizer</c>). Serializes by name.
    ''' </summary>
    <JsonConverter(GetType(JsonStringEnumConverter))>
    Public Enum Normalize
        MinMax
        MinMax2
        Standardize
        SubtractMean
        Log2Mean
        Log2MeanExcludingMin
    End Enum

End Namespace
