Imports System
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Normalizer to map (min to max) -> (newMin-to newMax) linearly. <br/>
    ''' Mathematically: (newMax-newMin)/(max-min) * (x-min) + newMin
    ''' Equality compares the class, the column name and the min/max values (and the derived ratio, as Lombok does).
    ''' </summary>
    ' Java: @Data @JsonIgnoreProperties({"ratio", "inputSchema", "columnNumber"})
    <Serializable>
    Public Class MinMaxNormalizer
        Inherits BaseDoubleTransform

        Protected Friend ReadOnly min As Double
        Protected Friend ReadOnly max As Double
        Protected Friend ReadOnly newMin As Double
        Protected Friend ReadOnly newMax As Double
        Protected Friend ReadOnly ratio As Double

        Public Sub New(ByVal columnName As String, ByVal min As Double, ByVal max As Double)
            Me.New(columnName, min, max, 0, 1)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal min As Double, ByVal max As Double, ByVal newMin As Double, ByVal newMax As Double)
            MyBase.New(columnName)
            Me.min = min
            Me.max = max
            Me.newMin = newMin
            Me.newMax = newMax
            Me.ratio = (newMax - newMin) / (max - min)
        End Sub

        ''' <summary>Java <c>getMin()</c> (Lombok).</summary>
        Public Function getMin() As Double
            Return min
        End Function

        ''' <summary>Java <c>getMax()</c> (Lombok).</summary>
        Public Function getMax() As Double
            Return max
        End Function

        ''' <summary>Java <c>getNewMin()</c> (Lombok).</summary>
        Public Function getNewMin() As Double
            Return newMin
        End Function

        ''' <summary>Java <c>getNewMax()</c> (Lombok).</summary>
        Public Function getNewMax() As Double
            Return newMax
        End Function

        ''' <summary>Java <c>getRatio()</c> (Lombok).</summary>
        Public Function getRatio() As Double
            Return ratio
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim val As Double = writable.toDouble()
            If Double.IsNaN(val) Then
                Return New DoubleWritable(0)
            End If
            Return New DoubleWritable(ratio * (val - min) + newMin)
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
            Return New DoubleMetaData(newColumnName, newMin, newMax)
        End Function

        Public Overrides Function ToString() As String
            Return "MinMaxNormalizer(min=" & Arrays.JavaDoubleToString(min) & ",max=" & Arrays.JavaDoubleToString(max) & ",newMin=" & Arrays.JavaDoubleToString(newMin) & ",newMax=" & Arrays.JavaDoubleToString(newMax) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the normalized value of a number (as in Java, a NaN input gives <c>DoubleWritable(0)</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim val As Double = JavaNumbers.doubleValue(input)
            If Double.IsNaN(val) Then
                Return New DoubleWritable(0)
            End If
            Return ratio * (val - min) + newMin
        End Function

        ''' <summary>Equal when the class, the column name and min, max, newMin, newMax and ratio are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As MinMaxNormalizer = DirectCast(obj, MinMaxNormalizer)
            Return min.Equals(o.min) AndAlso max.Equals(o.max) AndAlso newMin.Equals(o.newMin) AndAlso newMax.Equals(o.newMax) AndAlso ratio.Equals(o.ratio)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), min, max, newMin, newMax, ratio)
        End Function

    End Class

End Namespace
