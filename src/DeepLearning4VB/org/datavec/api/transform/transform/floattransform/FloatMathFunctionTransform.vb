Imports System
Imports System.Text.Json.Serialization
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.floattransform

    ''' <summary>
    ''' A simple transform to do common mathematical operations, such as sin(x), ceil(x), etc.<br/>
    ''' Equality compares the class, the column name and the function.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class FloatMathFunctionTransform
        Inherits BaseFloatTransform

        Private mathFunctionField As MathFunction

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mathFunction As MathFunction)
            MyBase.New(columnName)
            Me.mathFunctionField = mathFunction
        End Sub

        ''' <summary>Java <c>getMathFunction()</c> (Lombok).</summary>
        Public Function getMathFunction() As MathFunction
            Return mathFunctionField
        End Function

        ''' <summary>Java <c>setMathFunction()</c> (Lombok).</summary>
        Public Sub setMathFunction(ByVal mathFunction As MathFunction)
            Me.mathFunctionField = mathFunction
        End Sub

        Public Overrides Function map(ByVal w As WritableT) As WritableT
            Dim f As Single = w.toFloat()
            Select Case mathFunctionField
                Case MathFunction.ABS
                    Return New FloatWritable(Math.Abs(f))
                Case MathFunction.SIGNUM
                    Return New FloatWritable(signum(f))
                Case Else
                    Return New FloatWritable(CSng(applyDouble(f)))
            End Select
        End Function

        ''' <summary>
        ''' The function applied to a number (Java <c>((Number) input).floatValue()</c>): as in Java, ABS and SIGNUM
        ''' return a Single, the other functions a Double.
        ''' </summary>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim d As Single = JavaNumbers.floatValue(input)
            Select Case mathFunctionField
                Case MathFunction.ABS
                    Return Math.Abs(d)
                Case MathFunction.SIGNUM
                    Return signum(d)
                Case Else
                    Return applyDouble(d)
            End Select
        End Function

        ''' <summary>The double-precision functions (Java <c>Math.xxx(double)</c>).</summary>
        Private Function applyDouble(ByVal d As Double) As Double
            Select Case mathFunctionField
                Case MathFunction.ABS
                    Return Math.Abs(d)
                Case MathFunction.ACOS
                    Return Math.Acos(d)
                Case MathFunction.ASIN
                    Return Math.Asin(d)
                Case MathFunction.ATAN
                    Return Math.Atan(d)
                Case MathFunction.CEIL
                    Return Math.Ceiling(d)
                Case MathFunction.COS
                    Return Math.Cos(d)
                Case MathFunction.COSH
                    Return Math.Cosh(d)
                Case MathFunction.EXP
                    Return Math.Exp(d)
                Case MathFunction.FLOOR
                    Return Math.Floor(d)
                Case MathFunction.LOG
                    Return Math.Log(d)
                Case MathFunction.LOG10
                    Return Math.Log10(d)
                Case MathFunction.SIN
                    Return Math.Sin(d)
                Case MathFunction.SINH
                    Return Math.Sinh(d)
                Case MathFunction.SQRT
                    Return Math.Sqrt(d)
                Case MathFunction.TAN
                    Return Math.Tan(d)
                Case MathFunction.TANH
                    Return Math.Tanh(d)
                Case Else
                    Throw New Exception("Unknown function: " & mathFunctionField.ToString())
            End Select
        End Function

        ''' <summary>Java <c>Math.signum(float)</c>: NaN and signed zeros are returned unchanged.</summary>
        Private Shared Function signum(ByVal f As Single) As Single
            If Single.IsNaN(f) OrElse f = 0.0F Then Return f
            Return If(f > 0, 1.0F, -1.0F)
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "FloatMathFunctionTransform(mathFunction=" & mathFunctionField.ToString() & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the function are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso mathFunctionField = DirectCast(obj, FloatMathFunctionTransform).mathFunctionField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mathFunctionField)
        End Function

    End Class

End Namespace
