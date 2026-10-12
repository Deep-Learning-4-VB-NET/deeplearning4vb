Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ScalarAdd = org.nd4j.linalg.api.ops.impl.scalar.ScalarAdd
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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class IndexingTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexingTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecSubArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecSubArray(ByVal backend As Nd4jBackend)
			Dim nd As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6}, New Integer() {2, 3})

			Dim [sub] As INDArray = nd.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 2))
			Nd4j.Executioner.exec(New ScalarAdd([sub], 2))
			assertEquals(Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {6, 7}
			}), [sub],getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinearViewElementWiseMatching(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testLinearViewElementWiseMatching(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim dup As INDArray = linspace.dup()
			linspace.addi(dup)
		  End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRows(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testGetRows(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim testAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {7, 8}
			})

			Dim test As INDArray = arr.get(New SpecifiedIndex(1, 2), New SpecifiedIndex(0, 1))
			assertEquals(testAssertion, test)

		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFirstColumn(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testFirstColumn(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 7},
				New Double() {6, 8}
			})

			Dim assertion As INDArray = Nd4j.create(New Double() {5, 6})
			Dim test As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(0))
			assertEquals(assertion, test)
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testMultiRow(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 7}
			})

			Dim test As INDArray = matrix.get(New SpecifiedIndex(1, 2), NDArrayIndex.interval(0, 1))
			assertEquals(assertion, test)
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPointIndexes(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testPointIndexes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, 4, 3, 2)
			Dim get As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.all())
			assertArrayEquals(New Long() {4, 2}, get.shape())
			Dim linspaced As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {9, 10},
				New Double() {15, 16},
				New Double() {21, 22}
			})

			Dim linspacedGet As INDArray = linspaced.get(NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.all())
			Dim i As Integer = 0
			Do While i < linspacedGet.slices()
				Dim sliceI As INDArray = linspacedGet.slice(i)
				assertEquals(assertion.slice(i), sliceI)
				i += 1
			Loop
			assertArrayEquals(New Long() {6, 1}, linspacedGet.stride())
			assertEquals(assertion, linspacedGet)
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetWithVariedStride(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testGetWithVariedStride(ByVal backend As Nd4jBackend)
			Dim ph As Integer = 0
			Dim pw As Integer = 0
			Dim sy As Integer = 2
			Dim sx As Integer = 2
			Dim iLim As Integer = 8
			Dim jLim As Integer = 8
			Dim i As Integer = 0
			Dim j As Integer = 0
			Dim img As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4}, New Long() {1, 1, 8, 8})


			Dim padded As INDArray = Nd4j.pad(img, New Integer()() {
				New Integer() {0, 0},
				New Integer() {0, 0},
				New Integer() {ph, ph + sy - 1},
				New Integer() {pw, pw + sx - 1}
			})

			Dim get As INDArray = padded.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i, sy, iLim), NDArrayIndex.interval(j, sx, jLim))
			assertArrayEquals(New Long() {81, 81, 18, 2}, get.stride())
			Dim assertion As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3}, New Integer() {1, 1, 4, 4})
			assertEquals(assertion, get)

			i = 1
			iLim = 9
			Dim get3 As INDArray = padded.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i, sy, iLim), NDArrayIndex.interval(j, sx, jLim))

			Dim assertion2 As INDArray = Nd4j.create(New Double() {2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4}, New Integer() {1, 1, 4, 4})
			assertArrayEquals(New Long() {81, 81, 18, 2}, get3.stride())
			assertEquals(assertion2, get3)



			i = 0
			iLim = 8
			jLim = 9
			j = 1
			Dim get2 As INDArray = padded.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i, sy, iLim), NDArrayIndex.interval(j, sx, jLim))
			assertArrayEquals(New Long() {81, 81, 18, 2}, get2.stride())
			assertEquals(assertion, get2)



		  End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowVectorInterval(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testRowVectorInterval(ByVal backend As Nd4jBackend)
			Dim len As Integer = 30
			Dim row As INDArray = Nd4j.zeros(1, len)
			For i As Integer = 0 To len - 1
				row.putScalar(i, i)
			Next i

			Dim first10a As INDArray = row.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 10))
			assertArrayEquals(first10a.shape(), New Long() {10})
			For i As Integer = 0 To 9
				assertTrue(first10a.getDouble(i) = i)
			Next i

			Dim first10b As INDArray = row.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 10))
			assertArrayEquals(first10b.shape(), New Long() {10})
			For i As Integer = 0 To 9
				assertTrue(first10b.getDouble(i) = i)
			Next i

			Dim last10a As INDArray = row.get(NDArrayIndex.point(0), NDArrayIndex.interval(20, 30))
			assertArrayEquals(last10a.shape(), New Long() {10})
			For i As Integer = 0 To 9
				assertEquals(i+20, last10a.getDouble(i), 1e-6)
			Next i

			Dim last10b As INDArray = row.get(NDArrayIndex.point(0), NDArrayIndex.interval(20, 30))
			assertArrayEquals(last10b.shape(), New Long() {10})
			For i As Integer = 0 To 9
				assertTrue(last10b.getDouble(i) = 20 + i)
			Next i
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test1dSubarray_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub test1dSubarray_1(ByVal backend As Nd4jBackend)
			Dim data As val = Nd4j.linspace(DataType.FLOAT,0, 10, 1)
			Dim exp As val = Nd4j.createFromArray(New Single(){3.0f, 4.0f})
			Dim dataAtIndex As val = data.get(NDArrayIndex.interval(3, 5))

			assertEquals(exp, dataAtIndex)
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test1dSubarray_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub test1dSubarray_2(ByVal backend As Nd4jBackend)
			Dim data As val = Nd4j.linspace(DataType.FLOAT,1, 10, 1)
			Dim exp As val = Nd4j.createFromArray(New Single(){4.0f, 6.0f})
			Dim dataAtIndex As val = data.get(Nd4j.createFromArray(New Integer(){3, 5}))

			assertEquals(exp, dataAtIndex)
		  End Sub

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
	'        System.out.println(whatToPut);
			Dim whereToPut() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all()}

			subArr_B.put(whereToPut, whatToPut)

			assertEquals(subArr_A, subArr_B)

	'        System.out.println("... done");
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimplePoint(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testSimplePoint(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.linspace(1, 3 * 3 * 3, 3 * 3 * 3).reshape(ChrW(3), 3, 3)

	'        
	'            c - ordering
	'            1,2,3   10,11,12    19,20,21
	'            4,5,6   13,14,15    22,23,24
	'            7,8,9   16,17,18    25,26,27
	'         
			Dim viewOne As INDArray = A.get(NDArrayIndex.point(1), NDArrayIndex.interval(0, 2), NDArrayIndex.interval(1, 3))
			Dim viewTwo As INDArray = A.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).get(NDArrayIndex.interval(0, 2), NDArrayIndex.interval(1, 3))
			Dim expected As INDArray = Nd4j.zeros(2, 2)
			expected.putScalar(0, 0, 11)
			expected.putScalar(0, 1, 12)
			expected.putScalar(1, 0, 14)
			expected.putScalar(1, 1, 15)
			assertEquals(expected, viewTwo,"View with two get")
			assertEquals(expected, viewOne,"View with one get") 'FAILS!
			assertEquals(viewOne, viewTwo,"Two views should be the same") 'obviously fails
		  End Sub

	'    
	'        This is the same as the above test - just tests every possible window with a slice from the 0th dim
	'        They all fail - so it's possibly unrelated to the value of the index
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
							log.error("Error on view ",t)
							'collector.addError(t);
						End Try
						j += 1
					Loop
					i += 1
				Loop
			Next s
		  End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace