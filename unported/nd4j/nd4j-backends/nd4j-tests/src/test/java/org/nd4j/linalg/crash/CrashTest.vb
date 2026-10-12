Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports LogSoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports Sqrt = org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.nd4j.linalg.crash

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class CrashTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CrashTest
		Inherits BaseNd4jTestWithBackends

		Private Const ITERATIONS As Integer = 10
		Private Shared ReadOnly paramsA() As Boolean = {True, False}
		Private Shared ReadOnly paramsB() As Boolean = {True, False}


		''' <summary>
		''' tensorAlongDimension() produces shapeInfo without EWS defined
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonEWSViews1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonEWSViews1(ByVal backend As Nd4jBackend)
			log.debug("non-EWS 1")
			Dim x As INDArray = Nd4j.create(64, 1024, 64)
			Dim y As INDArray = Nd4j.create(64, 64, 1024)

			For i As Integer = 0 To ITERATIONS - 1
				Dim slice As Integer = RandomUtils.nextInt(0, CInt(x.size(0)))
				op(x.tensorAlongDimension(slice, 1, 2), y.tensorAlongDimension(slice, 1, 2), i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNonEWSViews2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNonEWSViews2(ByVal backend As Nd4jBackend)
			log.debug("non-EWS 2")
			Dim x As INDArray = Nd4j.create(New Integer() {64, 1024, 64}, "f"c)
			Dim y As INDArray = Nd4j.create(New Integer() {64, 64, 1024}, "f"c)

			For i As Integer = 0 To ITERATIONS - 1
				Dim slice As Integer = RandomUtils.nextInt(0, CInt(x.size(0)))
				op(x.tensorAlongDimension(slice, 1, 2), y.tensorAlongDimension(slice, 1, 2), i)
			Next i
		End Sub

		''' <summary>
		''' slice() produces shapeInfo with EWS being 1 in our case
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEWSViews1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEWSViews1(ByVal backend As Nd4jBackend)
			log.debug("EWS 1")
			Dim x As INDArray = Nd4j.create(64, 1024, 64)
			Dim y As INDArray = Nd4j.create(64, 64, 1024)

			For i As Integer = 0 To ITERATIONS - 1
				Dim slice As Long = RandomUtils.nextLong(0, x.shape()(0))
				op(x.slice(slice), y.slice(slice), i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEWSViews2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEWSViews2(ByVal backend As Nd4jBackend)
			log.debug("EWS 2")
			Dim x As INDArray = Nd4j.create(New Integer() {96, 1024, 64}, "f"c)
			Dim y As INDArray = Nd4j.create(New Integer() {96, 64, 1024}, "f"c)

			For i As Integer = 0 To 0
				Dim slice As Integer = 0 'RandomUtils.nextInt(0, x.shape()[0]);
				op(x.slice(slice), y.slice(slice), i)
			Next i
		End Sub

		Protected Friend Overridable Sub op(ByVal x As INDArray, ByVal y As INDArray, ByVal i As Integer)
			' broadcast along row & column
			Dim row As INDArray = Nd4j.ones(64)
			Dim column As INDArray = Nd4j.ones(1024, 1)

			x.addiRowVector(row)
			x.addiColumnVector(column)

			' casual scalar
			x.addi(i * 2)

			' reduction along all dimensions
			Dim sum As Single = x.sumNumber().floatValue()

			' index reduction
			Nd4j.Executioner.exec(New ArgMax(x))

			' casual transform
			Nd4j.Executioner.exec(New Sqrt(x, x))

			'  dup
			Dim x1 As INDArray = x.dup(x.ordering())
			Dim x2 As INDArray = x.dup(x.ordering())
			Dim x3 As INDArray = x.dup("c"c)
			Dim x4 As INDArray = x.dup("f"c)


			' vstack && hstack
			Dim vstack As INDArray = Nd4j.vstack(x, x1, x2, x3, x4)

			Dim hstack As INDArray = Nd4j.hstack(x, x1, x2, x3, x4)

			' reduce3 call
			Nd4j.Executioner.exec(New ManhattanDistance(x, x2))


			' flatten call
			Dim flat As INDArray = Nd4j.toFlattened(x, x1, x2, x3, x4)


			' reduction along dimension: row & column
			Dim max_0 As INDArray = x.max(0)
			Dim max_1 As INDArray = x.max(1)


			' index reduction along dimension: row & column
			Dim imax_0 As INDArray = Nd4j.argMax(x, 0)
			Dim imax_1 As INDArray = Nd4j.argMax(x, 1)


			' logisoftmax, softmax & softmax derivative
			Nd4j.Executioner.exec(DirectCast(New SoftMax(x), CustomOp))
			Nd4j.Executioner.exec(DirectCast(New LogSoftMax(x), CustomOp))


			' BooleanIndexing
			BooleanIndexing.replaceWhere(x, 5f, Conditions.lessThan(8f))

			' assing on view
			BooleanIndexing.assignIf(x, x1, Conditions.greaterThan(-1000000000f))

			' std var along all dimensions
			Dim std As Single = x.stdNumber().floatValue()

			' std var along row & col
			Dim xStd_0 As INDArray = x.std(0)
			Dim xStd_1 As INDArray = x.std(1)

			' blas call
			Dim dot As Single = CSng(Nd4j.BlasWrapper.dot(x, x1))

			' mmul
			For Each tA As Boolean In paramsA
				For Each tB As Boolean In paramsB

					Dim xT As INDArray = If(tA, x.dup(), x.dup().transpose())
					Dim yT As INDArray = If(tB, y.dup(), y.dup().transpose())

					Nd4j.gemm(xT, yT, tA, tB)
				Next tB
			Next tA

			' specially for views, checking here without dup and rollover
			Nd4j.gemm(x, y, False, False)

			log.debug("Iteration passed: " & i)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace