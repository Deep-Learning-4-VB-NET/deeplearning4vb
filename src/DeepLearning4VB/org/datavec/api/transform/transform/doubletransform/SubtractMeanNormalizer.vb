Imports System
Imports System.Text.Json.Serialization
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Normalize by subtracting the mean.
    ''' Equality compares the class, the column name and the mean.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class SubtractMeanNormalizer
        Inherits BaseDoubleTransform

        Private ReadOnly mean As Double

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mean As Double)
            MyBase.New(columnName)
            Me.mean = mean
        End Sub

        ''' <summary>Java <c>getMean()</c> (Lombok).</summary>
        Public Function getMean() As Double
            Return mean
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Return New DoubleWritable(writable.toDouble() - mean)
        End Function

        ''' <summary>As in Java (including the spelling).</summary>
        Public Overrides Function ToString() As String
            Return "SubstractMeanNormalizer(mean=" & Arrays.JavaDoubleToString(mean) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: a number minus the mean
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return JavaNumbers.doubleValue(input) - mean
        End Function

        ''' <summary>Equal when the class, the column name and the mean are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso mean.Equals(DirectCast(obj, SubtractMeanNormalizer).mean)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mean)
        End Function

    End Class

End Namespace
