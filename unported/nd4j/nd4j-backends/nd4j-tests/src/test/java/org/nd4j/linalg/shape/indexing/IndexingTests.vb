Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports SpecifiedIndex = org.nd4j.linalg.indexing.SpecifiedIndex
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

Namespace org.nd4j.linalg.shape.indexing

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class IndexingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexingTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGet(ByVal backend As Nd4jBackend)
	'        System.out.println("Testing sub-array put and get with a 3D array ...");

			Dim arr As INDArray = Nd4j.linspace(0, 124, 125).reshape(ChrW(5), 5, 5)

	'        
	'         * Extract elements with the following indices:
	'         *
	'         * (2,1,1) (2,1,2) (2,1,3)
	'         * (2,2,1) (2,2,2) (2,2,3)
	'         * (2,3,1) (2,3,2) (2,3,3)
	'         

			Dim slice As Integer = 2

			Dim iStart As Integer = 1
			Dim jStart As Integer = 1

			Dim iEnd As Integer = 4
			Dim jEnd As Integer = 4

			' Method A: Element-wise.

			Dim subArr_A As INDArray = Nd4j.create(New Integer() {3, 3})

			For i As Integer = iStart To iEnd - 1
				For j As Integer = jStart To jEnd - 1

					Dim val As Double = arr.getDouble(slice, i, j)
					Dim [sub]() As Integer = {i - iStart, j - jStart}

					subArr_A.putScalar([sub], val)

				Next j
			Next i

			' Method B: Using NDArray get and put with index classes.

			Dim subArr_B As INDArray = Nd4j.create(New Integer() {3, 3})

			Dim ndi_Slice As INDArrayIndex = NDArrayIndex.point(slice)
			Dim ndi_J As INDArrayIndex = NDArrayIndex.interval(jStart, jEnd)
			Dim ndi_I As INDArrayIndex = NDArrayIndex.interval(iStart, iEnd)

			Dim whereToGet() As INDArrayIndex = {ndi_Slice, ndi_I, ndi_J}

			Dim whatToPut As INDArray = arr.get(whereToGet)
			assertEquals(subArr_A, whatToPut)
	'        System.out.println(whatToPut);
			Dim whereToPut() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all()}

			subArr_B.put(whereToPut, whatToPut)

			assertEquals(subArr_A, subArr_B)
	'        System.out.println("... done");
		End Sub

	'    
	'        Simple test that checks indexing through different ways that fails
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimplePoint(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testSimplePoint(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.linspace(1, 3 * 3 * 3, 3 * 3 * 3).reshape(ChrW(3), 3, 3)

	'        
	'            f - ordering
	'            1,10,19   2,11,20   3,12,21
	'            4,13,22   5,14,23   6,15,24
	'            7,16,25   8,17,26   9,18,27
	'        
	'            subsetting the
	'                11,20
	'                14,24 ndarray
	'        
	'         
			Dim viewOne As INDArray = A.get(NDArrayIndex.point(1), NDArrayIndex.interval(0, 2), NDArrayIndex.interval(1, 3))
			Dim viewTwo As INDArray = A.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).get(NDArrayIndex.interval(0, 2), NDArrayIndex.interval(1, 3))
			Dim expected As INDArray = Nd4j.zeros(2, 2)
			expected.putScalar(0, 0, 11)
			expected.putScalar(0, 1, 20)
			expected.putScalar(1, 0, 14)
			expected.putScalar(1, 1, 23)
			assertEquals(expected, viewTwo,"View with two get")
			assertEquals(expected, viewOne,"View with one get") 'FAILS!
			assertEquals(viewOne, viewTwo,"Two views should be the same") 'Obviously fails
		 End Sub

	'    
	'    This is the same as the above test - just tests every possible window with a slice from the 0th dim
	'    They all fail - so it's possibly unrelated to the value of the index
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPointIndexing(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testPointIndexing(ByVal backend As Nd4jBackend)
			Dim slices As Integer = 5
			Dim rows As Integer = 5
			Dim cols As Integer = 5
			Dim l As Integer = slices * rows * cols
			Dim A As INDArray = Nd4j.linspace(1, l, l).reshape(ChrW(slices), rows, cols)

			For s As Integer = 0 To slices - 1
				Dim ndi_Slice As INDArrayIndex = NDArrayIndex.point(s)
				Dim i As Integer = 0
				Do While i < rows
					Dim j As Integer = 0
					Do While j < cols
	'                    log.info("Running for ( {}, {} - {} , {} - {} )", s, i, rows, j, cols);
						Dim ndi_I As INDArrayIndex = NDArrayIndex.interval(i, rows)
						Dim ndi_J As INDArrayIndex = NDArrayIndex.interval(j, cols)
						Dim aView As INDArray = A.get(ndi_Slice, NDArrayIndex.all(), NDArrayIndex.all()).get(ndi_I, ndi_J)
						Dim sameView As INDArray = A.get(ndi_Slice, ndi_I, ndi_J)
						Dim failureMessage As String = String.Format("Fails for ({0:D} , {1:D} - {2:D}, {3:D} - {4:D})" & vbLf, s, i, rows, j, cols)
						Try
							assertEquals(aView, sameView,failureMessage)
						Catch t As Exception
							log.error("Error with view",t)
						End Try
						j += 1
					Loop
					i += 1
				Loop
			Next s
		 End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testTensorGet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorGet(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 2, 2)
	'        
	'        * [[[  1.,   7.],
	'        [  4.,  10.]],
	'        
	'        [[  2.,   8.],
	'        [  5.,  11.]],
	'        
	'        [[  3.,   9.],
	'        [  6.,  12.]]])
	'        

			Dim firstAssertion As INDArray = Nd4j.create(New Double() {1, 7})
			Dim firstTest As INDArray = threeTwoTwo.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all())
			assertEquals(firstAssertion, firstTest)
			Dim secondAssertion As INDArray = Nd4j.create(New Double() {3, 9})
			Dim secondTest As INDArray = threeTwoTwo.get(NDArrayIndex.point(2), NDArrayIndex.point(0), NDArrayIndex.all())
			assertEquals(secondAssertion, secondTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void concatGetBug(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub concatGetBug(ByVal backend As Nd4jBackend)
			Dim width As Integer = 5
			Dim height As Integer = 4
			Dim depth As Integer = 3
			Dim nExamples1 As Integer = 2
			Dim nExamples2 As Integer = 1

			Dim length1 As Integer = width * height * depth * nExamples1
			Dim length2 As Integer = width * height * depth * nExamples2

			Dim first As INDArray = Nd4j.linspace(1, length1, length1).reshape("c"c, nExamples1, depth, width, height)
			Dim second As INDArray = Nd4j.linspace(1, length2, length2).reshape("c"c, nExamples2, depth, width, height).addi(0.1)

			Dim fMerged As INDArray = Nd4j.concat(0, first, second)

			assertEquals(first, fMerged.get(NDArrayIndex.interval(0, nExamples1), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))

			Dim get As INDArray = fMerged.get(NDArrayIndex.interval(nExamples1, nExamples1 + nExamples2), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			assertEquals(second, get.dup()) 'Passes
			assertEquals(second, get) 'Fails
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testShape(ByVal backend As Nd4jBackend)
			Dim ndarray As INDArray = Nd4j.create(New Single()() {
				New Single() {1f, 2f},
				New Single() {3f, 4f}
			})
			Dim subarray As INDArray = ndarray.get(NDArrayIndex.point(0), NDArrayIndex.all())
			assertTrue(subarray.RowVector)
			Dim shape As val = subarray.shape()
			assertArrayEquals(New Long(){2}, shape)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRows(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testGetRows(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim testAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 8},
				New Double() {6, 9}
			})

			Dim test As INDArray = arr.get(New SpecifiedIndex(1, 2), New SpecifiedIndex(1, 2))
			assertEquals(testAssertion, test)

		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFirstColumn(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testFirstColumn(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 6},
				New Double() {7, 8}
			})

			Dim assertion As INDArray = Nd4j.create(New Double() {5, 7})
			Dim test As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(0))
			assertEquals(assertion, test)
		 End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinearIndex(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testLinearIndex(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			For i As Integer = 0 To linspace.length() - 1
				assertEquals(i + 1, linspace.getDouble(i), 1e-1)
			Next i
		 End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace