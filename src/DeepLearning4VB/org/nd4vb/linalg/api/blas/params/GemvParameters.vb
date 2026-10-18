Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBArraySizeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBArraySizeException
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
    ''' The BLAS gemv parameters (sizes, leading dimension, increments, operands and the transpose flag of A) computed
    ''' from the operands (Java <c>GemvParameters</c>). Non-contiguous matrices and offset vectors are copied first.
    ''' Lombok's <c>@Data</c> members are written out.
    ''' </summary>
    Public Class GemvParameters

        Private m, n, lda, incx, incy As Integer
        Private a, x, y As INDArray
        Private aOrdering As Char = "N"c

        Public Sub New(ByVal a As INDArray, ByVal x As INDArray, ByVal y As INDArray)
            a = copyIfNecessary(a)
            x = copyIfNecessaryVector(x)
            Me.a = a
            Me.x = x
            Me.y = y

            If a.columns() > Integer.MaxValue OrElse a.rows() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If x.columns() > Integer.MaxValue OrElse x.rows() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If a.ordering() = "f"c AndAlso a.Matrix Then
                Me.m = a.rows()
                Me.n = a.columns()
                Me.lda = a.rows()
            ElseIf a.ordering() = "c"c AndAlso a.Matrix Then
                Me.m = a.columns()
                Me.n = a.rows()
                Me.lda = a.columns()
                aOrdering = "T"c
            Else
                Me.m = a.rows()
                Me.n = a.columns()
                Me.lda = CInt(a.size(0))
            End If

            If x.rank() = 1 Then
                incx = 1
            ElseIf x.ColumnVector Then
                incx = x.stride(0)
            Else
                incx = x.stride(1)
            End If

            Me.incy = y.elementWiseStride()
        End Sub

        Private Function copyIfNecessary(ByVal arr As INDArray) As INDArray
            'See also: Shape.toMmulCompatible - want same conditions here and there
            'Check if matrix values are contiguous in memory. If not: dup
            'Contiguous for c if: stride[0] == shape[1] and stride[1] = 1
            'Contiguous for f if: stride[0] == 1 and stride[1] == shape[0]
            If arr.ordering() = "c"c AndAlso (arr.stride(0) <> arr.size(1) OrElse arr.stride(1) <> 1) Then
                Return arr.dup()
            ElseIf arr.ordering() = "f"c AndAlso (arr.stride(0) <> 1 OrElse arr.stride(1) <> arr.size(0)) Then
                Return arr.dup()
            ElseIf arr.elementWiseStride() < 1 Then
                Return arr.dup()
            End If
            Return arr
        End Function

        Private Function copyIfNecessaryVector(ByVal vec As INDArray) As INDArray
            If vec.offset() <> 0 Then
                Return vec.dup()
            End If
            Return vec
        End Function

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

        Public Overridable Function getLda() As Integer
            Return lda
        End Function

        Public Overridable Sub setLda(ByVal lda As Integer)
            Me.lda = lda
        End Sub

        Public Overridable Function getIncx() As Integer
            Return incx
        End Function

        Public Overridable Sub setIncx(ByVal incx As Integer)
            Me.incx = incx
        End Sub

        Public Overridable Function getIncy() As Integer
            Return incy
        End Function

        Public Overridable Sub setIncy(ByVal incy As Integer)
            Me.incy = incy
        End Sub

        Public Overridable Function getA() As INDArray
            Return a
        End Function

        Public Overridable Sub setA(ByVal a As INDArray)
            Me.a = a
        End Sub

        Public Overridable Function getX() As INDArray
            Return x
        End Function

        Public Overridable Sub setX(ByVal x As INDArray)
            Me.x = x
        End Sub

        Public Overridable Function getY() As INDArray
            Return y
        End Function

        Public Overridable Sub setY(ByVal y As INDArray)
            Me.y = y
        End Sub

        Public Overridable Function getAOrdering() As Char
            Return aOrdering
        End Function

        Public Overridable Sub setAOrdering(ByVal aOrdering As Char)
            Me.aOrdering = aOrdering
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As GemvParameters = TryCast(obj, GemvParameters)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return m.Equals(o.m) AndAlso n.Equals(o.n) AndAlso lda.Equals(o.lda) AndAlso incx.Equals(o.incx) AndAlso incy.Equals(o.incy) AndAlso Object.Equals(a, o.a) AndAlso Object.Equals(x, o.x) AndAlso Object.Equals(y, o.y) AndAlso aOrdering.Equals(o.aOrdering)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + m.GetHashCode()
            h = h * 59 + n.GetHashCode()
            h = h * 59 + lda.GetHashCode()
            h = h * 59 + incx.GetHashCode()
            h = h * 59 + incy.GetHashCode()
            h = h * 59 + If(a Is Nothing, 43, a.GetHashCode())
            h = h * 59 + If(x Is Nothing, 43, x.GetHashCode())
            h = h * 59 + If(y Is Nothing, 43, y.GetHashCode())
            h = h * 59 + aOrdering.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "GemvParameters(" & "m=" & m.ToString() & ", n=" & n.ToString() & ", lda=" & lda.ToString() & ", incx=" & incx.ToString() & ", incy=" & incy.ToString() & ", a=" & Objects.toString(a) & ", x=" & Objects.toString(x) & ", y=" & Objects.toString(y) & ", aOrdering=" & aOrdering.ToString() & ")"
        End Function

    End Class

End Namespace
