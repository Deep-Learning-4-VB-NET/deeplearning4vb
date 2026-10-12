Imports System.Collections.Generic
Imports System.Linq
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class BatchMmul extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class BatchMmul
		Inherits DynamicCustomOp

		Protected Friend transposeA As Integer
		Protected Friend transposeB As Integer

		Protected Friend batchSize As Integer

		Protected Friend M As Integer
		Protected Friend N As Integer
		Protected Friend K As Integer

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

		Public Sub New(ByVal matricesA() As INDArray, ByVal matricesB() As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
			MyBase.New(ArrayUtils.addAll(matricesA, matricesB), Nothing)
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
	End Class


End Namespace