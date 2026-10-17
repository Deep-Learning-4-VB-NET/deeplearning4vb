Imports System
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.longtransform

    ''' <summary>
    ''' Long mathematical operation.<br/>
    ''' This is an in-place operation of the long column value and a long scalar (long division).
    ''' Equality compares the class, the column name, the op and the scalar.
    ''' </summary>
    ''' <seealso cref="LongColumnsMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class LongMathOpTransform
        Inherits BaseColumnTransform

        Private ReadOnly mathOpField As MathOp
        Private ReadOnly scalar As Long

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Long)
            MyBase.New(columnName)
            Me.mathOpField = mathOp
            Me.scalar = scalar
        End Sub

        ''' <summary>Java <c>getMathOp()</c> (Lombok).</summary>
        Public Function getMathOp() As MathOp
            Return mathOpField
        End Function

        ''' <summary>Java <c>getScalar()</c> (Lombok).</summary>
        Public Function getScalar() As Long
            Return scalar
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            If Not (TypeOf oldColumnType Is LongMetaData) Then
                Throw New InvalidOperationException("Column is not an Long column")
            End If
            Dim meta As LongMetaData = DirectCast(oldColumnType, LongMetaData)
            Dim minValue As Long? = meta.getMinAllowedValue()
            Dim maxValue As Long? = meta.getMaxAllowedValue()
            If minValue.HasValue Then
                minValue = doOp(minValue.Value)
            End If
            If maxValue.HasValue Then
                maxValue = doOp(maxValue.Value)
            End If
            If minValue.HasValue AndAlso maxValue.HasValue AndAlso minValue.Value > maxValue.Value Then
                'Consider rsub 1, with original min/max of 0 and 1: (1-0) -> 1 and (1-1) -> 0
                'Or multiplication by -1: (0 to 1) -> (-1 to 0)
                'Need to swap min/max here...
                Dim temp As Long? = minValue
                minValue = maxValue
                maxValue = temp
            End If
            Return New LongMetaData(newName, minValue, maxValue)
        End Function

        Private Function doOp(ByVal input As Long) As Long
            Select Case mathOpField
                Case MathOp.Add
                    Return input + scalar
                Case MathOp.Subtract
                    Return input - scalar
                Case MathOp.Multiply
                    Return input * scalar
                Case MathOp.Divide
                    Return input \ scalar
                Case MathOp.Modulus
                    Return input Mod scalar
                Case MathOp.ReverseSubtract
                    Return scalar - input
                Case MathOp.ReverseDivide
                    Return scalar \ input
                Case MathOp.ScalarMin
                    Return Math.Min(input, scalar)
                Case MathOp.ScalarMax
                    Return Math.Max(input, scalar)
                Case Else
                    Throw New InvalidOperationException("Unknown or not implemented math op: " & mathOpField.ToString())
            End Select
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Return New LongWritable(doOp(columnWritable.toLong()))
        End Function

        Public Overrides Function ToString() As String
            Return "LongMathOpTransform(mathOp=" & mathOpField.ToString() & ",scalar=" & scalar & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a number (Java <c>((Number) input).longValue()</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return doOp(JavaNumbers.longValue(input))
        End Function

        ''' <summary>Equal when the class, the column name, the op and the scalar are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As LongMathOpTransform = DirectCast(obj, LongMathOpTransform)
            Return mathOpField = o.mathOpField AndAlso scalar = o.scalar
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mathOpField, scalar)
        End Function

    End Class

End Namespace
