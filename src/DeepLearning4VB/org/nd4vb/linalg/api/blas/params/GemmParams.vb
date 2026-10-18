Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBArraySizeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBArraySizeException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.blas.params

    ''' <summary>
    ''' The arguments of a BLAS gemm call for C = A * B (Java <c>GemmParams</c>): the operands (A and B copied when their
    ''' layout cannot be passed to BLAS), the dimensions m, n and k, the leading dimensions and the transpose flags.
    ''' When the backend does not let BLAS calls specify the order (<c>Nd4vb.allowsSpecifyOrdering()</c> is False, as
    ''' for the CPU and managed backends), everything is described in column-major terms: a 'c' operand is passed
    ''' transposed.
    ''' <para>
    ''' Lombok's <c>@Data</c> members are written out: the getters and setters, <see cref="Equals"/>,
    ''' <see cref="GetHashCode"/> and <see cref="ToString"/>.
    ''' </para>
    ''' </summary>
    Public Class GemmParams
        Private lda, ldb, ldc, m, n, k As Integer
        Private a, b, c As INDArray
        Private transA As Char = "N"c
        Private transB As Char = "N"c
        Private ordering As Char = "f"c

        ''' <summary>The parameters of C = A * B.</summary>
        ''' <param name="a"> </param>
        ''' <param name="b"> </param>
        ''' <param name="c"> </param>
        ''' <exception cref="ArgumentException">The shapes do not fit.</exception>
        Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray)
            If a.columns() <> b.rows() Then
                Throw New ArgumentException("A columns must equal B rows. MMul attempt: " & shapeText(a) & "x" & shapeText(b))
            End If
            If b.columns() <> c.columns() Then
                Throw New ArgumentException("B columns must match C columns. MMul attempt: " & shapeText(a) & "x" & shapeText(b) & "; result array provided: " & shapeText(c))
            End If
            If a.rows() <> c.rows() Then
                Throw New ArgumentException("A rows must equal C rows. MMul attempt: " & shapeText(a) & "x" & shapeText(b) & "; result array provided: " & shapeText(c))
            End If

            If a.columns() > Integer.MaxValue OrElse a.rows() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If b.columns() > Integer.MaxValue OrElse b.rows() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If c.columns() > Integer.MaxValue OrElse c.rows() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If Nd4vbT.allowsSpecifyOrdering() Then
                If a.ordering() = b.ordering() Then
                    'both will be same ordering for cblas
                    Me.ordering = a.ordering()
                    'automatically assume fortran ordering
                    'multiple backends force us to be
                    'in fortran ordering only
                    Me.a = copyIfNeccessary(a)
                    Me.b = copyIfNeccessary(b)
                    Me.c = c
                    If ordering = "c"c Then
                        Me.m = CInt(c.columns())
                        Me.n = CInt(c.rows())
                        Me.k = CInt(a.columns())
                    Else
                        Me.m = CInt(c.rows())
                        Me.n = CInt(c.columns())
                        Me.k = CInt(b.columns())
                    End If

                    Me.lda = CInt(a.rows())
                    Me.ldb = CInt(b.rows())
                    Me.ldc = CInt(c.rows())

                    Me.transA = "N"c
                    Me.transB = "N"c
                Else
                    'automatically assume fortran ordering
                    'multiple backends force us to be
                    'in fortran ordering only
                    Me.a = copyIfNeccessary(a)
                    Me.b = b.dup(a.ordering())
                    Me.c = c

                    Me.m = CInt(c.rows())
                    Me.n = CInt(c.columns())
                    Me.k = CInt(a.columns())

                    Me.ordering = a.ordering()

                    Me.lda = CInt(a.rows())
                    Me.ldb = CInt(b.rows())
                    Me.ldc = CInt(c.rows())

                    Me.transA = "N"c
                    Me.transB = "N"c
                End If

            Else
                'automatically assume fortran ordering
                'multiple backends force us to be
                'in fortran ordering only
                Me.a = copyIfNeccessary(a)
                Me.b = copyIfNeccessary(b)
                Me.c = c

                Me.m = CInt(c.rows())
                Me.n = CInt(c.columns())
                Me.k = CInt(a.columns())

                'always fortran ordering
                Me.lda = CInt(If(Me.a.ordering() = "f"c, Me.a.rows(), Me.a.columns())) 'Leading dimension of a, as declared. But swap if 'c' order
                Me.ldb = CInt(If(Me.b.ordering() = "f"c, Me.b.rows(), Me.b.columns())) 'Leading dimension of b, as declared. But swap if 'c' order
                Me.ldc = CInt(c.rows())

                Me.transA = (If(Me.a.ordering() = "c"c, "T"c, "N"c))
                Me.transB = (If(Me.b.ordering() = "c"c, "T"c, "N"c))

            End If

            '/validate();
        End Sub

        ''' <summary>The parameters of C = op(A) * op(B), op(X) being X transposed when the flag is set.</summary>
        Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
            Me.New(If(transposeA, a.transpose(), a), If(transposeB, b.transpose(), b), c)
        End Sub

        Private Shared Function shapeText(ByVal arr As INDArray) As String
            Return "[" & String.Join(", ", arr.shape()) & "]"
        End Function

        Private Function copyIfNeccessary(ByVal arr As INDArray) As INDArray
            'See also: Shape.toMmulCompatible - want same conditions here and there
            'Check if matrix values are contiguous in memory. If not: dup
            'Contiguous for c if: stride[0] == shape[1] and stride[1] = 1
            'Contiguous for f if: stride[0] == 1 and stride[1] == shape[0]
            If Not Nd4vbT.allowsSpecifyOrdering() AndAlso arr.ordering() = "c"c AndAlso (arr.stride(0) <> arr.size(1) OrElse arr.stride(1) <> 1) Then
                Return arr.dup()
            ElseIf arr.ordering() = "f"c AndAlso (arr.stride(0) <> 1 OrElse arr.stride(1) <> arr.size(0)) Then
                Return arr.dup()
            ElseIf arr.elementWiseStride() < 0 Then
                Return arr.dup()
            End If
            Return arr
        End Function

        Public Overridable Function getLda() As Integer
            Return lda
        End Function

        Public Overridable Sub setLda(ByVal lda As Integer)
            Me.lda = lda
        End Sub

        Public Overridable Function getLdb() As Integer
            Return ldb
        End Function

        Public Overridable Sub setLdb(ByVal ldb As Integer)
            Me.ldb = ldb
        End Sub

        Public Overridable Function getLdc() As Integer
            Return ldc
        End Function

        Public Overridable Sub setLdc(ByVal ldc As Integer)
            Me.ldc = ldc
        End Sub

        Public Overridable Function getM() As Integer
            Return m
        End Function

        Public Overridable Sub setM(ByVal m As Integer)
            Me.m = m
        End Sub

        Public Overridable Function getN() As Integer
            Return n
        End Function

        Public Overridable Sub setN(ByVal n As Integer)
            Me.n = n
        End Sub

        Public Overridable Function getK() As Integer
            Return k
        End Function

        Public Overridable Sub setK(ByVal k As Integer)
            Me.k = k
        End Sub

        Public Overridable Function getA() As INDArray
            Return a
        End Function

        Public Overridable Sub setA(ByVal a As INDArray)
            Me.a = a
        End Sub

        Public Overridable Function getB() As INDArray
            Return b
        End Function

        Public Overridable Sub setB(ByVal b As INDArray)
            Me.b = b
        End Sub

        Public Overridable Function getC() As INDArray
            Return c
        End Function

        Public Overridable Sub setC(ByVal c As INDArray)
            Me.c = c
        End Sub

        Public Overridable Function getTransA() As Char
            Return transA
        End Function

        Public Overridable Sub setTransA(ByVal transA As Char)
            Me.transA = transA
        End Sub

        Public Overridable Function getTransB() As Char
            Return transB
        End Function

        Public Overridable Sub setTransB(ByVal transB As Char)
            Me.transB = transB
        End Sub

        Public Overridable Function getOrdering() As Char
            Return ordering
        End Function

        Public Overridable Sub setOrdering(ByVal ordering As Char)
            Me.ordering = ordering
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then
                Return True
            End If
            Dim other As GemmParams = TryCast(obj, GemmParams)
            If other Is Nothing Then
                Return False
            End If
            Return lda = other.lda AndAlso ldb = other.ldb AndAlso ldc = other.ldc AndAlso m = other.m AndAlso n = other.n AndAlso k = other.k AndAlso
                Object.Equals(a, other.a) AndAlso Object.Equals(b, other.b) AndAlso Object.Equals(c, other.c) AndAlso
                transA = other.transA AndAlso transB = other.transB AndAlso ordering = other.ordering
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + lda
            result = result * 59 + ldb
            result = result * 59 + ldc
            result = result * 59 + m
            result = result * 59 + n
            result = result * 59 + k
            result = result * 59 + If(a Is Nothing, 43, a.GetHashCode())
            result = result * 59 + If(b Is Nothing, 43, b.GetHashCode())
            result = result * 59 + If(c Is Nothing, 43, c.GetHashCode())
            result = result * 59 + AscW(transA)
            result = result * 59 + AscW(transB)
            result = result * 59 + AscW(ordering)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "GemmParams(lda=" & lda & ", ldb=" & ldb & ", ldc=" & ldc & ", m=" & m & ", n=" & n & ", k=" & k &
                ", a=" & If(a Is Nothing, "null", CObj(a).ToString()) & ", b=" & If(b Is Nothing, "null", CObj(b).ToString()) &
                ", c=" & If(c Is Nothing, "null", CObj(c).ToString()) & ", transA=" & transA & ", transB=" & transB &
                ", ordering=" & ordering & ")"
        End Function
    End Class

End Namespace
