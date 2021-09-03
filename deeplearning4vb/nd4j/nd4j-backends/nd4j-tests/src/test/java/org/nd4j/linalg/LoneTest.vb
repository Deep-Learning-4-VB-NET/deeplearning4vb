Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports Tanh = org.nd4j.linalg.api.ops.impl.transforms.strict.Tanh
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class LoneTest extends BaseNd4jTestWithBackends
	Public Class LoneTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxStability(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxStability(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.create(New Double(){-0.75, 0.58, 0.42, 1.03, -0.61, 0.19, -0.37, -0.40, -1.42, -0.04}).reshape(ChrW(1), -1).transpose()
	'        System.out.println("Input transpose " + Shape.shapeToString(input.shapeInfo()));
			Dim output As INDArray = Nd4j.create(DataType.DOUBLE, 10, 1)
	'        System.out.println("Element wise stride of output " + output.elementWiseStride());
			Nd4j.Executioner.exec(New SoftMax(input, output))
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenedView(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenedView(ByVal backend As Nd4jBackend)
			Dim rows As Integer = 8
			Dim cols As Integer = 8
			Dim dim2 As Integer = 4
			Dim length As Integer = rows * cols
			Dim length3d As Integer = rows * cols * dim2

			Dim first As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, rows, cols)
			Dim second As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){rows, cols}, "f"c).assign(first)
			Dim third As INDArray = Nd4j.linspace(1, length3d, length3d).reshape("c"c, rows, cols, dim2)
			first.addi(0.1)
			second.addi(0.2)
			third.addi(0.3)

			first = first.get(NDArrayIndex.interval(4, 8), NDArrayIndex.interval(0, 2, 8))
			Dim i As Integer = 0
			Do While i < first.tensorsAlongDimension(0)
	'            System.out.println(first.tensorAlongDimension(i, 0));
				first.tensorAlongDimension(i, 0)
				i += 1
			Loop

			i = 0
			Do While i < first.tensorsAlongDimension(1)
	'            System.out.println(first.tensorAlongDimension(i, 1));
				first.tensorAlongDimension(i, 1)
				i += 1
			Loop
			second = second.get(NDArrayIndex.interval(3, 7), NDArrayIndex.all())
			third = third.permute(0, 2, 1)

			Dim cAssertion As INDArray = Nd4j.create(New Double(){33.10, 35.10, 37.10, 39.10, 41.10, 43.10, 45.10, 47.10, 49.10, 51.10, 53.10, 55.10, 57.10, 59.10, 61.10, 63.10})
			Dim fAssertion As INDArray = Nd4j.create(New Double(){33.10, 41.10, 49.10, 57.10, 35.10, 43.10, 51.10, 59.10, 37.10, 45.10, 53.10, 61.10, 39.10, 47.10, 55.10, 63.10})
			assertEquals(cAssertion, Nd4j.toFlattened("c"c, first))
			assertEquals(fAssertion, Nd4j.toFlattened("f"c, first))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexingColVec(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexingColVec(ByVal backend As Nd4jBackend)
			Dim elements As Integer = 5
			Dim rowVector As INDArray = Nd4j.linspace(1, elements, elements).reshape(ChrW(1), elements)
			Dim colVector As INDArray = rowVector.transpose()
			Dim j As Integer
			Dim jj As INDArray
			For i As Integer = 0 To elements - 1
				j = i + 1
				assertEquals(i + 1,colVector.getRow(i).getInt(0))
				assertEquals(i + 1,rowVector.getColumn(i).getInt(0))
				assertEquals(i + 1,rowVector.get(NDArrayIndex.point(0), NDArrayIndex.interval(i, j)).getInt(0))
				assertEquals(i + 1,colVector.get(NDArrayIndex.interval(i, j), NDArrayIndex.point(0)).getInt(0))
	'            System.out.println("Making sure index interval will not crash with begin/end vals...");
				jj = colVector.get(NDArrayIndex.interval(i, i + 1))
				jj = colVector.get(NDArrayIndex.interval(i, i + 1))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void concatScalarVectorIssue(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub concatScalarVectorIssue(ByVal backend As Nd4jBackend)
			'A bug was found when the first array that concat sees is a scalar and the rest vectors + scalars
			Dim arr1 As INDArray = Nd4j.create(1, 1)
			Dim arr2 As INDArray = Nd4j.create(1, 8)
			Dim arr3 As INDArray = Nd4j.create(1, 1)
			Dim arr4 As INDArray = Nd4j.concat(1, arr1, arr2, arr3)
			assertTrue(arr4.sumNumber().floatValue() <= Nd4j.EPS_THRESHOLD)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reshapeTensorMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub reshapeTensorMmul(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 2, 12).reshape(ChrW(2), 3, 2)
			Dim b As INDArray = Nd4j.linspace(3, 4, 4).reshape(ChrW(2), 2)
			Dim axes(1)() As Integer
			axes(0) = New Integer(){0, 1}
			axes(1) = New Integer(){0, 2}

			'this was throwing an exception
			Dim c As INDArray = Nd4j.tensorMmul(b, a, axes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void maskWhenMerge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub maskWhenMerge(ByVal backend As Nd4jBackend)
			Dim dsA As New DataSet(Nd4j.linspace(1, 15, 15).reshape(ChrW(1), 3, 5), Nd4j.zeros(1, 3, 5))
			Dim dsB As New DataSet(Nd4j.linspace(1, 9, 9).reshape(ChrW(1), 3, 3), Nd4j.zeros(1, 3, 3))
			Dim dataSetList As IList(Of DataSet) = New List(Of DataSet)()
			dataSetList.Add(dsA)
			dataSetList.Add(dsB)
			Dim fullDataSet As DataSet = DataSet.merge(dataSetList)
			assertTrue(fullDataSet.FeaturesMaskArray IsNot Nothing)

			Dim fullDataSetCopy As DataSet = fullDataSet.copy()
			assertTrue(fullDataSetCopy.FeaturesMaskArray IsNot Nothing)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRelu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRelu(ByVal backend As Nd4jBackend)
			Dim aA As INDArray = Nd4j.linspace(-3, 4, 8).reshape(ChrW(2), 4)
			Dim aD As INDArray = Nd4j.linspace(-3, 4, 8).reshape(ChrW(2), 4)
			Dim b As INDArray = Nd4j.Executioner.exec(New Tanh(aA))
			'Nd4j.getExecutioner().execAndReturn(new TanhDerivative(aD));
	'        System.out.println(aA);
	'        System.out.println(aD);
	'        System.out.println(b);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMax(ByVal backend As Nd4jBackend)
			Dim max As Integer = 63
			Dim A As INDArray = Nd4j.linspace(1, max, max).reshape(ChrW(1), max)
			Dim currentArgMax As Integer = Nd4j.argMax(A).getInt(0)
			assertEquals(max - 1, currentArgMax)

			max = 64
			A = Nd4j.linspace(1, max, max).reshape(ChrW(1), max)
			currentArgMax = Nd4j.argMax(A).getInt(0)
	'        System.out.println("Returned argMax is " + currentArgMax);
			assertEquals(max - 1, currentArgMax)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRPF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRPF(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.createFromArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12).reshape(2, 2, 3)

			log.info("--------")

			Dim tad As val = array.tensorAlongDimension(1, 1, 2)
			Nd4j.exec(New PrintVariable(tad, False))
			log.info("TAD native shapeInfo: {}", tad.shapeInfoDataBuffer().asLong())
			log.info("TAD Java shapeInfo: {}", tad.shapeInfoJava())
			log.info("TAD:" & vbLf & "{}", tad)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3D_Vstack_C(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3D_Vstack_C(ByVal backend As Nd4jBackend)
			Dim shape As val = New Long(){1, 1000, 20}

			Dim cArrays As IList(Of INDArray) = New List(Of INDArray)()
			Dim fArrays As IList(Of INDArray) = New List(Of INDArray)()

			For e As Integer = 0 To 31
				Dim arr As val = Nd4j.create(DataType.FLOAT, shape, "c"c).assign(e)
				cArrays.Add(arr)
				'            fArrays.add(cOrder.dup('f'));
			Next e

			Nd4j.Executioner.commit()

			Dim time1 As val = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As val = Nd4j.vstack(cArrays)
			Dim time2 As val = DateTimeHelper.CurrentUnixTimeMillis()

	'        log.info("Time spent: {} ms", time2 - time1);

			For e As Integer = 0 To 31
				Dim tad As val = res.tensorAlongDimension(e, 1, 2)

				assertEquals(CDbl(e), tad.meanNumber().doubleValue(), 1e-5,"Failed for TAD [" & e & "]")
				assertEquals(CDbl(e), tad.getDouble(0), 1e-5)
			Next e
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testGetRow1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRow1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(10000, 10000)

			'Thread.sleep(10000);

			Dim numTries As Integer = 1000
			Dim times As IList(Of Long) = New List(Of Long)()
			Dim time As Long = 0
			For i As Integer = 0 To numTries - 1

				Dim idx As Integer = RandomUtils.nextInt(0, 10000)
				Dim time1 As Long = System.nanoTime()
				array.getRow(idx)
				Dim time2 As Long = System.nanoTime() - time1

				times.Add(time2)
				time += time2
			Next i

			time \= numTries

			times.Sort()

	'        log.info("p50: {}; avg: {};", times.get(times.size() / 2), time);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkIllegalElementOps(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkIllegalElementOps(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim A As INDArray = Nd4j.linspace(1, 20, 20).reshape(ChrW(4), 5)
			Dim B As INDArray = A.dup().reshape(ChrW(2), 2, 5)
			Dim C As INDArray = A.mul(B)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkSliceofSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkSliceofSlice(ByVal backend As Nd4jBackend)
	'        
	'            Issue 1: Slice of slice with c order and f order views are not equal
	'
	'            Comment out assert and run then -> Issue 2: Index out of bound exception with certain shapes when accessing elements with getDouble() in f order
	'            (looks like problem is when rank-1==1) eg. 1,2,1 and 2,2,1
	'         
			Dim ranksToCheck() As Integer = {2, 3, 4, 5}
			For rank As Integer = 0 To ranksToCheck.Length - 1
	'            log.info("\nRunning through rank " + ranksToCheck[rank]);
				Dim allF As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getTestMatricesWithVaryingShapes(ranksToCheck(rank), "f"c, DataType.FLOAT)
				Dim iter As IEnumerator(Of Pair(Of INDArray, String)) = allF.GetEnumerator()
				Do While iter.MoveNext()
					Dim currentPair As Pair(Of INDArray, String) = iter.Current
					Dim origArrayF As INDArray = currentPair.First
					Dim sameArrayC As INDArray = origArrayF.dup("c"c)
	'                log.info("\nLooping through slices for shape " + currentPair.getSecond());
	'                log.info("\nOriginal array:\n" + origArrayF);
					origArrayF.ToString()
					Dim viewF As INDArray = origArrayF.slice(0)
					Dim viewC As INDArray = sameArrayC.slice(0)
	'                log.info("\nSlice 0, C order:\n" + viewC.toString());
	'                log.info("\nSlice 0, F order:\n" + viewF.toString());
					viewC.ToString()
					viewF.ToString()
					Dim i As Integer = 0
					Do While i < viewF.slices()
						'assertEquals(viewF.slice(i),viewC.slice(i));
						Dim j As Integer = 0
						Do While j < viewF.slice(i).length()
							'if (j>0) break;
	'                        log.info("\nC order slice " + i + ", element 0 :" + viewC.slice(i).getDouble(j)); //C order is fine
	'                        log.info("\nF order slice " + i + ", element 0 :" + viewF.slice(i).getDouble(j)); //throws index out of bound err on F order
							viewC.slice(i).getDouble(j)
							viewF.slice(i).getDouble(j)
							j += 1
						Loop
						i += 1
					Loop
				Loop
			Next rank
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkWithReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkWithReshape(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(1, 3)
			Dim reshaped As INDArray = arr.reshape("f"c, 3, 1)
			Dim i As Integer=0
			Do While i<reshaped.length()
	'            log.info("C order element " + i + arr.getDouble(i));
	'            log.info("F order element " + i + reshaped.getDouble(i));
				arr.getDouble(i)
				reshaped.getDouble(i)
				i += 1
			Loop
			Dim j As Integer=0
			Do While j<arr.slices()
				Dim k As Integer = 0
				Do While k < arr.slice(j).length()
	'                log.info("\nArr: slice " + j + " element " + k + " " + arr.slice(j).getDouble(k));
					arr.slice(j).getDouble(k)
					k += 1
				Loop
				j += 1
			Loop
			j = 0
			Do While j < reshaped.slices()
				Dim k As Integer = 0
				Do While k < reshaped.slice(j).length()
	'                log.info("\nReshaped: slice " + j + " element " + k + " " + reshaped.slice(j).getDouble(k));
					reshaped.slice(j).getDouble(k)
					k += 1
				Loop
				j += 1
			Loop
		End Sub
	End Class

End Namespace