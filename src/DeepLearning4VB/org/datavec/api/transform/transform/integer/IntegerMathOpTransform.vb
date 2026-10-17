Imports System
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Integer mathematical operation.<br/>
    ''' This is an in-place operation of the integer column value and an integer scalar (integer division).
    ''' Equality compares the class, the column name, the op and the scalar.
    ''' </summary>
    ''' <seealso cref="IntegerColumnsMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class IntegerMathOpTransform
        Inherits BaseColumnTransform

        Private ReadOnly mathOpField As MathOp
        Private ReadOnly scalar As Integer

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Integer)
            MyBase.New(columnName)
            Me.mathOpField = mathOp
            Me.scalar = scalar
        End Sub

        ''' <summary>Java <c>getMathOp()</c> (Lombok).</summary>
        Public Function getMathOp() As MathOp
            Return mathOpField
        End Function

        ''' <summary>Java <c>getScalar()</c> (Lombok).</summary>
        Public Function getScalar() As Integer
            Return scalar
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            If Not (TypeOf oldColumnType Is IntegerMetaData) Then
                Throw New InvalidOperationException("Column is not an integer column")
            End If
            Dim meta As IntegerMetaData = DirectCast(oldColumnType, IntegerMetaData)
            Dim minValue As Integer? = meta.getMinAllowedValue()
            Dim maxValue As Integer? = meta.getMaxAllowedValue()
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
                Dim temp As Integer? = minValue
                minValue = maxValue
                maxValue = temp
            End If
            Return New IntegerMetaData(newColumnName, minValue, maxValue)
        End Function

        Private Function doOp(ByVal input As Integer) As Integer
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
            Return New IntWritable(doOp(columnWritable.toInt()))
        End Function

        Public Overrides Function ToString() As String
            Return "IntegerMathOpTransform(mathOp=" & mathOpField.ToString() & ",scalar=" & scalar & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a number (Java <c>((Number) input).intValue()</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return doOp(JavaNumbers.intValue(input))
        End Function

        ''' <summary>Equal when the class, the column name, the op and the scalar are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As IntegerMathOpTransform = DirectCast(obj, IntegerMathOpTransform)
            Return mathOpField = o.mathOpField AndAlso scalar = o.scalar
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mathOpField, scalar)
        End Function

    End Class

End Namespace
