Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Eps = org.nd4j.linalg.api.ops.impl.transforms.comparison.Eps
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports ExecutorServiceProvider = org.nd4j.linalg.executors.ExecutorServiceProvider
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg



	''' <summary>
	''' NDArrayTests for fortran ordering
	''' 
	''' @author Adam Gibson
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class NDArrayTestsFortran extends BaseNd4jTestWithBackends
	Public Class NDArrayTestsFortran
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarOps(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.ones(27).data(), New Long() {3, 3, 3})
			assertEquals(27R, n.length(), 1e-1)
			n.addi(Nd4j.scalar(1R))
			n.subi(Nd4j.scalar(1.0R))
			n.muli(Nd4j.scalar(1.0R))
			n.divi(Nd4j.scalar(1.0R))

			n = Nd4j.create(Nd4j.ones(27).data(), New Long() {3, 3, 3})
			assertEquals(27, n.sumNumber().doubleValue(), 1e-1)
			Dim a As INDArray = n.slice(2)
			assertEquals(True, New Long() {3, 3}.SequenceEqual(a.shape()))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnMmul(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 10, 18, DataType.FLOAT).data()
			Dim x2 As INDArray = Nd4j.create(data, New Long() {2, 3, 3})
			data = Nd4j.linspace(1, 12, 9, DataType.FLOAT).data()
			Dim y2 As INDArray = Nd4j.create(data, New Long() {3, 3})
			Dim z2 As INDArray = Nd4j.create(DataType.FLOAT, New Long() {3, 2}, "f"c)
			z2.putColumn(0, y2.getColumn(0))
			z2.putColumn(1, y2.getColumn(1))
			Dim nofOffset As INDArray = Nd4j.create(DataType.FLOAT, New Long() {3, 3}, "f"c)
			nofOffset.assign(x2.slice(0))
			assertEquals(nofOffset, x2.slice(0))

			Dim slice As INDArray = x2.slice(0)
			Dim zeroOffsetResult As INDArray = slice.mmul(z2)
			Dim offsetResult As INDArray = nofOffset.mmul(z2)
			assertEquals(zeroOffsetResult, offsetResult)


			Dim slice1 As INDArray = x2.slice(1)
			Dim noOffset2 As INDArray = Nd4j.create(DataType.FLOAT, slice1.shape())
			noOffset2.assign(slice1)
			assertEquals(slice1, noOffset2)

			Dim noOffsetResult As INDArray = noOffset2.mmul(z2)
			Dim slice1OffsetResult As INDArray = slice1.mmul(z2)

			assertEquals(noOffsetResult, slice1OffsetResult)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowVectorGemm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowVectorGemm(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), -1).castTo(DataType.DOUBLE)
			Dim other As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(4), 4).castTo(DataType.DOUBLE)
			Dim result As INDArray = linspace.mmul(other)
			Dim assertion As INDArray = Nd4j.create(New Double() {30.0, 70.0, 110.0, 150.0}, New Integer(){1, 4})
			assertEquals(assertion, result)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRepmat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRepmat(ByVal backend As Nd4jBackend)
			Dim rowVector As INDArray = Nd4j.create(1, 4)
			Dim repmat As INDArray = rowVector.repmat(4, 4)
			assertTrue(New Long() {4, 16}.SequenceEqual(repmat.shape()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReadWrite() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReadWrite()
			Dim write As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(write, dos)

			Dim bis As New MemoryStream(bos.toByteArray())
			Dim dis As New DataInputStream(bis)
			Dim read As INDArray = Nd4j.read(dis)
			assertEquals(write, read)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReadWriteDouble() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReadWriteDouble()
			Dim write As INDArray = Nd4j.linspace(1, 4, 4, DataType.FLOAT)
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(write, dos)

			Dim bis As New MemoryStream(bos.toByteArray())
			Dim dis As New DataInputStream(bis)
			Dim read As INDArray = Nd4j.read(dis)
			assertEquals(write, read)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiThreading() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiThreading()
			Dim ex As ExecutorService = ExecutorServiceProvider.ExecutorService

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<java.util.concurrent.Future<?>> list = new java.util.ArrayList<>(100);
			Dim list As IList(Of Future(Of Object)) = New List(Of Future(Of Object))(100)
			For i As Integer = 0 To 99
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.concurrent.Future<?> future = ex.submit(() ->
				Dim future As Future(Of Object) = ex.submit(Sub()
				Dim dot As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE)
				Transforms.sigmoid(dot)
				End Sub)
				list.Add(future)
			Next i
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (java.util.concurrent.Future<?> future : list)
			For Each future As Future(Of Object) In list
				future.get(1, TimeUnit.MINUTES)
			Next future

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastingGenerated(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastingGenerated(ByVal backend As Nd4jBackend)
			Dim broadcastShape()() As Integer = NDArrayCreationUtil.getRandomBroadCastShape(7, 6, 10)
			Dim broadCastList As IList(Of IList(Of Pair(Of INDArray, String))) = New List(Of IList(Of Pair(Of INDArray, String)))(broadcastShape.Length)
			For Each shape As Integer() In broadcastShape
				Dim arrShape As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.get6dPermutedWithShape(7, shape, DataType.DOUBLE)
				broadCastList.Add(arrShape)
				broadCastList.Add(NDArrayCreationUtil.get6dReshapedWithShape(7, shape, DataType.DOUBLE))
				broadCastList.Add(NDArrayCreationUtil.getAll6dTestArraysWithShape(7, shape, DataType.DOUBLE))
			Next shape

			For Each b As IList(Of Pair(Of INDArray, String)) In broadCastList
				For Each lombok As Pair(Of INDArray, String) In b
					Dim inputArrBroadcast As INDArray = val.getFirst()
					Dim destShape As val = NDArrayCreationUtil.broadcastToShape(inputArrBroadcast.shape(), 7)
					Dim output As INDArray = inputArrBroadcast.broadcast(NDArrayCreationUtil.broadcastToShape(inputArrBroadcast.shape(), 7))
					assertArrayEquals(destShape, output.shape())
				Next lombok
			Next b



		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadCasting(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadCasting(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.arange(0, 3).reshape(ChrW(3), 1).castTo(DataType.DOUBLE)
			Dim ret As INDArray = first.broadcast(3, 4)
			Dim testRet As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 0, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2}
			})
			assertEquals(testRet, ret)
			Dim r As INDArray = Nd4j.arange(0, 4).reshape(ChrW(1), 4).castTo(DataType.DOUBLE)
			Dim r2 As INDArray = r.broadcast(4, 4)
			Dim testR2 As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3},
				New Double() {0, 1, 2, 3}
			})
			assertEquals(testR2, r2)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneTensor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneTensor(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(1, 1, 1, 1, 1, 1, 1)
			Dim matrixToBroadcast As INDArray = Nd4j.ones(1, 1)
			assertEquals(matrixToBroadcast.broadcast(arr.shape()), arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortWithIndicesDescending(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortWithIndicesDescending(ByVal backend As Nd4jBackend)
			Dim toSort As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			'indices,data
			Dim sorted() As INDArray = Nd4j.sortWithIndices(toSort.dup(), 1, False)
			Dim sorted2 As INDArray = Nd4j.sort(toSort.dup(), 1, False)
			assertEquals(sorted(1), sorted2)
			Dim shouldIndex As INDArray = Nd4j.create(New Double() {1, 1, 0, 0}, New Long() {2, 2})
			assertEquals(shouldIndex, sorted(0),getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortDeadlock(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortDeadlock(ByVal backend As Nd4jBackend)
			Dim toSort As val = Nd4j.linspace(DataType.DOUBLE, 1, 32*768, 1).reshape(ChrW(32), 768)

			Dim sorted As val = Nd4j.sort(toSort.dup(), 1, False)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSortWithIndices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSortWithIndices(ByVal backend As Nd4jBackend)
			Dim toSort As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			'indices,data
			Dim sorted() As INDArray = Nd4j.sortWithIndices(toSort.dup(), 1, True)
			Dim sorted2 As INDArray = Nd4j.sort(toSort.dup(), 1, True)
			assertEquals(sorted(1), sorted2)
			Dim shouldIndex As INDArray = Nd4j.create(New Double() {0, 0, 1, 1}, New Long() {2, 2})
			assertEquals(shouldIndex, sorted(0),getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNd4jSortScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNd4jSortScalar(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim sorted As INDArray = Nd4j.sort(linspace, 1, False)
	'        System.out.println(sorted);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSwapAxesFortranOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSwapAxesFortranOrder(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 30, 30, DataType.DOUBLE).data(), New Long() {3, 5, 2}).castTo(DataType.DOUBLE)
			Dim i As Integer = 0
			Do While i < n.slices()
				Dim nSlice As INDArray = n.slice(i)
				Dim j As Integer = 0
				Do While j < nSlice.slices()
					Dim sliceJ As INDArray = nSlice.slice(j)
	'                System.out.println(sliceJ);
					j += 1
				Loop
	'            System.out.println(nSlice);
				i += 1
			Loop
			Dim slice As INDArray = n.swapAxes(2, 1)
			Dim assertion As INDArray = Nd4j.create(New Double() {1, 4, 7, 10, 13})
			Dim test As INDArray = slice.slice(0).slice(0)
			assertEquals(assertion, test)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimShuffle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimShuffle(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim twoOneTwo As INDArray = n.dimShuffle(New Object() {0, "x"c, 1}, New Integer() {0, 1}, New Boolean() {False, False})
			assertTrue(New Long() {2, 1, 2}.SequenceEqual(twoOneTwo.shape()))

			Dim reverse As INDArray = n.dimShuffle(New Object() {1, "x"c, 0}, New Integer() {1, 0}, New Boolean() {False, False})
			assertTrue(New Long() {2, 1, 2}.SequenceEqual(reverse.shape()))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetVsGetScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetVsGetScalar(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim element As Single = a.getFloat(0, 1)
			Dim element2 As Double = a.getDouble(0, 1)
			assertEquals(element, element2, 1e-1)
			Dim a2 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim element23 As Single = a2.getFloat(0, 1)
			Dim element22 As Double = a2.getDouble(0, 1)
			assertEquals(element23, element22, 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDivide(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDivide(ByVal backend As Nd4jBackend)
			Dim two As INDArray = Nd4j.create(New Single() {2, 2, 2, 2})
			Dim div As INDArray = two.div(two)
			assertEquals(Nd4j.ones(DataType.FLOAT, 4), div,getFailureMessage(backend))

			Dim half As INDArray = Nd4j.create(New Single() {0.5f, 0.5f, 0.5f, 0.5f}, New Long() {2, 2})
			Dim divi As INDArray = Nd4j.create(New Single() {0.3f, 0.6f, 0.9f, 0.1f}, New Long() {2, 2})
			Dim assertion As INDArray = Nd4j.create(New Single() {1.6666666f, 0.8333333f, 0.5555556f, 5}, New Long() {2, 2})
			Dim result As INDArray = half.div(divi)
			assertEquals(assertion, result,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSigmoid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSigmoid(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim assertion As INDArray = Nd4j.create(New Single() {0.73105858f, 0.88079708f, 0.95257413f, 0.98201379f})
			Dim sigmoid As INDArray = Transforms.sigmoid(n, False)
			assertEquals(assertion, sigmoid,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNeg(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNeg(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim assertion As INDArray = Nd4j.create(New Single() {-1, -2, -3, -4})
			Dim neg As INDArray = Transforms.neg(n)
			assertEquals(assertion, neg,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineSim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCosineSim(ByVal backend As Nd4jBackend)
			Dim vec1 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			Dim vec2 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			Dim sim As Double = Transforms.cosineSim(vec1, vec2)
			assertEquals(1, sim, 1e-1,getFailureMessage(backend))

			Dim vec3 As INDArray = Nd4j.create(New Single() {0.2f, 0.3f, 0.4f, 0.5f})
			Dim vec4 As INDArray = Nd4j.create(New Single() {0.6f, 0.7f, 0.8f, 0.9f})
			sim = Transforms.cosineSim(vec3, vec4)
			assertEquals(0.98, sim, 1e-1,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExp(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			Dim assertion As INDArray = Nd4j.create(New Double() {2.71828183f, 7.3890561f, 20.08553692f, 54.59815003f})
			Dim exped As INDArray = Transforms.exp(n)
			assertEquals(assertion, exped)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalar(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.scalar(1.0f)
			assertEquals(True, a.Scalar)

			Dim n As INDArray = Nd4j.create(New Single() {1.0f}, New Long(){})
			assertEquals(n, a)
			assertTrue(n.Scalar)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWrap(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWrap(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {2, 4}
			Dim d As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(shape(0)), shape(1))
			Dim n As INDArray = d
			assertEquals(d.rows(), n.rows())
			assertEquals(d.columns(), n.columns())

			Dim vector As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE)
			Dim testVector As INDArray = vector
			For i As Integer = 0 To vector.length() - 1
				assertEquals(vector.getDouble(i), testVector.getDouble(i), 1e-1)
			Next i
			assertEquals(3, testVector.length())
			assertEquals(True, testVector.Vector)
			assertEquals(True, Shape.shapeEquals(New Long() {3}, testVector.shape()))

			Dim row12 As INDArray = Nd4j.linspace(1, 2, 2, DataType.DOUBLE).reshape(ChrW(2), 1)
			Dim row22 As INDArray = Nd4j.linspace(3, 4, 2, DataType.DOUBLE).reshape(ChrW(1), 2)

			assertEquals(row12.rows(), 2)
			assertEquals(row12.columns(), 1)
			assertEquals(row22.rows(), 1)
			assertEquals(row22.columns(), 2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRowFortran(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRowFortran(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.FLOAT).data(), New Long() {2, 2})
			Dim column As INDArray = Nd4j.create(New Single() {1, 3})
			Dim column2 As INDArray = Nd4j.create(New Single() {2, 4})
			Dim testColumn As INDArray = n.getRow(0)
			Dim testColumn1 As INDArray = n.getRow(1)
			assertEquals(column, testColumn)
			assertEquals(column2, testColumn1)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumnFortran(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetColumnFortran(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2})
			Dim column As INDArray = Nd4j.create(New Double() {1, 2})
			Dim column2 As INDArray = Nd4j.create(New Double() {3, 4})
			Dim testColumn As INDArray = n.getColumn(0)
			Dim testColumn1 As INDArray = n.getColumn(1)
	'        log.info("testColumn shape: {}", Arrays.toString(testColumn.shapeInfoDataBuffer().asInt()));
			assertEquals(column, testColumn)
			assertEquals(column2, testColumn1)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetColumns(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3).castTo(DataType.DOUBLE)
	'        log.info("Original: {}", matrix);
			Dim matrixGet As INDArray = matrix.getColumns(1, 2)
			Dim matrixAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 5},
				New Double() {4, 6}
			})
	'        log.info("order A: {}", Arrays.toString(matrixAssertion.shapeInfoDataBuffer().asInt()));
	'        log.info("order B: {}", Arrays.toString(matrixGet.shapeInfoDataBuffer().asInt()));
	'        log.info("data A: {}", Arrays.toString(matrixAssertion.data().asFloat()));
	'        log.info("data B: {}", Arrays.toString(matrixGet.data().asFloat()));
			assertEquals(matrixAssertion, matrixGet)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorInit(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorInit(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data()
			Dim arr As INDArray = Nd4j.create(data, New Long() {1, 4})
			assertEquals(True, arr.RowVector)
			Dim arr2 As INDArray = Nd4j.create(data, New Long() {1, 4})
			assertEquals(True, arr2.RowVector)

			Dim columnVector As INDArray = Nd4j.create(data, New Long() {4, 1})
			assertEquals(True, columnVector.ColumnVector)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignOffset(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssignOffset(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(5, 5)
			Dim row As INDArray = arr.slice(1)
			row.assign(1)
			assertEquals(Nd4j.ones(5), row)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumns(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Long() {3, 2}).castTo(DataType.DOUBLE)
			Dim column As INDArray = Nd4j.create(New Double() {1, 2, 3})
			arr.putColumn(0, column)

			Dim firstColumn As INDArray = arr.getColumn(0)

			assertEquals(column, firstColumn)


			Dim column1 As INDArray = Nd4j.create(New Double() {4, 5, 6})
			arr.putColumn(1, column1)
			Dim testRow1 As INDArray = arr.getColumn(1)
			assertEquals(column1, testRow1)


			Dim evenArr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {2, 2})
			Dim put As INDArray = Nd4j.create(New Double() {5, 6})
			evenArr.putColumn(1, put)
			Dim testColumn As INDArray = evenArr.getColumn(1)
			assertEquals(put, testColumn)


			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2}).castTo(DataType.DOUBLE)
			Dim column23 As INDArray = n.getColumn(0)
			Dim column12 As INDArray = Nd4j.create(New Double() {1, 2})
			assertEquals(column23, column12)


			Dim column0 As INDArray = n.getColumn(1)
			Dim column01 As INDArray = Nd4j.create(New Double() {3, 4})
			assertEquals(column0, column01)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRow(ByVal backend As Nd4jBackend)
			Dim d As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim n As INDArray = d.dup()

			'works fine according to matlab, let's go with it..
			'reproduce with:  A = newShapeNoCopy(linspace(1,4,4),[2 2 ]);
			'A(1,2) % 1 index based
			Dim nFirst As Single = 3
			Dim dFirst As Single = d.getFloat(0, 1)
			assertEquals(nFirst, dFirst, 1e-1)
			assertEquals(d, n)
			assertEquals(True, New Long() {2, 2}.SequenceEqual(n.shape()))

			Dim newRow As INDArray = Nd4j.linspace(5, 6, 2, DataType.DOUBLE)
			n.putRow(0, newRow)
			d.putRow(0, newRow)


			Dim testRow As INDArray = n.getRow(0)
			assertEquals(newRow.length(), testRow.length())
			assertEquals(True, Shape.shapeEquals(New Long() {1, 2}, testRow.shape()))


			Dim nLast As INDArray = Nd4j.create(Nd4j.linspace(1, 4, 4, DataType.DOUBLE).data(), New Long() {2, 2}).castTo(DataType.DOUBLE)
			Dim row As INDArray = nLast.getRow(1)
			Dim row1 As INDArray = Nd4j.create(New Double() {2, 4})
			assertEquals(row, row1)


			Dim arr As INDArray = Nd4j.create(New Long() {3, 2}).castTo(DataType.DOUBLE)
			Dim evenRow As INDArray = Nd4j.create(New Double() {1, 2})
			arr.putRow(0, evenRow)
			Dim firstRow As INDArray = arr.getRow(0)
			assertEquals(True, Shape.shapeEquals(New Long() {1, 2}, firstRow.shape()))
			Dim testRowEven As INDArray = arr.getRow(0)
			assertEquals(evenRow, testRowEven)


			Dim row12 As INDArray = Nd4j.create(New Double() {5, 6})
			arr.putRow(1, row12)
			assertEquals(True, Shape.shapeEquals(New Long() {1, 2}, arr.getRow(0).shape()))
			Dim testRow1 As INDArray = arr.getRow(1)
			assertEquals(row12, testRow1)


			Dim multiSliceTest As INDArray = Nd4j.create(Nd4j.linspace(1, 16, 16, DataType.DOUBLE).data(), New Long() {4, 2, 2}).castTo(DataType.DOUBLE)
			Dim test As INDArray = Nd4j.create(New Double() {2, 10})
			Dim test2 As INDArray = Nd4j.create(New Double() {6, 14})

			Dim multiSliceRow1 As INDArray = multiSliceTest.slice(1).getRow(0)
			Dim multiSliceRow2 As INDArray = multiSliceTest.slice(1).getRow(1)

			assertEquals(test, multiSliceRow1)
			assertEquals(test2, multiSliceRow2)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInplaceTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInplaceTranspose(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.rand(3, 4)
			Dim orig As INDArray = test.dup()
			Dim transposei As INDArray = test.transposei()

			Dim i As Integer = 0
			Do While i < orig.rows()
				Dim j As Integer = 0
				Do While j < orig.columns()
					assertEquals(orig.getDouble(i, j), transposei.getDouble(j, i), 1e-1)
					j += 1
				Loop
				i += 1
			Loop
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulF(ByVal backend As Nd4jBackend)

			Dim data As DataBuffer = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).data()
			Dim n As INDArray = Nd4j.create(data, New Long() {1, 10})
			Dim transposed As INDArray = n.transpose()
			assertEquals(True, n.RowVector)
			assertEquals(True, transposed.ColumnVector)


			Dim innerProduct As INDArray = n.mmul(transposed)

			Dim scalar As INDArray = Nd4j.scalar(385.0).reshape(ChrW(1), 1)
			assertEquals(scalar, innerProduct,getFailureMessage(backend))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowsColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowsColumns(ByVal backend As Nd4jBackend)
			Dim data As DataBuffer = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).data()
			Dim rows As INDArray = Nd4j.create(data, New Long() {2, 3})
			assertEquals(2, rows.rows())
			assertEquals(3, rows.columns())

			Dim columnVector As INDArray = Nd4j.create(data, New Long() {6, 1})
			assertEquals(6, columnVector.rows())
			assertEquals(1, columnVector.columns())
			Dim rowVector As INDArray = Nd4j.create(data, New Long() {1, 6})
			assertEquals(1, rowVector.rows())
			assertEquals(6, rowVector.columns())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.ones(100).castTo(DataType.DOUBLE).data(), New Long() {5, 5, 4})
			Dim transpose As INDArray = n.transpose()
			assertEquals(n.length(), transpose.length())
			assertEquals(True, New Long() {4, 5, 5}.SequenceEqual(transpose.shape()))

			Dim rowVector As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).reshape(ChrW(1), -1)
			assertTrue(rowVector.RowVector)
			Dim columnVector As INDArray = rowVector.transpose()
			assertTrue(columnVector.ColumnVector)


			Dim linspaced As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim transposed As INDArray = Nd4j.create(New Double() {1, 3, 2, 4}, New Long() {2, 2})
			assertEquals(transposed, linspaced.transpose())

			linspaced = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			'fortran ordered
			Dim transposed2 As INDArray = Nd4j.create(New Double() {1, 3, 2, 4}, New Long() {2, 2})
			transposed = linspaced.transpose()
			assertEquals(transposed, transposed2)


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddMatrix(ByVal backend As Nd4jBackend)
			Dim five As INDArray = Nd4j.ones(5)
			five.addi(five.dup())
			Dim twos As INDArray = Nd4j.valueArrayOf(5, 2)
			assertEquals(twos, five,getFailureMessage(backend))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMMul(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3},
				New Double() {4, 5, 6}
			})

			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {14, 32},
				New Double() {32, 77}
			})

			Dim test As INDArray = arr.mmul(arr.transpose())
			assertEquals(assertion, test,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutSlice(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 27, 27, DataType.DOUBLE).reshape(ChrW(3), 3, 3)
			Dim newSlice As INDArray = Nd4j.create(DataType.DOUBLE, 3, 3)
			Nd4j.exec(New PrintVariable(newSlice))
			log.info("Slice: {}", newSlice)
			n.putSlice(0, newSlice)
			assertEquals(newSlice, n.slice(0),getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowVectorMultipleIndices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowVectorMultipleIndices(ByVal backend As Nd4jBackend)
			Dim linear As INDArray = Nd4j.create(DataType.DOUBLE, 1, 4)
			linear.putScalar(New Long() {0, 1}, 1)
			assertEquals(linear.getDouble(0, 1), 1, 1e-1,getFailureMessage(backend))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDim1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDim1(ByVal backend As Nd4jBackend)
			Dim sum As INDArray = Nd4j.linspace(1, 2, 2, DataType.DOUBLE).reshape(ChrW(2), 1)
			Dim same As INDArray = sum.dup()
			assertEquals(same.sum(1), sum.reshape(ChrW(2)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEps(ByVal backend As Nd4jBackend)
			Dim ones As val = Nd4j.ones(5)
			Dim res As val = Nd4j.createUninitialized(DataType.BOOL, 5)
			assertTrue(Nd4j.Executioner.exec(New Eps(ones, ones, res)).all())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogDouble(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogDouble(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).castTo(DataType.DOUBLE)
			Dim log As INDArray = Transforms.log(linspace)
			Dim assertion As INDArray = Nd4j.create(New Double() {0, 0.6931471805599453, 1.0986122886681098, 1.3862943611198906, 1.6094379124341005, 1.791759469228055})
			assertEquals(assertion, log)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorSum(ByVal backend As Nd4jBackend)
			Dim lin As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			assertEquals(10.0, lin.sumNumber().doubleValue(), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorSum2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorSum2(ByVal backend As Nd4jBackend)
			Dim lin As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			assertEquals(10.0, lin.sumNumber().doubleValue(), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorSum3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorSum3(ByVal backend As Nd4jBackend)
			Dim lin As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			Dim lin2 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4})
			assertEquals(lin, lin2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSmallSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSmallSum(ByVal backend As Nd4jBackend)
			Dim base As INDArray = Nd4j.create(New Double() {5.843333333333335, 3.0540000000000007})
			base.addi(1e-12)
			Dim assertion As INDArray = Nd4j.create(New Double() {5.84333433, 3.054001})
			assertEquals(assertion, base)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 20, 20, DataType.DOUBLE).data(), New Long() {5, 4})
			Dim transpose As INDArray = n.transpose()
			Dim permute As INDArray = n.permute(1, 0)
			assertEquals(permute, transpose)
			assertEquals(transpose.length(), permute.length(), 1e-1)


			Dim toPermute As INDArray = Nd4j.create(Nd4j.linspace(0, 7, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim permuted As INDArray = toPermute.dup().permute(2, 1, 0)
			Dim eq As Boolean = toPermute.Equals(permuted)
			assertNotEquals(toPermute, permuted)

			Dim permuteOther As INDArray = toPermute.permute(1, 2, 0)
			Dim i As Integer = 0
			Do While i < permuteOther.slices()
				Dim toPermutesliceI As INDArray = toPermute.slice(i)
				Dim permuteOtherSliceI As INDArray = permuteOther.slice(i)
				permuteOtherSliceI.ToString()
				assertNotEquals(toPermutesliceI, permuteOtherSliceI)
				i += 1
			Loop
			assertArrayEquals(permuteOther.shape(), toPermute.shape())
			assertNotEquals(toPermute, permuteOther)


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAppendBias(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAppendBias(ByVal backend As Nd4jBackend)
			Dim rand As INDArray = Nd4j.linspace(1, 25, 25, DataType.DOUBLE).reshape(ChrW(1), -1).transpose()
			Dim test As INDArray = Nd4j.appendBias(rand)
			Dim assertion As INDArray = Nd4j.toFlattened(rand, Nd4j.scalar(DataType.DOUBLE, 1.0)).reshape(-1, 1)
			assertEquals(assertion, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRand(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRand(ByVal backend As Nd4jBackend)
			Dim rand As INDArray = Nd4j.randn(5, 5)
			Nd4j.Distributions.createUniform(0.4, 4).sample(5)
			Nd4j.Distributions.createNormal(1, 5).sample(10)
			'Nd4j.getDistributions().createBinomial(5, 1.0).sample(new long[]{5, 5});
			'Nd4j.getDistributions().createBinomial(1, Nd4j.ones(5, 5)).sample(rand.shape());
			Nd4j.Distributions.createNormal(rand, 1).sample(rand.shape())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIdentity(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIdentity(ByVal backend As Nd4jBackend)
			Dim eye As INDArray = Nd4j.eye(5)
			assertTrue(New Long() {5, 5}.SequenceEqual(eye.shape()))
			eye = Nd4j.eye(5)
			assertTrue(New Long() {5, 5}.SequenceEqual(eye.shape()))


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnVectorOpsFortran(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnVectorOpsFortran(ByVal backend As Nd4jBackend)
			Dim twoByTwo As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Long() {2, 2})
			Dim toAdd As INDArray = Nd4j.create(New Single() {1, 2}, New Long() {2, 1})
			twoByTwo.addiColumnVector(toAdd)
			Dim assertion As INDArray = Nd4j.create(New Single() {2, 4, 4, 6}, New Long() {2, 2})
			assertEquals(assertion, twoByTwo)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRSubi(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRSubi(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.ones(2)
			Dim n2Assertion As INDArray = Nd4j.zeros(2)
			Dim nRsubi As INDArray = n2.rsubi(1)
			assertEquals(n2Assertion, nRsubi)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign(ByVal backend As Nd4jBackend)
			Dim vector As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			vector.assign(1)
			assertEquals(Nd4j.ones(5).castTo(DataType.DOUBLE), vector)
			Dim twos As INDArray = Nd4j.ones(2, 2)
			Dim rand As INDArray = Nd4j.rand(2, 2)
			twos.assign(rand)
			assertEquals(rand, twos)

			Dim tensor As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 3, 3)
			Dim ones As INDArray = Nd4j.ones(3, 3, 3).castTo(DataType.DOUBLE)
			assertTrue(tensor.shape().SequenceEqual(ones.shape()))
			ones.assign(tensor)
			assertEquals(tensor, ones)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddScalar(ByVal backend As Nd4jBackend)
			Dim div As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 4.0)
			Dim rdiv As INDArray = div.add(1)
			Dim answer As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 5.0)
			assertEquals(answer, rdiv)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRdivScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRdivScalar(ByVal backend As Nd4jBackend)
			Dim div As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 4.0)
			Dim rdiv As INDArray = div.rdiv(1)
			Dim answer As INDArray = Nd4j.valueArrayOf(New Long() {1, 4}, 0.25)
			assertEquals(rdiv, answer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRDivi(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRDivi(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.valueArrayOf(New Long() {1, 2}, 4.0)
			Dim n2Assertion As INDArray = Nd4j.valueArrayOf(New Long() {1, 2}, 0.5)
			Dim nRsubi As INDArray = n2.rdivi(2)
			assertEquals(n2Assertion, nRsubi)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNumVectorsAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNumVectorsAlongDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			assertEquals(12, arr.vectorsAlongDimension(2))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadCast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadCast(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Dim broadCasted As INDArray = n.broadcast(5, 4)
			Dim i As Integer = 0
			Do While i < broadCasted.rows()
				assertEquals(n, broadCasted.getRow(i))
				i += 1
			Loop

			Dim broadCast2 As INDArray = broadCasted.getRow(0).broadcast(5, 4)
			assertEquals(broadCasted, broadCast2)


			Dim columnBroadcast As INDArray = n.reshape(ChrW(4), 1).broadcast(4, 5)
			i = 0
			Do While i < columnBroadcast.columns()
				assertEquals(columnBroadcast.getColumn(i), n.reshape(ChrW(4)))
				i += 1
			Loop

			Dim fourD As INDArray = Nd4j.create(1, 2, 1, 1)
			Dim broadCasted3 As INDArray = fourD.broadcast(1, 2, 36, 36)
			assertTrue(New Long() {1, 2, 36, 36}.SequenceEqual(broadCasted3.shape()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrix(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {2, 2})
			Dim brr As INDArray = Nd4j.create(New Double() {5, 6}, New Long() {2})
			Dim row As INDArray = arr.getRow(0)
			row.subi(brr)
			assertEquals(Nd4j.create(New Double() {-4, -3}), arr.getRow(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRowGetRowOrdering(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRowGetRowOrdering(ByVal backend As Nd4jBackend)
			Dim row1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim put As INDArray = Nd4j.create(New Double() {5, 6})
			row1.putRow(1, put)

	'        System.out.println(row1);
			row1.ToString()

			Dim row1Fortran As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim putFortran As INDArray = Nd4j.create(New Double() {5, 6})
			row1Fortran.putRow(1, putFortran)
			assertEquals(row1, row1Fortran)
			Dim row1CTest As INDArray = row1.getRow(1)
			Dim row1FortranTest As INDArray = row1Fortran.getRow(1)
			assertEquals(row1CTest, row1FortranTest)



		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumWithRow1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumWithRow1(ByVal backend As Nd4jBackend)
			'Works:
			Dim array2d As INDArray = Nd4j.ones(1, 10)
			array2d.sum(0) 'OK
			array2d.sum(1) 'OK

			Dim array3d As INDArray = Nd4j.ones(1, 10, 10)
			array3d.sum(0) 'OK
			array3d.sum(1) 'OK
			array3d.sum(2) 'java.lang.IllegalArgumentException: Illegal index 100 derived from 9 with offset of 10 and stride of 10

			Dim array4d As INDArray = Nd4j.ones(1, 10, 10, 10)
			Dim sum40 As INDArray = array4d.sum(0) 'OK
			Dim sum41 As INDArray = array4d.sum(1) 'OK
			Dim sum42 As INDArray = array4d.sum(2) 'java.lang.IllegalArgumentException: Illegal index 1000 derived from 9 with offset of 910 and stride of 10
			Dim sum43 As INDArray = array4d.sum(3) 'java.lang.IllegalArgumentException: Illegal index 1000 derived from 9 with offset of 100 and stride of 100

	'        System.out.println("40: " + sum40.length());
	'        System.out.println("41: " + sum41.length());
	'        System.out.println("42: " + sum42.length());
	'        System.out.println("43: " + sum43.length());

			Dim array5d As INDArray = Nd4j.ones(1, 10, 10, 10, 10)
			array5d.sum(0) 'OK
			array5d.sum(1) 'OK
			array5d.sum(2) 'java.lang.IllegalArgumentException: Illegal index 10000 derived from 9 with offset of 9910 and stride of 10
			array5d.sum(3) 'java.lang.IllegalArgumentException: Illegal index 10000 derived from 9 with offset of 9100 and stride of 100
			array5d.sum(4) 'java.lang.IllegalArgumentException: Illegal index 10000 derived from 9 with offset of 1000 and stride of 1000
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumWithRow2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumWithRow2(ByVal backend As Nd4jBackend)
			'All sums in this method execute without exceptions.
			Dim array3d As INDArray = Nd4j.ones(2, 10, 10)
			array3d.sum(0)
			array3d.sum(1)
			array3d.sum(2)

			Dim array4d As INDArray = Nd4j.ones(2, 10, 10, 10)
			array4d.sum(0)
			array4d.sum(1)
			array4d.sum(2)
			array4d.sum(3)

			Dim array5d As INDArray = Nd4j.ones(2, 10, 10, 10, 10)
			array5d.sum(0)
			array5d.sum(1)
			array5d.sum(2)
			array5d.sum(3)
			array5d.sum(4)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRowFortran(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRowFortran(ByVal backend As Nd4jBackend)
			Dim row1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2).castTo(DataType.DOUBLE)
			Dim put As INDArray = Nd4j.create(New Double() {5, 6})
			row1.putRow(1, put)

			Dim row1Fortran As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {2, 4}
			})
			Dim putFortran As INDArray = Nd4j.create(New Double() {5, 6})
			row1Fortran.putRow(1, putFortran)
			assertEquals(row1, row1Fortran)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseOps(ByVal backend As Nd4jBackend)
			Dim n1 As INDArray = Nd4j.scalar(1)
			Dim n2 As INDArray = Nd4j.scalar(2)
			Dim nClone As INDArray = n1.add(n2)
			assertEquals(Nd4j.scalar(3), nClone)
			Dim n1PlusN2 As INDArray = n1.add(n2)
			assertFalse(n1PlusN2.Equals(n1),getFailureMessage(backend))

			Dim n3 As INDArray = Nd4j.scalar(3.0)
			Dim n4 As INDArray = Nd4j.scalar(4.0)
			Dim subbed As INDArray = n4.sub(n3)
			Dim mulled As INDArray = n4.mul(n3)
			Dim div As INDArray = n4.div(n3)

			assertFalse(subbed.Equals(n4))
			assertFalse(mulled.Equals(n4))
			assertEquals(Nd4j.scalar(1.0), subbed)
			assertEquals(Nd4j.scalar(12.0), mulled)
			assertEquals(Nd4j.scalar(1.333333333333333333333), div)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRollAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRollAxis(ByVal backend As Nd4jBackend)
			Dim toRoll As INDArray = Nd4j.ones(3, 4, 5, 6)
			assertArrayEquals(New Long() {3, 6, 4, 5}, Nd4j.rollAxis(toRoll, 3, 1).shape())
			Dim shape As val = Nd4j.rollAxis(toRoll, 3).shape()
			assertArrayEquals(New Long() {6, 3, 4, 5}, shape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorDot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorDot(ByVal backend As Nd4jBackend)
			Dim oneThroughSixty As INDArray = Nd4j.arange(60).reshape("f"c, 3, 4, 5).castTo(DataType.DOUBLE)
			Dim oneThroughTwentyFour As INDArray = Nd4j.arange(24).reshape("f"c, 4, 3, 2).castTo(DataType.DOUBLE)
			Dim result As INDArray = Nd4j.tensorMmul(oneThroughSixty, oneThroughTwentyFour, New Integer()() {
				New Integer() {1, 0},
				New Integer() {0, 1}
			})
			assertArrayEquals(New Long() {5, 2}, result.shape())
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {440.0, 1232.0},
				New Double() {1232.0, 3752.0},
				New Double() {2024.0, 6272.0},
				New Double() {2816.0, 8792.0},
				New Double() {3608.0, 11312.0}
			})
			assertEquals(assertion, result)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeShape(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE)
			Dim reshaped As INDArray = linspace.reshape(ChrW(-1), 2)
			assertArrayEquals(New Long() {2, 2}, reshaped.shape())

			Dim linspace6 As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE)
			Dim reshaped2 As INDArray = linspace6.reshape(ChrW(-1), 3)
			assertArrayEquals(New Long() {2, 3}, reshaped2.shape())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumnGetRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetColumnGetRow(ByVal backend As Nd4jBackend)
			Dim row As INDArray = Nd4j.ones(1, 5)
			For i As Integer = 0 To 4
				Dim col As INDArray = row.getColumn(i)
				assertArrayEquals(col.shape(), New Long() {1})
			Next i

			Dim col As INDArray = Nd4j.ones(5, 1)
			For i As Integer = 0 To 4
				Dim row2 As INDArray = col.getRow(i)
				assertArrayEquals(New Long() {1}, row2.shape())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupAndDupWithOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupAndDupWithOrder(ByVal backend As Nd4jBackend)
			Dim testInputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(4, 5, 123, DataType.DOUBLE)
			Dim count As Integer = 0
			For Each pair As Pair(Of INDArray, String) In testInputs
				Dim msg As String = pair.Second
				Dim [in] As INDArray = pair.First
	'            System.out.println("Count " + count);
				Dim dup As INDArray = [in].dup()
				Dim dupc As INDArray = [in].dup("c"c)
				Dim dupf As INDArray = [in].dup("f"c)

				assertEquals([in], dup,msg)
				assertEquals(dup.ordering(), CChar(Nd4j.order()),msg)
				assertEquals(dupc.ordering(), "c"c,msg)
				assertEquals(dupf.ordering(), "f"c,msg)
				assertEquals([in], dupc,msg)
				assertEquals([in], dupf,msg)
				count += 1
			Next pair
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToOffsetZeroCopy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToOffsetZeroCopy(ByVal backend As Nd4jBackend)
			Dim testInputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(4, 5, 123, DataType.DOUBLE)

			Dim cnt As Integer = 0
			For Each pair As Pair(Of INDArray, String) In testInputs
				Dim msg As String = pair.Second
				Dim [in] As INDArray = pair.First
				Dim dup As INDArray = Shape.toOffsetZeroCopy([in])
				Dim dupc As INDArray = Shape.toOffsetZeroCopy([in], "c"c)
				Dim dupf As INDArray = Shape.toOffsetZeroCopy([in], "f"c)
				Dim dupany As INDArray = Shape.toOffsetZeroCopyAnyOrder([in])

				assertEquals([in], dup,msg & ": " & cnt)
				assertEquals([in], dupc,msg)
				assertEquals([in], dupf,msg)
				assertEquals(dupc.ordering(), "c"c,msg)
				assertEquals(dupf.ordering(), "f"c,msg)
				assertEquals([in], dupany,msg)

				assertEquals(dup.offset(), 0)
				assertEquals(dupc.offset(), 0)
				assertEquals(dupf.offset(), 0)
				assertEquals(dupany.offset(), 0)
				assertEquals(dup.length(), dup.data().length())
				assertEquals(dupc.length(), dupc.data().length())
				assertEquals(dupf.length(), dupf.data().length())
				assertEquals(dupany.length(), dupany.data().length())
				cnt += 1
			Next pair
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace