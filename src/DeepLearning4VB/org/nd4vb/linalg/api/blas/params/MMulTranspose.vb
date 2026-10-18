Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.api.blas.params

    ''' <summary>
    ''' The transpose flags of a matrix multiplication (A, B and the result). Lombok's <c>@Getter</c>,
    ''' <c>@EqualsAndHashCode</c> and <c>@Builder</c> members are written out:
    ''' <c>MMulTranspose.builder().transposeA(True).build()</c>.
    ''' </summary>
    <Serializable>
    Public Class MMulTranspose

        Private Shared ReadOnly allFalseValue As MMulTranspose = MMulTranspose.builder().build()

        Private transposeA As Boolean
        Private transposeB As Boolean
        Private transposeResult As Boolean

        Public Sub New(ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal transposeResult As Boolean)
            Me.transposeA = transposeA
            Me.transposeB = transposeB
            Me.transposeResult = transposeResult
        End Sub

        ''' <summary>
        ''' Returns the default transpose
        ''' where all are false
        ''' </summary>
        Public Shared Function allFalse() As MMulTranspose
            Return allFalseValue
        End Function

        Public Shared Function builder() As MMulTransposeBuilder
            Return New MMulTransposeBuilder()
        End Function

        Public Overridable Function isTransposeA() As Boolean
            Return transposeA
        End Function

        Public Overridable Function isTransposeB() As Boolean
            Return transposeB
        End Function

        Public Overridable Function isTransposeResult() As Boolean
            Return transposeResult
        End Function

        ''' <summary>
        ''' Execute the matrix multiplication: A x B
        ''' Note that if a or b have transposeA/B == true, then this is done internally.
        ''' Also, if transposeResult == true, then this is also done internally - i.e., the result array - if present -
        ''' should not be transposed beforehand. </summary>
        ''' <param name="a">      A array </param>
        ''' <param name="b">      B array </param>
        ''' <param name="result"> Result array (pre resultArrayTranspose if required). May be null. </param>
        ''' <returns> Result array </returns>
        Public Overridable Function exec(ByVal a As INDArray, ByVal b As INDArray, ByVal result As INDArray) As INDArray
            a = transposeIfReq(transposeA, a)
            b = transposeIfReq(transposeB, b)
            If result Is Nothing Then
                Dim ret As INDArray = a.mmul(b)
                Return transposeIfReq(transposeResult, ret)
            Else
                If Not transposeResult Then
                    Return a.mmuli(b, result)
                Else
                    Return a.mmuli(b, result).transpose()
                End If
            End If
        End Function

        Private Shared Function transposeIfReq(ByVal transpose As Boolean, ByVal x As INDArray) As INDArray
            If transpose Then
                If x.rank() = 2 Then
                    Return x.transpose()
                End If
                If x.rank() = 3 Then
                    Return x.permute(0, 2, 1)
                End If
            End If
            Return x
        End Function

        ''' <summary>The value of one of this object's fields (Java <c>getValue(Field)</c>).</summary>
        Public Overridable Function getValue(ByVal [property] As FieldInfo) As Object
            Return [property].GetValue(Me)
        End Function

        Public Overridable Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
            ret("transposeA") = transposeA
            ret("transposeB") = transposeB
            ret("transposeResult") = transposeResult
            Return ret
        End Function

        Public Overridable Sub setProperties(ByVal properties As IDictionary(Of String, Object))
            If properties.ContainsKey("transposeA") Then
                transposeA = CBool(properties("transposeA"))
            End If
            If properties.ContainsKey("transposeB") Then
                transposeB = CBool(properties("transposeB"))
            End If
            If properties.ContainsKey("transposeResult") Then
                transposeResult = CBool(properties("transposeResult"))
            End If
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As MMulTranspose = TryCast(obj, MMulTranspose)
            If o Is Nothing Then Return False
            Return transposeA = o.transposeA AndAlso transposeB = o.transposeB AndAlso transposeResult = o.transposeResult
        End Function

        Public Overrides Function GetHashCode() As Integer
            ' Lombok: PRIME 59, 79 for true and 97 for false
            Dim result As Integer = 1
            result = result * 59 + If(transposeA, 79, 97)
            result = result * 59 + If(transposeB, 79, 97)
            result = result * 59 + If(transposeResult, 79, 97)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "MMulTranspose(transposeA=" & If(transposeA, "true", "false") & ", transposeB=" & If(transposeB, "true", "false") & ", transposeResult=" & If(transposeResult, "true", "false") & ")"
        End Function

        ''' <summary>Lombok's builder for <see cref="MMulTranspose"/>.</summary>
        Public Class MMulTransposeBuilder
            Private transposeAValue As Boolean
            Private transposeBValue As Boolean
            Private transposeResultValue As Boolean

            Public Function transposeA(ByVal value As Boolean) As MMulTransposeBuilder
                Me.transposeAValue = value
                Return Me
            End Function

            Public Function transposeB(ByVal value As Boolean) As MMulTransposeBuilder
                Me.transposeBValue = value
                Return Me
            End Function

            Public Function transposeResult(ByVal value As Boolean) As MMulTransposeBuilder
                Me.transposeResultValue = value
                Return Me
            End Function

            Public Function build() As MMulTranspose
                Return New MMulTranspose(transposeAValue, transposeBValue, transposeResultValue)
            End Function
        End Class

    End Class

End Namespace
