Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.FloatMetaData
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.floattransform

    ''' <summary>
    ''' Float mathematical operation.<br/>
    ''' This is an in-place operation of the float column value and a float scalar.
    ''' Equality compares the class, the column name, the op and the scalar.
    ''' </summary>
    ''' <seealso cref="FloatColumnsMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class FloatMathOpTransform
        Inherits BaseColumnTransform

        Private ReadOnly mathOpField As MathOp
        Private ReadOnly scalar As Single

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Single)
            MyBase.New(columnName)
            Me.mathOpField = mathOp
            Me.scalar = scalar
        End Sub

        ''' <summary>Java <c>getMathOp()</c> (Lombok).</summary>
        Public Function getMathOp() As MathOp
            Return mathOpField
        End Function

        ''' <summary>Java <c>getScalar()</c> (Lombok).</summary>
        Public Function getScalar() As Single
            Return scalar
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            If Not (TypeOf oldColumnType Is FloatMetaData) Then
                Throw New InvalidOperationException("Column is not an float column")
            End If
            Dim meta As FloatMetaData = DirectCast(oldColumnType, FloatMetaData)
            Dim minValue As Single? = meta.getMinAllowedValue()
            Dim maxValue As Single? = meta.getMaxAllowedValue()
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
                Dim temp As Single? = minValue
                minValue = maxValue
                maxValue = temp
            End If
            Return New FloatMetaData(newColumnName, minValue, maxValue)
        End Function

        Private Function doOp(ByVal input As Single) As Single
            Select Case mathOpField
                Case MathOp.Add
                    Return input + scalar
                Case MathOp.Subtract
                    Return input - scalar
                Case MathOp.Multiply
                    Return input * scalar
                Case MathOp.Divide
                    Return input / scalar
                Case MathOp.Modulus
                    Return input Mod scalar
                Case MathOp.ReverseSubtract
                    Return scalar - input
                Case MathOp.ReverseDivide
                    Return scalar / input
                Case MathOp.ScalarMin
                    Return Math.Min(input, scalar)
                Case MathOp.ScalarMax
                    Return Math.Max(input, scalar)
                Case Else
                    Throw New InvalidOperationException("Unknown or not implemented math op: " & mathOpField.ToString())
            End Select
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Return New FloatWritable(doOp(columnWritable.toFloat()))
        End Function

        Public Overrides Function ToString() As String
            Return "FloatMathOpTransform(mathOp=" & mathOpField.ToString() & ",scalar=" & Arrays.JavaFloatToString(scalar) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a number
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            If JavaNumbers.isNumber(input) Then
                Return doOp(JavaNumbers.floatValue(input))
            End If
            Throw New ArgumentException("Input must be a number")
        End Function

        ''' <summary>Equal when the class, the column name, the op and the scalar are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As FloatMathOpTransform = DirectCast(obj, FloatMathOpTransform)
            Return mathOpField = o.mathOpField AndAlso scalar.Equals(o.scalar)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mathOpField, scalar)
        End Function

    End Class

End Namespace
