Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ArgMin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin
Imports Mean = org.nd4j.linalg.api.ops.impl.reduce.floating.Mean
Imports Norm2 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2
Imports NormMax = org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax
Imports Max = org.nd4j.linalg.api.ops.impl.reduce.same.Max
Imports Min = org.nd4j.linalg.api.ops.impl.reduce.same.Min
Imports Prod = org.nd4j.linalg.api.ops.impl.reduce.same.Prod
Imports Sum = org.nd4j.linalg.api.ops.impl.reduce.same.Sum
Imports EuclideanDistance = org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports Pow = org.nd4j.linalg.api.ops.impl.scalar.Pow
Imports ScalarAdd = org.nd4j.linalg.api.ops.impl.scalar.ScalarAdd
Imports ScalarMax = org.nd4j.linalg.api.ops.impl.scalar.ScalarMax
Imports ScalarReverseSubtraction = org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseSubtraction
Imports ScalarGreaterThan = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarGreaterThan
Imports ScalarLessThan = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarLessThan
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports Histogram = org.nd4j.linalg.api.ops.impl.transforms.Histogram
Imports LogSoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports Exp = org.nd4j.linalg.api.ops.impl.transforms.strict.Exp
Imports Log = org.nd4j.linalg.api.ops.impl.transforms.strict.Log
Imports SetRange = org.nd4j.linalg.api.ops.impl.transforms.strict.SetRange
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.nd4j.linalg.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class OpExecutionerTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class OpExecutionerTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxReference(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxReference(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.linspace(1,4,4, DataType.FLOAT).reshape(ChrW(2), 2)
			Dim dup As INDArray = input.dup()
			Nd4j.Executioner.exec(New SoftMax(dup))
			Dim result As INDArray = Nd4j.zeros(DataType.FLOAT, 2,2)
			Nd4j.Executioner.exec(New SoftMax(input,result))
			assertEquals(dup,result)


			dup = input.dup()
			Nd4j.Executioner.exec(New LogSoftMax(dup))

			result = Nd4j.zeros(DataType.FLOAT,2,2)
			Nd4j.Executioner.exec(New LogSoftMax(input,result))

			assertEquals(dup,result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarReverseSub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarReverseSub(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.valueArrayOf(4,2.0)
			Dim result As INDArray= Nd4j.zeros(4)
			Nd4j.Executioner.exec(New ScalarReverseSubtraction(input,Nothing,result,1.0))
			Dim assertion As INDArray = Nd4j.valueArrayOf(4,-1.0)
			assertEquals(assertion,result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastMultiDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastMultiDim(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape(ChrW(2), 3, 5)
	'        System.out.println(data);
			Dim mask As INDArray = Nd4j.create(New Double()() {
				New Double() {1.00, 1.00, 1.00, 1.00, 1.00},
				New Double() {1.00, 1.00, 1.00, 0.00, 0.00}
			})
			Nd4j.Executioner.exec(New BroadcastMulOp(data, mask, data, 0, 2))
			Dim assertion As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 0.0, 0.0, 21.0, 22.0, 23.0, 0.0, 0.0, 26.0, 27.0, 28.0, 0.0, 0.0}).reshape(ChrW(2), 3, 5)
			assertEquals(assertion, data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineSimilarity(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCosineSimilarity(ByVal backend As Nd4jBackend)
			Dim vec1 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4, 5})
			Dim vec2 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4, 5})
			Dim sim As Double = Transforms.cosineSim(vec1, vec2)
			assertEquals(1, sim, 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCosineDistance(ByVal backend As Nd4jBackend)
			Dim vec1 As INDArray = Nd4j.create(New Single() {1, 2, 3})
			Dim vec2 As INDArray = Nd4j.create(New Single() {3, 5, 7})
			' 1-17*sqrt(2/581)
			Dim distance As Double = Transforms.cosineDistance(vec1, vec2)
			assertEquals(0.0025851, distance, 1e-7,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLog(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLog(ByVal backend As Nd4jBackend)
			Dim log As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim transformed As INDArray = Transforms.log(log)
			Dim assertion As INDArray = Nd4j.create(New Double() {0.0, 0.69314718, 1.09861229, 1.38629436, 1.60943791, 1.79175947})
			assertEquals(assertion, transformed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm1AlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm1AlongDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 4)
			Dim arrNorm1 As INDArray = arr.norm2(1)
			Dim assertion As INDArray = Nd4j.create(New Double() {5.47722558, 13.19090596})
			assertEquals(assertion, arrNorm1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEuclideanDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEuclideanDistance(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {55, 55})
			Dim arr2 As INDArray = Nd4j.create(New Double() {60, 60})
			Dim result As Double = Nd4j.Executioner.execAndReturn(New EuclideanDistance(arr, arr2)).getFinalResult().doubleValue()
			assertEquals(7.0710678118654755, result, 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMaxOp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMaxOp(ByVal backend As Nd4jBackend)
			Dim scalarMax As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).negi()
			Dim postMax As INDArray = Nd4j.ones(DataType.DOUBLE, 6)
			Nd4j.Executioner.exec(New ScalarMax(scalarMax, 1))
			assertEquals(postMax, scalarMax,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSetRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSetRange(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Nd4j.Executioner.exec(New SetRange(linspace, 0, 1))
			For i As Integer = 0 To linspace.length() - 1
				Dim val As Double = linspace.getDouble(i)
				assertTrue(val >= 0 AndAlso val <= 1,getFailureMessage(backend))
			Next i

			Dim linspace2 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Nd4j.Executioner.exec(New SetRange(linspace2, 2, 4))
			For i As Integer = 0 To linspace2.length() - 1
				Dim val As Double = linspace2.getDouble(i)
				assertTrue(val >= 2 AndAlso val <= 4,getFailureMessage(backend))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormMax(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim normMax As Double = Nd4j.Executioner.execAndReturn(New NormMax(arr)).getFinalResult().doubleValue()
			assertEquals(4, normMax, 1e-1,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim norm2 As Double = Nd4j.Executioner.execAndReturn(New Norm2(arr)).getFinalResult().doubleValue()
			assertEquals(5.4772255750516612, norm2, 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdd(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.ones(5)
			Dim xDup As INDArray = x.dup()
			Dim solution As INDArray = Nd4j.valueArrayOf(5, 2.0)
			opExecutioner.exec(New AddOp(New INDArray(){x, xDup},New INDArray(){x}))
			assertEquals(solution, x,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMul(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.ones(5)
			Dim xDup As INDArray = x.dup()
			Dim solution As INDArray = Nd4j.valueArrayOf(5, 1.0)
			opExecutioner.exec(New MulOp(x, xDup, x))
			assertEquals(solution, x)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecutioner(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecutioner(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.ones(5)
			Dim xDup As INDArray = x.dup()
			Dim solution As INDArray = Nd4j.valueArrayOf(5, 2.0)
			opExecutioner.exec(New AddOp(New INDArray(){x, xDup},New INDArray(){ x}))
			assertEquals(solution, x,getFailureMessage(backend))
			Dim acc As New Sum(x.dup())
			opExecutioner.exec(acc)
			assertEquals(10.0, acc.FinalResult.doubleValue(), 1e-1,getFailureMessage(backend))
			Dim prod As New Prod(x.dup())
			opExecutioner.exec(prod)
			assertEquals(32.0, prod.FinalResult.doubleValue(), 1e-1,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxMin(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			Dim max As New Max(x)
			opExecutioner.exec(max)
			assertEquals(5, max.FinalResult.doubleValue(), 1e-1)
			Dim min As New Min(x)
			opExecutioner.exec(min)
			assertEquals(1, min.FinalResult.doubleValue(), 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testProd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testProd(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim prod As New Prod(linspace)
			Dim prod2 As Double = Nd4j.Executioner.execAndReturn(prod).getFinalResult().doubleValue()
			assertEquals(720, prod2, 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim sum As New Sum(linspace)
			Dim sum2 As Double = Nd4j.Executioner.execAndReturn(sum).getFinalResult().doubleValue()
			assertEquals(21, sum2, 1e-1)

			Dim matrixSums As INDArray = linspace.reshape(ChrW(2), 3)
			Dim rowSums As INDArray = matrixSums.sum(1)
			assertEquals(Nd4j.create(New Double() {6, 15}), rowSums)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDescriptiveStatsDouble(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDescriptiveStatsDouble(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)

			Dim mean As New Mean(x)
			opExecutioner.exec(mean)
			assertEquals(3.0, mean.FinalResult.doubleValue(), 1e-1)

			Dim variance As New Variance(x.dup(), True)
			opExecutioner.exec(variance)
			assertEquals(2.5, variance.FinalResult.doubleValue(), 1e-1,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDescriptiveStats(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDescriptiveStats(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim x As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)

			Dim mean As New Mean(x)
			opExecutioner.exec(mean)
			assertEquals(3.0, mean.FinalResult.doubleValue(), 1e-1,getFailureMessage(backend))

			Dim variance As New Variance(x.dup(), True)
			opExecutioner.exec(variance)
			assertEquals(2.5, variance.FinalResult.doubleValue(), 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowSoftmax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowSoftmax(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim softMax As val = New SoftMax(arr)
			opExecutioner.exec(DirectCast(softMax, CustomOp))
			assertEquals(1.0, softMax.outputArguments().get(0).sumNumber().doubleValue(), 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddiRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddiRowVector(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim arr2 As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 4, 6, 5, 7, 9}).reshape(ChrW(2), 3)
			Dim test As INDArray = arr.addRowVector(arr2)
			assertEquals(assertion, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTad(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(2), 3, 2)
			Dim i As Integer = 0
			Do While i < arr.tensorsAlongDimension(0)
	'            System.out.println(arr.tensorAlongDimension(i, 0));
				arr.tensorAlongDimension(i, 0)
				i += 1
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPow(ByVal backend As Nd4jBackend)
			Dim oneThroughSix As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim pow As New Pow(oneThroughSix, 2)
			Nd4j.Executioner.exec(pow)
			Dim answer As INDArray = Nd4j.create(New Double() {1, 4, 9, 16, 25, 36})
			assertEquals(answer, pow.z(),getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testComparisonOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testComparisonOps(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim ones As INDArray = Nd4j.ones(DataType.BOOL, 1,6)
			Dim zeros As INDArray = Nd4j.create(DataType.BOOL, 1,6)
			Dim res As INDArray = Nd4j.create(DataType.BOOL, 1,6)
			assertEquals(ones, Nd4j.Executioner.exec(New ScalarGreaterThan(linspace, res, 0)))
			assertEquals(zeros, Nd4j.Executioner.exec(New ScalarGreaterThan(linspace, res,7)))
			assertEquals(zeros, Nd4j.Executioner.exec(New ScalarLessThan(linspace, res,0)))
			assertEquals(ones, Nd4j.Executioner.exec(New ScalarLessThan(linspace, res,7)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarArithmetic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarArithmetic(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim plusOne As INDArray = Nd4j.linspace(2, 7, 6, DataType.DOUBLE)
			Nd4j.Executioner.exec(New ScalarAdd(linspace, 1))
			assertEquals(plusOne, linspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimensionMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimensionMax(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim axis As Integer = 0
			Dim row As INDArray = linspace.slice(axis)
			Dim max As New Max(row)
			Dim max2 As Double = Nd4j.Executioner.execAndReturn(max).getFinalResult().doubleValue()
			assertEquals(3.0, max2, 1e-1)

			Dim min As New Min(row)
			Dim min2 As Double = Nd4j.Executioner.execAndReturn(min).getFinalResult().doubleValue()
			assertEquals(1.0, min2, 1e-1)
			Dim matrixMax As New Max(linspace, 1)
			Dim exec2 As INDArray = Nd4j.Executioner.execAndReturn(matrixMax).z()
			assertEquals(Nd4j.create(New Double() {3, 6}), exec2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedLog(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedLog(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim slice As INDArray = arr.slice(0)
			Dim exp As New Log(slice)
			opExecutioner.exec(exp)
			Dim assertion As INDArray = Nd4j.create(New Double() {0.0, 0.6931471824645996, 1.0986123085021973})
			assertEquals(assertion, slice,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedExp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedExp(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim slice As INDArray = arr.slice(0)
			Dim expected As val = New Double(CInt(slice.length()) - 1){}
			For i As Integer = 0 To slice.length() - 1
				expected(i) = CSng(Math.Exp(slice.getDouble(i)))
			Next i
			Dim exp As New Exp(slice)
			opExecutioner.exec(exp)
			assertEquals(Nd4j.create(expected), slice,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftMax(ByVal backend As Nd4jBackend)
			Dim opExecutioner As OpExecutioner = Nd4j.Executioner
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim softMax As val = New SoftMax(arr)
			opExecutioner.exec(softMax)
			assertEquals(1.0, softMax.outputArguments().get(0).sumNumber().doubleValue(), 1e-1,getFailureMessage(backend))

			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
'JAVA TO VB CONVERTER NOTE: The variable softmax was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim softmax_Conflict As val = New SoftMax(linspace.dup())
			Nd4j.Executioner.exec(softmax_Conflict)
			assertEquals(linspace.rows(), softmax_Conflict.outputArguments().get(0).sumNumber().doubleValue(), 1e-1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimensionSoftMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimensionSoftMax(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim max As val = New SoftMax(linspace)
			Nd4j.Executioner.exec(max)
			linspace.assign(max.outputArguments().get(0))
			assertEquals(linspace.getRow(0).sumNumber().doubleValue(), 1.0, 1e-1,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnMean(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim columnMean As INDArray = twoByThree.mean(0)
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 3})
			assertEquals(assertion, columnMean)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnVar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnVar(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 600, 600, DataType.DOUBLE).reshape(ChrW(150), 4)
			Dim columnStd As INDArray = twoByThree.var(0)
			Dim assertion As INDArray = Nd4j.create(New Double() {30200f, 30200f, 30200f, 30200f})
			assertEquals(assertion, columnStd)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnStd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnStd(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 600, 600, DataType.DOUBLE).reshape(ChrW(150), 4)
			Dim columnStd As INDArray = twoByThree.std(0)
			Dim assertion As INDArray = Nd4j.create(New Double() {173.78147196982766f, 173.78147196982766f, 173.78147196982766f, 173.78147196982766f})
			assertEquals(assertion, columnStd)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDim1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDim1(ByVal backend As Nd4jBackend)
			Dim sum As INDArray = Nd4j.linspace(1, 2, 2, DataType.DOUBLE).reshape(ChrW(2), 1)
			Dim same As INDArray = sum.dup()
			assertEquals(same.sum(1), sum.reshape(ChrW(2)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMax(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim imax As New ArgMax(arr)
			assertEquals(9, Nd4j.Executioner.exec(imax)(0).getInt(0))

			arr.muli(-1)
			imax = New ArgMax(arr)
			Dim maxIdx As Integer = Nd4j.Executioner.exec(imax)(0).getInt(0)
			assertEquals(0, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIMin(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE)
			Dim imin As New ArgMin(arr)
			assertEquals(0, Nd4j.Executioner.exec(imin)(0).getInt(0))

			arr.muli(-1)
			imin = New ArgMin(arr)
			Dim minIdx As Integer = Nd4j.Executioner.exec(imin)(0).getInt(0)
			assertEquals(9, minIdx)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanSumSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanSumSimple(ByVal backend As Nd4jBackend)
	'        System.out.println("3d");
			Dim arr As INDArray = Nd4j.ones(1, 4, 4)
			assertEquals(Nd4j.ones(1), arr.mean(1, 2))
			assertEquals(Nd4j.ones(1).muli(16), arr.sum(1, 2))

	'        System.out.println("4d");
			Dim arr4 As INDArray = Nd4j.ones(1, 1, 4, 4)
			Dim arr4m As INDArray = arr4.mean(2, 3)
			Dim arr4s As INDArray = arr4.sum(2, 3)
			For i As Integer = 0 To arr4m.length() - 1
				assertEquals(arr4m.getDouble(i), 1, 1e-1)
			Next i
			For i As Integer = 0 To arr4s.length() - 1
				assertEquals(arr4s.getDouble(i), 16, 1e-1)
			Next i
	'        System.out.println("5d");
			Dim arr5 As INDArray = Nd4j.ones(1, 1, 4, 4, 4)
			Dim arr5s As INDArray = arr5.sum(2, 3)
			For i As Integer = 0 To arr5s.length() - 1
				assertEquals(arr5s.getDouble(i), 16, 1e-1)
			Next i
			Dim arr5m As INDArray = arr5.mean(2, 3)
			For i As Integer = 0 To arr5m.length() - 1
				assertEquals(1, arr5m.getDouble(i), 1e-1)
			Next i

	'        System.out.println("6d");
			Dim arr6 As INDArray = Nd4j.ones(1, 1, 4, 4, 4, 4)
			Dim arr6m As INDArray = arr6.mean(2, 3)
			For i As Integer = 0 To arr6m.length() - 1
				assertEquals(arr6m.getDouble(i), 1, 1e-1)
			Next i

			Dim arr6s As INDArray = arr6.sum(2, 3)

			For i As Integer = 0 To arr6s.length() - 1
				assertEquals(arr6s.getDouble(i), 16, 1e-1)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum6d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum6d(ByVal backend As Nd4jBackend)
			Dim arr6 As INDArray = Nd4j.ones(1, 1, 4, 4, 4, 4)
			Dim arr6s As INDArray = arr6.sum(2, 3)
			For i As Integer = 0 To arr6s.length() - 1
				assertEquals(16, arr6s.getDouble(i), 1e-1)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMean(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {1, 2, 2, 2, 2, 2}
			Dim len As Integer = ArrayUtil.prod(shape)
			Dim val As INDArray = Nd4j.linspace(1, len, len, DataType.DOUBLE).reshape("c"c, shape)
			''' <summary>
			''' Failure comes from the lack of a jump
			''' when doing tad offset in c++
			''' 
			''' We need to jump from the last element rather than the
			''' first for the next element.
			''' 
			''' This happens when the index for a tad is >= the
			''' stride[0]
			''' 
			''' When the index is >= a stride[0] then you take
			''' the offset at the end of the tad and use that +
			''' (possibly the last stride?)
			''' to get to the next offset.
			''' 
			''' In order to get to the last element for a jump, just iterate
			''' over the tad (coordinate wise) to get the coordinate pair +
			''' offset at which to do compute.
			''' 
			''' Another possible solution is to create an initialize pointer
			''' method that will just set up the tad pointer directly.
			''' Right now it is a simplistic base pointer + offset that
			''' we could turn in to an init method instead.
			''' This would allow use to use coordinate based techniques
			''' on the pointer directly. The proposal here
			''' would then be turning tad offset given an index
			''' in to a pointer initialization method which
			''' will auto insert the pointer at the right index.
			''' </summary>
			Dim sum As INDArray = val.sum(2, 3)
			Dim assertionData() As Double = {28.0, 32.0, 36.0, 40.0, 92.0, 96.0, 100.0, 104.0}

			Dim avgExpected As INDArray = Nd4j.create(assertionData).reshape(ChrW(1), 2, 2, 2)

			assertEquals(avgExpected, sum)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum5d() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSum5d()
	'        System.out.println("5d");
			Dim arr5 As INDArray = Nd4j.ones(1, 1, 4, 4, 4)
			Dim arr5s As INDArray = arr5.sum(2, 3)
			Thread.Sleep(1000)
	'        System.out.println("5d length: " + arr5s.length());
			For i As Integer = 0 To arr5s.length() - 1
				assertEquals(16, arr5s.getDouble(i), 1e-1)
			Next i


			Dim arrF As INDArray = Nd4j.ones(1, 1, 4, 4, 4)
	'        System.out.println("A: " + arrF);
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneMinus(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneMinus(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE)
			Dim [out] As INDArray = Transforms.timesOneMinus([in], True)

			'Expect: 0, -2, -6 -> from 1*(1-1), 2*(1-2), 3*(1-3). Getting: [0,0,0]
			Dim exp As INDArray = Nd4j.create(New Double() {0, -2.0, -6.0})
			assertEquals([out], exp)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSubColumnVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSubColumnVector(ByVal backend As Nd4jBackend)
			Dim vec As INDArray = Nd4j.linspace(1, 18, 18, DataType.DOUBLE)
			Dim matrix As INDArray = vec.dup().reshape(ChrW(3), 6)
			Dim vector As INDArray = Nd4j.create(New Double() {6, 12, 18}).reshape(ChrW(3), 1)
			Dim assertion As INDArray = Nd4j.create(New Double() {-5.0, -4.0, -3.0, -2.0, -1.0, 0.0, -5.0, -4.0, -3.0, -2.0, -1.0, 0.0, -5.0, -4.0, -3.0, -2.0, -1.0, 0.0}, New Integer() {3, 6})
			Dim test As INDArray = matrix.subColumnVector(vector)
			assertEquals(assertion, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogSoftmaxVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogSoftmaxVector(ByVal backend As Nd4jBackend)
			Dim temp As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0, 4.0})
			Dim logsoftmax As INDArray = Nd4j.Executioner.exec(New LogSoftMax(temp.dup()))(0)
			Dim assertion As INDArray = Nd4j.create(New Double() {-3.4401898, -2.4401898, -1.4401897, -0.44018975})
			assertEquals(assertion, logsoftmax)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumDifferentOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumDifferentOrder(ByVal backend As Nd4jBackend)
			Dim toAssign As INDArray = Nd4j.linspace(0, 3, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim cOrder As INDArray = Nd4j.create(New Integer() {2, 2}, "c"c).assign(toAssign)
			Dim fOrder As INDArray = Nd4j.create(New Integer() {2, 2}, "f"c).assign(toAssign)

	'        System.out.println(cOrder);
	'        System.out.println(cOrder.sum(0)); //[2,4] -> correct
	'        System.out.println(fOrder.sum(0)); //[2,3] -> incorrect

			assertEquals(cOrder, fOrder)
			assertEquals(cOrder.sum(0), fOrder.sum(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogSoftmax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogSoftmax(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.create(New Double() {-0.115370326, -0.12137828, -0.120233774, -0.12121266, -0.11363905, -0.101017155, -0.11571029, -0.116997495, -0.123033985, -0.1222254, -0.11120513, -0.11710341, -0.12319958, -0.124424405, -0.105285235, -0.08768927, -0.10296882, -0.11346505, -0.10607526, -0.10681274, -0.11604863, -0.1070115, -0.114202365, -0.11168295, -0.11615404, -0.120522454, -0.11282451, -0.11514864, -0.11681116, -0.11987897, -0.12054029, -0.112625614, -0.10337835, -0.098809384, -0.1222254, -0.11966098, -0.11500366, -0.1222254, -0.122691356, -0.1168594, -0.11369472, -0.11666928, -0.12075868, -0.10658686, -0.10251844, -0.119958505, -0.10873747, -0.12036781, -0.11125211, -0.118474, 0.07354958, 0.06268418, 0.08751996, 0.05259535, 0.07969022, 0.062334962, 0.07089297, -0.006484107, 0.0702586, 0.03601057, 0.03228142, 0.051330067, 0.048092633, 0.0753836, 0.0026741663, 0.060346458, 0.064265735, 0.03208362, 0.07322607, 0.034286126, 0.08459597, 0.040570714, 0.08494339, 0.06835921, 0.055334114, 0.06346921, 0.08284429, 0.09769646, 0.07128828, 0.0012985547, 0.033257447, 0.024084045, 0.03130147, 0.09381818, 0.062283173, 0.049273495, 0.0789609, 0.06648661, 0.030163772, 0.047266945, 0.05704684, 0.06862679, 0.04134995, 0.0029913357, 0.050757334, 0.031863946, 0.043180045, 0.053592253, -0.02633951, 0.04229047, 0.12401424, 0.1025523, 0.11914653, 0.10838079, 0.119204566, 0.120582364, 0.079642124, 0.1136303, 0.103594445, 0.12434465, 0.10481718, 0.10615024, 0.1161067, 0.101516, 0.11543929, 0.11498181, 0.1083647, 0.12498043, 0.117732316, 0.080594465, 0.12140614, 0.10168964, 0.11630502, 0.097365364, 0.11659742, 0.11525785, 0.095346555, 0.095523514, 0.1145297, 0.10820676, 0.113681756, 0.12088448, 0.11661095, 0.09196416, 0.09367608, 0.12396194, 0.11715822, 0.10781161, 0.09206241, 0.11529953, 0.12193694, 0.11471913, 0.1025523, 0.12246918, 0.12278436, 0.11647938, 0.09907566, 0.10939402, 0.11121245, 0.09931412, -0.2015398, -0.19392101, -0.19934568, -0.19083071, -0.20022182, -0.18812077, -0.19819336, -0.19751601, -0.18787658, -0.1910854, -0.19982933, -0.19259657, -0.1910668, -0.19623408, -0.20643783, -0.17979786, -0.20085241, -0.20226628, -0.1943775, -0.19513902, -0.1944603, -0.19675966, -0.20814213, -0.19372807, -0.18230462, -0.18796724, -0.19594413, -0.19937015, -0.20221426, -0.1900377, -0.18905015, -0.20246184, -0.18973471, -0.1917036, -0.1910854, -0.2045007, -0.20772256, -0.1910854, -0.19349803, -0.19836159, -0.20438254, -0.16650572, -0.19694945, -0.19511227, -0.18056169, -0.19521528, -0.19218414, -0.19556037, -0.1989097, -0.19989866, 0.110895164, 0.09209204, 0.13636513, 0.09708423, 0.12663901, 0.11280878, 0.10437618, 0.008251642, 0.11656475, 0.062448665, 0.07663319, 0.076713376, 0.09773914, 0.1284772, 0.0019391886, 0.08873351, 0.10645666, 0.06874694, 0.12830636, 0.069761865, 0.12597786, 0.064558044, 0.14945637, 0.12600589, 0.08889626, 0.096229844, 0.13689923, 0.15111938, 0.11476847, 0.012906413, 0.06886689, 0.05653629, 0.056540295, 0.1647724, 0.1054803, 0.06795046, 0.12039944, 0.11954296, 0.052694272, 0.085520394, 0.110611565, 0.11398453, 0.07550961, 0.023511963, 0.090924345, 0.0600122, 0.07526812, 0.088270955, -0.03518031, 0.073293336, 0.17944553, 0.16982275, 0.1886539, 0.18693338, 0.18788463, 0.2058602, 0.13861835, 0.20437749, 0.18895163, 0.16544276, 0.149991, 0.17463979, 0.17583887, 0.16696452, 0.16749835, 0.1592365, 0.17954215, 0.1818188, 0.21207899, 0.15266286, 0.17395115, 0.15906107, 0.21057771, 0.15467106, 0.17414747, 0.19151127, 0.14792846, 0.14762704, 0.1860418, 0.18808068, 0.19654934, 0.17514904, 0.18510495, 0.16045007, 0.18320344, 0.18669076, 0.16069236, 0.17718756, 0.14080223, 0.1681495, 0.17300002, 0.1528326, 0.16982275, 0.1817097, 0.16696694, 0.16177535, 0.1604718, 0.16464049, 0.15210003, 0.16091338, 0.19544502, 0.1334315, 0.16168839, 0.11322618, 0.19517533, 0.18929626, 0.17545204, 0.1665815, 0.09131178, 0.11004268, 0.20550796, 0.13831247, 0.10610545, 0.12289211, 0.27147663, 0.20504008, 0.2518754, 0.20981932, 0.20138234, 0.19962592, 0.15790789, 0.20949593, 0.23528637, 0.18096939, 0.08758456, 0.10911943, 0.18139273, 0.18525626, 0.19391479, 0.11438076, 0.1093913, 0.22006766, 0.18334126, 0.21811387, 0.11004268, 0.19371085, 0.23279056, 0.11004268, 0.11990581, 0.17242423, 0.21975593, 0.046734467, 0.1444371, 0.20759591, 0.13962208, 0.14867997, 0.17288592, 0.14028637, 0.19978605, 0.1737019, -0.038705423, -0.03880039, -0.060744748, 0.005578369, -0.026154364, -0.09166601, -0.061155446, 0.008943805, -0.04777039, -0.012912485, -0.010861377, -0.01913654, -0.0061141956, -0.09119834, 0.034481876, -0.008210908, -0.09062711, -0.0464008, -0.0038113478, -0.006515413, -0.06737334, 0.022068182, -0.078238964, -0.10467487, -0.012385059, -0.008899481, -0.0507185, -0.0612416, -0.05302817, 0.03657996, 0.0040081483, 0.0017336496, 0.00966107, -0.13457696, -0.106228024, -0.05810899, -0.042826205, -0.004804179, -0.054947495, -0.0023088162, -0.083174944, -0.0812491, 0.0012216767, 0.017188948, -0.0416347, -0.0750825, -0.052436177, -0.028371494, 0.07799446, -0.02655019, -0.04801802, -0.11302035, -0.114139326, -0.17401277, -0.11443192, -0.19375448, -0.08697115, -0.22462566, -0.18594599, 0.029962104, -0.03072077, -0.10795037, -0.0687454, -0.08853653, -0.02800453, -0.0044006817, -0.14119355, -0.057319514, -0.23839943, -0.09940908, -0.03132951, -0.07696326, -0.23962279, -0.05578459, -0.073864885, -0.16175121, -0.046830498, -0.071334355, -0.12525235, -0.1762308, -0.17853433, -0.05481769, -0.10788009, -0.12848935, -0.21946594, -0.07054761, -0.0043790466, -0.1421547, -0.062456187, -0.038439218, -0.01970637, 0.04187341, -0.11302035, -0.06571084, 0.012916437, 0.008474918, -0.058553338, -0.05822342, -0.0072570713, -0.117029555}, New Integer() {150, 3}, "c"c)
			Dim assertion As INDArray = Nd4j.create(New Double() {-1.0949919, -1.1009998, -1.0998554, -1.1079034, -1.1003298, -1.0877079, -1.0957471, -1.0970343, -1.1030709, -1.1040032, -1.0929829, -1.0988811, -1.1042137, -1.1054386, -1.0862994, -1.0849832, -1.1002628, -1.110759, -1.0950522, -1.0957897, -1.1050256, -1.0946627, -1.1018535, -1.0993341, -1.098271, -1.1026394, -1.0949415, -1.0964833, -1.0981458, -1.1012137, -1.1069958, -1.0990812, -1.0898339, -1.0839114, -1.1073275, -1.104763, -1.0936487, -1.1008704, -1.1013364, -1.0997316, -1.0965669, -1.0995414, -1.1094468, -1.0952749, -1.0912066, -1.1022308, -1.0910097, -1.10264, -1.1618325, -1.1690543, -0.97703075, -1.1036359, -1.0788001, -1.1137247, -1.0899199, -1.1072751, -1.0987172, -1.13885, -1.0621073, -1.0963553, -1.1102668, -1.0912181, -1.0944556, -1.0698514, -1.1425608, -1.0848886, -1.0910273, -1.1232094, -1.0820669, -1.1177288, -1.0674189, -1.1114442, -1.083288, -1.0998721, -1.1128973, -1.1165779, -1.0972028, -1.0823506, -1.063015, -1.1330047, -1.1010458, -1.1247563, -1.1175389, -1.0550222, -1.0999088, -1.1129185, -1.0832311, -1.0802083, -1.1165311, -1.0994279, -1.0973024, -1.0857224, -1.1129993, -1.124351, -1.076585, -1.0954784, -1.0795343, -1.0691221, -1.1490538, -1.1465356, -1.0648118, -1.0862738, -1.0950559, -1.1058216, -1.0949979, -1.0828075, -1.1237478, -1.0897596, -1.1059818, -1.0852317, -1.1047591, -1.100405, -1.0904485, -1.1050392, -1.0961069, -1.0965644, -1.1031815, -1.0815891, -1.0888373, -1.125975, -1.0903746, -1.1100911, -1.0954757, -1.1110255, -1.0917934, -1.093133, -1.1051062, -1.1049292, -1.0859231, -1.1046766, -1.0992017, -1.0919989, -1.082815, -1.1074618, -1.10575, -1.0909829, -1.0977867, -1.1071333, -1.116398, -1.0931609, -1.0865234, -1.0971736, -1.1093404, -1.0894235, -1.0886579, -1.0949628, -1.1123666, -1.095872, -1.0940536, -1.1059519, -1.1018884, -1.0942696, -1.0996943, -1.0963987, -1.1057898, -1.0936887, -1.102288, -1.1016107, -1.0919713, -1.0952013, -1.1039451, -1.0967125, -1.0917866, -1.0969539, -1.1071577, -1.0841576, -1.1052121, -1.106626, -1.098331, -1.0990925, -1.0984138, -1.095848, -1.1072304, -1.0928164, -1.0921938, -1.0978565, -1.1058333, -1.1007886, -1.1036327, -1.0914562, -1.0939325, -1.1073442, -1.0946171, -1.0945718, -1.0939536, -1.107369, -1.1089264, -1.0922892, -1.0947019, -1.1073625, -1.1133835, -1.0755067, -1.1047142, -1.102877, -1.0883265, -1.0995088, -1.0964776, -1.0998539, -1.2125868, -1.2135757, -0.9027819, -1.115231, -1.0709579, -1.1102388, -1.0866234, -1.1004536, -1.1088862, -1.1537597, -1.0454466, -1.0995628, -1.1057239, -1.1056436, -1.0846179, -1.0445701, -1.1711081, -1.0843138, -1.0936275, -1.1313372, -1.0717777, -1.1160054, -1.0597894, -1.1212093, -1.0709189, -1.0943694, -1.131479, -1.1307347, -1.0900652, -1.0758451, -1.0502236, -1.1520857, -1.0961251, -1.1360092, -1.1360053, -1.0277731, -1.091318, -1.1288478, -1.0763988, -1.065361, -1.1322097, -1.0993836, -1.0881867, -1.0848137, -1.1232886, -1.133629, -1.0662166, -1.0971287, -1.0676445, -1.0546416, -1.1780928, -1.1673087, -1.0611565, -1.0707793, -1.0977826, -1.0995032, -1.0985519, -1.0761919, -1.1434338, -1.0776746, -1.0779177, -1.1014266, -1.1168783, -1.0964613, -1.0952622, -1.1041365, -1.0999078, -1.1081696, -1.0878639, -1.0992746, -1.0690144, -1.1284306, -1.1060928, -1.1209829, -1.0694662, -1.1174977, -1.0980213, -1.0806575, -1.1113796, -1.111681, -1.0732663, -1.0971633, -1.0886947, -1.110095, -1.0898226, -1.1144775, -1.0917242, -1.0868361, -1.1128345, -1.0963393, -1.1185608, -1.0912135, -1.086363, -1.1139716, -1.0969814, -1.0850945, -1.0947206, -1.0999122, -1.1012157, -1.0932035, -1.105744, -1.0969306, -1.0670104, -1.1290239, -1.100767, -1.1519758, -1.0700266, -1.0759057, -1.0683149, -1.0771854, -1.1524552, -1.1406635, -1.0451982, -1.1123937, -1.1621376, -1.1453509, -0.99676645, -1.1160396, -1.0692043, -1.1112604, -1.0837362, -1.0854926, -1.1272106, -1.0979462, -1.0721557, -1.1264727, -1.1378707, -1.1163357, -1.0440625, -1.0785028, -1.0698442, -1.1493783, -1.1612072, -1.0505308, -1.0872571, -1.0555155, -1.1635867, -1.0799185, -1.0216377, -1.1443856, -1.1345224, -1.0751246, -1.0277929, -1.2008144, -1.1185431, -1.0553844, -1.1233582, -1.1039788, -1.0797728, -1.1123724, -1.0159799, -1.0420641, -1.2544713, -1.1064723, -1.1284167, -1.0620935, -1.0654664, -1.1309781, -1.1004674, -1.0726943, -1.1294085, -1.0945506, -1.0974507, -1.1057259, -1.0927036, -1.1695204, -1.0438402, -1.086533, -1.1429209, -1.0986946, -1.0561051, -1.0885462, -1.149404, -1.0599625, -1.112509, -1.1389449, -1.046655, -1.0674819, -1.1093009, -1.119824, -1.1481767, -1.0585686, -1.0911404, -1.0579745, -1.050047, -1.194285, -1.136149, -1.08803, -1.0727472, -1.0830219, -1.1331651, -1.0805265, -1.1281672, -1.1262413, -1.0437706, -1.0489775, -1.1078012, -1.141249, -1.1517346, -1.1276698, -1.0213039, -1.0633042, -1.084772, -1.1497743, -1.0789506, -1.1388241, -1.0792432, -1.125674, -1.0188907, -1.1565453, -1.2263924, -1.0104843, -1.0711672, -1.1182799, -1.079075, -1.0988661, -1.0705098, -1.046906, -1.1836989, -1.0271709, -1.2082508, -1.0692605, -1.017894, -1.0635278, -1.2261873, -1.0583237, -1.0764041, -1.1642903, -1.0648377, -1.0893415, -1.1432595, -1.140007, -1.1423105, -1.0185939, -1.0557104, -1.0763197, -1.1672963, -1.09838, -1.0322114, -1.1699871, -1.1210208, -1.0970039, -1.078271, -1.0132385, -1.1681323, -1.1208228, -1.0738388, -1.0782803, -1.1453086, -1.0970035, -1.0460371, -1.1558095}, New Integer() {150, 3}, "c"c)
			Nd4j.Executioner.exec(New LogSoftMax(test))
			assertEquals(assertion, test)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmax(ByVal backend As Nd4jBackend)
			Dim vec As INDArray = Nd4j.linspace(1, 18, 18, DataType.DOUBLE)
			Dim matrix As INDArray = vec.dup().reshape(ChrW(3), 6)
			Nd4j.Executioner.exec(DirectCast(New SoftMax(matrix), CustomOp))
			Dim assertion As INDArray = Nd4j.create(New Double() {0.0042697787, 0.011606461, 0.031549633, 0.085760795, 0.23312202, 0.6336913, 0.0042697787, 0.011606461, 0.031549633, 0.085760795, 0.23312202, 0.6336913, 0.0042697787, 0.011606461, 0.031549633, 0.085760795, 0.23312202, 0.6336913}, New Integer() {3, 6}, "c"c)
			assertEquals(assertion, matrix)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStdev(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStdev(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Single() {0.9296161f, 0.31637555f, 0.1839188f}, New Integer() {1, 3}, ordering())
			Dim stdev As Double = arr.stdNumber().doubleValue()
			Dim stdev2 As Double = arr.std(1).getDouble(0)
			assertEquals(stdev, stdev2, 1e-3)

			Dim exp As Double = 0.39784279465675354
			assertEquals(exp, stdev, 1e-7f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariance(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.create(New Single() {0.9296161f, 0.31637555f, 0.1839188f}, New Integer() {1, 3}, ordering())
			Dim var As Double = arr.varNumber().doubleValue()
			Dim temp As INDArray = arr.var(1)
			Dim var2 As Double = arr.var(1).getDouble(0)
			assertEquals(var, var2, 1e-1)

			Dim exp As Double = 0.15827888250350952
			assertEquals(exp, var, 1e-7f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEpsOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEpsOps(ByVal backend As Nd4jBackend)
			Dim ones As INDArray = Nd4j.ones(DataType.DOUBLE, 1, 6)
			Dim tiny As Double = 1.000000000000001
			assertTrue(ones.eps(tiny).all())
			Dim consec As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(1), -1)
			assertTrue(consec.eps(5).any())
			assertTrue(consec.sub(1).eps(5).any())
			assertTrue(consec.sub(1).eps(5).getDouble(0, 5) = 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVarianceSingleVsMultipleDimensions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVarianceSingleVsMultipleDimensions(ByVal backend As Nd4jBackend)
			' this test should always run in double
			Dim type As DataType = Nd4j.dataType()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Nd4j.Random.setSeed(12345)

			'Generate C order random numbers. Strides: [500,100,10,1]
			Dim fourd As INDArray = Nd4j.rand("c"c, New Integer() {100, 5, 10, 10}).muli(10)
			Dim twod As INDArray = Shape.newShapeNoCopy(fourd, New Integer() {100, 5 * 10 * 10}, False)

			'Population variance. These two should be identical
			Dim var4 As INDArray = fourd.var(False, 1, 2, 3)
			Dim var2 As INDArray = twod.var(False, 1)

			'Manual calculation of population variance, not bias corrected
			'https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Na.C3.AFve_algorithm
			Dim sums(99) As Double
			Dim sumSquares(99) As Double
			Dim iter As New NdIndexIterator(fourd.shape())
			Do While iter.MoveNext()
				Dim [next] As val = iter.Current
				Dim d As Double = fourd.getDouble([next])

				sums(CInt(Math.Truncate([next](0)))) += d
				sumSquares(CInt(Math.Truncate([next](0)))) += d * d
			Loop

			Dim manualVariance(99) As Double
			Dim N As val = (fourd.length() \ sums.Length)
			For i As Integer = 0 To sums.Length - 1
				manualVariance(i) = (sumSquares(i) - (sums(i) * sums(i)) / N) / N
			Next i

			Dim var4bias As INDArray = fourd.var(True, 1, 2, 3)
			Dim var2bias As INDArray = twod.var(True, 1)

			assertArrayEquals(var2.data().asDouble(), var4.data().asDouble(), 1e-5)
			assertArrayEquals(manualVariance, var2.data().asDouble(), 1e-5)
			assertArrayEquals(var2bias.data().asDouble(), var4bias.data().asDouble(), 1e-5)

			DataTypeUtil.setDTypeForContext(type)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHistogram1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHistogram1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1, 1000, 100000, DataType.DOUBLE)
			Dim z As INDArray = Nd4j.zeros(DataType.LONG,New Long(){20})

			Dim xDup As INDArray = x.dup()
			Dim zDup As INDArray = z.dup()

			Dim zExp As INDArray = Nd4j.create(DataType.LONG, 20).assign(5000)

			Dim histogram As val = New Histogram(x, z)

			Nd4j.Executioner.exec(histogram)

			Nd4j.Executioner.commit()

			assertEquals(xDup, x)
			assertNotEquals(zDup, z)

			assertEquals(zExp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHistogram2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHistogram2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Single() {0f, 0f, 0f, 5f, 5f, 5f, 10f, 10f, 10f})

			Dim xDup As INDArray = x.dup()

			Dim zExp As INDArray = Nd4j.zeros(DataType.LONG, 10).putScalar(0, 3).putScalar(5, 3).putScalar(9, 3)

			Dim histogram As val = New Histogram(x, 10)

			Dim z As val = Nd4j.Executioner.exec(histogram)(0)

			assertEquals(xDup, x)

	'        log.info("bins: {}", z);

			assertEquals(zExp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEuclideanManhattanDistanceAlongDimension_Rank4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEuclideanManhattanDistanceAlongDimension_Rank4(ByVal backend As Nd4jBackend)
			Dim initialType As DataType = Nd4j.dataType()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Nd4j.Random.setSeed(12345)
			Dim firstOneExample As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape("c"c, 1, 2, 2, 2)
			Dim secondOneExample As INDArray = firstOneExample.add(1)

			Dim d1() As Double = firstOneExample.data().asDouble()
			Dim d2() As Double = secondOneExample.data().asDouble()
			Dim sumSquaredDiff As Double = 0.0
			Dim expManhattanDistance As Double = 0.0
			For i As Integer = 0 To d1.Length - 1
				Dim diff As Double = d1(i) - d2(i)
				sumSquaredDiff += diff * diff
				expManhattanDistance += Math.Abs(diff)
			Next i
			Dim expectedEuclidean As Double = Math.Sqrt(sumSquaredDiff)
	'        System.out.println("Expected, Euclidean: " + expectedEuclidean);
	'        System.out.println("Expected, Manhattan: " + expManhattanDistance);

			Dim mb As Integer = 2
			Dim firstOrig As INDArray = Nd4j.create(mb, 2, 2, 2)
			Dim secondOrig As INDArray = Nd4j.create(mb, 2, 2, 2)
			For i As Integer = 0 To mb - 1
				firstOrig.put(New INDArrayIndex() {point(i), all(), all(), all()}, firstOneExample)
				secondOrig.put(New INDArrayIndex() {point(i), all(), all(), all()}, secondOneExample)
			Next i

			For Each order As Char In New Char() {"c"c, "f"c}
				Dim first As INDArray = firstOrig.dup(order)
				Dim second As INDArray = secondOrig.dup(order)

				assertEquals(firstOrig, first)
				assertEquals(secondOrig, second)


				Dim [out] As INDArray = Nd4j.Executioner.exec(New EuclideanDistance(first, second, 1, 2, 3))
				Dim firstTadInfo As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(first, 1, 2, 3)
				Dim secondTadInfo As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(second, 1, 2, 3)


				Dim outManhattan As INDArray = Nd4j.Executioner.exec(New ManhattanDistance(first, second, 1, 2, 3))

	'            System.out.println("\n\nOrder: " + order);
	'            System.out.println("Euclidean:");
				'System.out.println(Arrays.toString(out.getRow(0).dup().data().asDouble()));
				'System.out.println(Arrays.toString(out.getRow(1).dup().data().asDouble()));

				assertEquals([out].getDouble(0), [out].getDouble(1), 1e-5)

	'            System.out.println("Manhattan:");
				'System.out.println(Arrays.toString(outManhattan.getRow(0).dup().data().asDouble()));
				'System.out.println(Arrays.toString(outManhattan.getRow(1).dup().data().asDouble()));

				assertEquals(expManhattanDistance, outManhattan.getDouble(0), 1e-5)
				assertEquals(expectedEuclidean, [out].getDouble(0), 1e-5)
			Next order

			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPile1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPile1(ByVal backend As Nd4jBackend)
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 9
				arrays.Add(Nd4j.create(10, 10).assign(i))
			Next i

			Dim pile As INDArray = Nd4j.pile(arrays)

			assertEquals(3, pile.rank())
			For i As Integer = 0 To 9
				assertEquals(CSng(i), pile.tensorAlongDimension(i, 1, 2).getDouble(0), 0.01)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPile2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPile2(ByVal backend As Nd4jBackend)
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 9
				arrays.Add(Nd4j.create(10, 10, 10).assign(i).castTo(DataType.FLOAT))
			Next i

			Dim pile As INDArray = Nd4j.pile(arrays).castTo(DataType.FLOAT)

			assertEquals(4, pile.rank())
			For i As Integer = 0 To 9
				assertEquals(CSng(i), pile.tensorAlongDimension(i, 1, 2, 3).getDouble(0), 0.01)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMean1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(32, 100, 100).assign(-119f)
			For i As Integer = 0 To 31
				Dim tad As val = array.tensorAlongDimension(i, 1, 2)
				tad.assign(CSng(100) + i)
			Next i

			For i As Integer = 0 To 31
				Dim tensor As INDArray = array.tensorAlongDimension(i, 1, 2)
	'            log.info("tad {}: {}", i, array.getDouble(0));
				assertEquals(CSng(100 + i) * (100 * 100), tensor.sumNumber().floatValue(), 0.001f)
				assertEquals(CSng(100) + i, tensor.meanNumber().floatValue(), 0.001f)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMean2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(32, 100, 100)
			For i As Integer = 0 To 31
				array.tensorAlongDimension(i, 1, 2).assign(CSng(100) + i)
			Next i

			Dim mean As INDArray = array.mean(1, 2)
			For i As Integer = 0 To 31
				assertEquals(CSng(100) + i, mean.getFloat(i), 0.001f)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2_1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.rand(1769472, 9)

			Dim max As INDArray = array.max(1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2_2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.rand(New Integer(){127, 164}, 1, 100, Nd4j.Random)

			Dim norm2 As Double = array.norm2Number().doubleValue()
		End Sub

		''' <summary>
		''' This test fails, but that's ok.
		''' It's here only as reminder, that in some cases we can have EWS==1 for better performances.
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadEws(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadEws(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(32, 5, 10)
			assertEquals(1, array.tensorAlongDimension(0, 1, 2).elementWiseStride())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTear1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTear1(ByVal backend As Nd4jBackend)
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			Dim num As val = 10
			For i As Integer = 0 To num - 1
				arrays.Add(Nd4j.create(5, 20).assign(i))
			Next i

			Dim pile As INDArray = Nd4j.pile(arrays)

	'        log.info("Pile: {}", pile);

			Dim tears() As INDArray = Nd4j.tear(pile, 1, 2)

			For i As Integer = 0 To num - 1
				assertEquals(CSng(i), tears(i).meanNumber().floatValue(), 0.01f)
			Next i
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace