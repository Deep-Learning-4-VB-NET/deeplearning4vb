Imports System
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Normalize by taking scale * log2((in-columnMin)/(mean-columnMin) + 1)
    ''' Maps values in range (columnMin to infinity) to (0 to infinity)
    ''' Most suitable for values with a geometric/negative exponential type distribution.
    ''' Equality compares the class, the column name, the mean, the minimum and the scaling factor.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class Log2Normalizer
        Inherits BaseDoubleTransform

        Protected Friend Shared ReadOnly log2Value As Double = Math.Log(2)
        Protected Friend ReadOnly columnMean As Double
        Protected Friend ReadOnly columnMin As Double
        Protected Friend ReadOnly scalingFactor As Double

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal columnMean As Double, ByVal columnMin As Double, ByVal scalingFactor As Double)
            MyBase.New(columnName)
            If Double.IsNaN(columnMean) OrElse Double.IsInfinity(columnMean) Then
                Throw New ArgumentException("Invalid input: column mean cannot be null/infinite (is: " & Arrays.JavaDoubleToString(columnMean) & ")")
            End If
            Me.columnMean = columnMean
            Me.columnMin = columnMin
            Me.scalingFactor = scalingFactor
        End Sub

        ''' <summary>Java <c>getColumnMean()</c> (Lombok).</summary>
        Public Function getColumnMean() As Double
            Return columnMean
        End Function

        ''' <summary>Java <c>getColumnMin()</c> (Lombok).</summary>
        Public Function getColumnMin() As Double
            Return columnMin
        End Function

        ''' <summary>Java <c>getScalingFactor()</c> (Lombok).</summary>
        Public Function getScalingFactor() As Double
            Return scalingFactor
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim val As Double = writable.toDouble()
            If Double.IsNaN(val) Then
                Return New DoubleWritable(0)
            End If
            Return New DoubleWritable(normMean(val))
        End Function

        Private Shared Function log2(ByVal x As Double) As Double
            Return Math.Log(x) / log2Value
        End Function

        Private Function normMean(ByVal [in] As Double) As Double
            Return scalingFactor * log2(([in] - columnMin) / (columnMean - columnMin) + 1)
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
            Return New DoubleMetaData(newColumnName, 0.0, Nothing)
        End Function

        Public Overrides Function ToString() As String
            Return "Log2Normalizer(columnMean=" & Arrays.JavaDoubleToString(columnMean) & ",columnMin=" & Arrays.JavaDoubleToString(columnMin) & ",scalingFactor=" & Arrays.JavaDoubleToString(scalingFactor) & ")"
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
            Return normMean(val)
        End Function

        ''' <summary>Equal when the class, the column name, the mean, the minimum and the scaling factor are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As Log2Normalizer = DirectCast(obj, Log2Normalizer)
            Return columnMean.Equals(o.columnMean) AndAlso columnMin.Equals(o.columnMin) AndAlso scalingFactor.Equals(o.scalingFactor)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), columnMean, columnMin, scalingFactor)
        End Function

    End Class

End Namespace
