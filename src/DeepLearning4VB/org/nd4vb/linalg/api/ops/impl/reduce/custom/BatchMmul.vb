Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.reduce.custom

    ''' <summary>The custom op <c>batched_gemm</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.custom.BatchMmul</c>).</summary>
    Public Class BatchMmul
        Inherits DynamicCustomOp

        Protected Friend transposeA As Integer
        Protected Friend transposeB As Integer

        Protected Friend batchSize As Integer

        Protected Friend M As Integer
        Protected Friend N As Integer
        Protected Friend K As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal matricesA() As SDVariable, ByVal matricesB() As SDVariable, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
			Me.New(sameDiff, ArrayUtils.addAll(matricesA, matricesB), transposeA, transposeB)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrices() As SDVariable, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
			MyBase.New(Nothing, sameDiff, ArrayUtils.addAll(New SDVariable(){ sameDiff.var(Nd4j.ones(matrices(0).dataType(), matrices.Length \ 2)), sameDiff.var(Nd4j.zeros(matrices(1).dataType(), matrices.Length \ 2))}, matrices))

			Preconditions.checkState(matrices.Length Mod 2 = 0, "The number of provided matrices needs" & "to be divisible by two.")
			Me.batchSize = matrices.Length \ 2

			Dim firstMatrix As SDVariable = matrices(0)
			Dim firstShape() As Long = firstMatrix.Shape
			For i As Integer = 0 To batchSize - 1
				Preconditions.checkState(firstShape.SequenceEqual(matrices(i).Shape))
			Next i
			Dim lastMatrix As SDVariable = matrices(2 * batchSize - 1)
			Dim lastShape() As Long = lastMatrix.Shape
			Dim i As Integer = batchSize
			Do While i < 2 * batchSize
				Preconditions.checkState(lastShape.SequenceEqual(matrices(i).Shape))
				i += 1
			Loop

			Me.transposeA = If(transposeA, 1, 0)
			Me.transposeB = If(transposeB, 1, 0)

			Me.M = If(transposeA, CInt(firstShape(1)), CInt(firstShape(0)))
			Me.N = If(transposeA, CInt(firstShape(0)), CInt(firstShape(1)))
			Me.K = If(transposeB, CInt(lastShape(0)), CInt(lastShape(1)))

			addArgs()
		End Sub
#End If

        Public Sub New(ByVal matricesA() As INDArray, ByVal matricesB() As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
            MyBase.New(matricesA.Concat(matricesB).ToArray(), Nothing)
            Me.batchSize = matricesA.Length

            Me.transposeA = If(transposeA, 1, 0)
            Me.transposeB = If(transposeB, 1, 0)

            Dim firstShape() As Long = matricesA(0).shape()
            Dim lastShape() As Long = matricesB(0).shape()

            Me.M = If(transposeA, CInt(firstShape(1)), CInt(firstShape(0)))
            Me.N = If(transposeA, CInt(firstShape(0)), CInt(firstShape(1)))
            Me.K = If(transposeB, CInt(lastShape(0)), CInt(lastShape(1)))
            addArgs()
        End Sub

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return batchSize
            End Get
        End Property

        Public Overridable Sub addArgs()
            addIArgument(transposeA, transposeB, M, K, N, M, K, N, batchSize)
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "batched_gemm"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim dLdOut() As SDVariable = CType(grads, List(Of SDVariable)).ToArray()

			Dim allArgs() As SDVariable = args()
			Dim matricesA() As SDVariable = Arrays.CopyOfRange(allArgs,0, batchSize)
			Dim matricesB() As SDVariable = Arrays.CopyOfRange(allArgs, batchSize, 2 * batchSize)

			Dim dLdx() As SDVariable = sameDiff.batchMmul(dLdOut, matricesB, False, transposeB = 1)
			Dim dLdy() As SDVariable = sameDiff.batchMmul(matricesA, dLdOut, transposeA = 1, False)

			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Collections.addAll(ret, dLdx)
			Collections.addAll(ret, dLdy)
			Return ret
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Dim [out] As IList(Of DataType) = New List(Of DataType)()
            For i As Integer = 0 To dataTypes.Count - 3 '-2 for the alpha and beta params
                Preconditions.checkState(dataTypes(i).isFPType(), "Inputs to batch mmul op must all be a floating point type: got %s", dataTypes)
                If i Mod 2 = 0 Then
                    [out].Add(dataTypes(i))
                End If
            Next i
            Return [out]
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As BatchMmul = TryCast(o, BatchMmul)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(transposeA, other.transposeA) Then
                Return False
            End If
            If Not Object.Equals(transposeB, other.transposeB) Then
                Return False
            End If
            If Not Object.Equals(batchSize, other.batchSize) Then
                Return False
            End If
            If Not Object.Equals(M, other.M) Then
                Return False
            End If
            If Not Object.Equals(N, other.N) Then
                Return False
            End If
            If Not Object.Equals(K, other.K) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(transposeA) Is Nothing, 43, transposeA.GetHashCode())
            result = result * 59 + If(CObj(transposeB) Is Nothing, 43, transposeB.GetHashCode())
            result = result * 59 + If(CObj(batchSize) Is Nothing, 43, batchSize.GetHashCode())
            result = result * 59 + If(CObj(M) Is Nothing, 43, M.GetHashCode())
            result = result * 59 + If(CObj(N) Is Nothing, 43, N.GetHashCode())
            result = result * 59 + If(CObj(K) Is Nothing, 43, K.GetHashCode())
            Return result
        End Function

    End Class

End Namespace
