Imports System
Imports System.Text.Json.Serialization
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Normalize using (x-mean)/stdev.
    ''' Also known as a standard score, standardization etc.
    ''' Equality compares the class, the column name, the mean and the standard deviation.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class StandardizeNormalizer
        Inherits BaseDoubleTransform

        Protected Friend ReadOnly mean As Double
        Protected Friend ReadOnly stdev As Double

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mean As Double, ByVal stdev As Double)
            MyBase.New(columnName)
            Me.mean = mean
            Me.stdev = stdev
        End Sub

        ''' <summary>Java <c>getMean()</c> (Lombok).</summary>
        Public Function getMean() As Double
            Return mean
        End Function

        ''' <summary>Java <c>getStdev()</c> (Lombok).</summary>
        Public Function getStdev() As Double
            Return stdev
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim val As Double = writable.toDouble()
            Return New DoubleWritable((val - mean) / stdev)
        End Function

        Public Overrides Function ToString() As String
            Return "StandardizeNormalizer(mean=" & Arrays.JavaDoubleToString(mean) & ",stdev=" & Arrays.JavaDoubleToString(stdev) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object (as in Java, the result of a number is a <c>DoubleWritable</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim val As Double = JavaNumbers.doubleValue(input)
            Return New DoubleWritable((val - mean) / stdev)
        End Function

        ''' <summary>Equal when the class, the column name, the mean and the standard deviation are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As StandardizeNormalizer = DirectCast(obj, StandardizeNormalizer)
            Return mean.Equals(o.mean) AndAlso stdev.Equals(o.stdev)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mean, stdev)
        End Function

    End Class

End Namespace
