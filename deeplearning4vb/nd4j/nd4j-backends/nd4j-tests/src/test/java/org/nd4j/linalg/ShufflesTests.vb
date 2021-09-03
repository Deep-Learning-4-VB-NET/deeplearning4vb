Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.RNG) public class ShufflesTests extends BaseNd4jTestWithBackends
	Public Class ShufflesTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimpleShuffle1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleShuffle1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(10, 10)
			For x As Integer = 0 To 9
				array.getRow(x).assign(x)
			Next x

	'        System.out.println(array);

			Dim scanner As New OrderScanner2D(array)

			assertArrayEquals(New Single() {0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f}, scanner.Map, 0.01f)

			Nd4j.shuffle(array, 1)

	'        System.out.println(array);

			ArrayUtil.argMin(New Integer() {})

			assertTrue(scanner.compareRow(array))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimpleShuffle2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleShuffle2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(10, 10)
			For x As Integer = 0 To 9
				array.getColumn(x).assign(x)
			Next x
	'        System.out.println(array);

			Dim scanner As New OrderScanner2D(array)
			assertArrayEquals(New Single() {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, scanner.Map, 0.01f)
			Nd4j.shuffle(array, 0)
	'        System.out.println(array);
			assertTrue(scanner.compareColumn(array))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimpleShuffle3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleShuffle3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(11, 10)
			For x As Integer = 0 To 10
				array.getRow(x).assign(x)
			Next x

	'        System.out.println(array);
			Dim scanner As New OrderScanner2D(array)

			assertArrayEquals(New Single() {0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f}, scanner.Map, 0.01f)
			Nd4j.shuffle(array, 1)
	'        System.out.println(array);
			assertTrue(scanner.compareRow(array))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSymmetricShuffle1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSymmetricShuffle1(ByVal backend As Nd4jBackend)
			Dim features As INDArray = Nd4j.zeros(10, 10)
			Dim labels As INDArray = Nd4j.zeros(10, 3)
			For x As Integer = 0 To 9
				features.getRow(x).assign(x)
				labels.getRow(x).assign(x)
			Next x
	'        System.out.println(features);

			Dim scanner As New OrderScanner2D(features)

			assertArrayEquals(New Single() {0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f}, scanner.Map, 0.01f)

			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			list.Add(features)
			list.Add(labels)

			Nd4j.shuffle(list, 1)

	'        System.out.println(features);
	'        System.out.println();
	'        System.out.println(labels);

			ArrayUtil.argMin(New Integer() {})

			assertTrue(scanner.compareRow(features))

			For x As Integer = 0 To 9
				Dim val As Double = features.getRow(x).getDouble(0)
				Dim row As INDArray = labels.getRow(x)

				For y As Integer = 0 To row.length() - 1
					assertEquals(val, row.getDouble(y), 0.001)
				Next y
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSymmetricShuffle2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSymmetricShuffle2(ByVal backend As Nd4jBackend)
			Dim features As INDArray = Nd4j.zeros(10, 10, 20)
			Dim labels As INDArray = Nd4j.zeros(10, 10, 3)

			For x As Integer = 0 To 9
				features.slice(x).assign(x)
				labels.slice(x).assign(x)
			Next x

	'        System.out.println(features);

			Dim scannerFeatures As New OrderScanner3D(features)
			Dim scannerLabels As New OrderScanner3D(labels)

			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			list.Add(features)
			list.Add(labels)

			Nd4j.shuffle(list, 1, 2)

	'        System.out.println(features);
	'        System.out.println("------------------");
	'        System.out.println(labels);

			assertTrue(scannerFeatures.compareSlice(features))
			assertTrue(scannerLabels.compareSlice(labels))

			For x As Integer = 0 To 9
				Dim val As Double = features.slice(x).getDouble(0)
				Dim row As INDArray = labels.slice(x)

				For y As Integer = 0 To row.length() - 1
					assertEquals(val, row.getDouble(y), 0.001)
				Next y
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSymmetricShuffle3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSymmetricShuffle3(ByVal backend As Nd4jBackend)
			Dim features As INDArray = Nd4j.zeros(10, 10, 20)
			Dim featuresMask As INDArray = Nd4j.zeros(10, 20)
			Dim labels As INDArray = Nd4j.zeros(10, 10, 3)
			Dim labelsMask As INDArray = Nd4j.zeros(10, 3)

			For x As Integer = 0 To 9
				features.slice(x).assign(x)
				featuresMask.slice(x).assign(x)
				labels.slice(x).assign(x)
				labelsMask.slice(x).assign(x)
			Next x

			Dim scannerFeatures As New OrderScanner3D(features)
			Dim scannerLabels As New OrderScanner3D(labels)
			Dim scannerFeaturesMask As New OrderScanner3D(featuresMask)
			Dim scannerLabelsMask As New OrderScanner3D(labelsMask)


			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			arrays.Add(features)
			arrays.Add(labels)
			arrays.Add(featuresMask)
			arrays.Add(labelsMask)

			Dim dimensions As IList(Of Integer()) = New List(Of Integer())()
			dimensions.Add(ArrayUtil.range(1, features.rank()))
			dimensions.Add(ArrayUtil.range(1, labels.rank()))
			dimensions.Add(ArrayUtil.range(1, featuresMask.rank()))
			dimensions.Add(ArrayUtil.range(1, labelsMask.rank()))

			Nd4j.shuffle(arrays, New Random(11), dimensions)

			assertTrue(scannerFeatures.compareSlice(features))
			assertTrue(scannerLabels.compareSlice(labels))
			assertTrue(scannerFeaturesMask.compareSlice(featuresMask))
			assertTrue(scannerLabelsMask.compareSlice(labelsMask))


			For x As Integer = 0 To 9
				Dim val As Double = features.slice(x).getDouble(0)
				Dim sliceLabels As INDArray = labels.slice(x)
				Dim sliceLabelsMask As INDArray = labelsMask.slice(x)
				Dim sliceFeaturesMask As INDArray = featuresMask.slice(x)

				For y As Integer = 0 To sliceLabels.length() - 1
					assertEquals(val, sliceLabels.getDouble(y), 0.001)
				Next y

				For y As Integer = 0 To sliceLabelsMask.length() - 1
					assertEquals(val, sliceLabelsMask.getDouble(y), 0.001)
				Next y

				For y As Integer = 0 To sliceFeaturesMask.length() - 1
					assertEquals(val, sliceFeaturesMask.getDouble(y), 0.001)
				Next y
			Next x
		End Sub


		''' <summary>
		''' There's SMALL chance this test will randomly fail, since spread isn't too big </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHalfVectors1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHalfVectors1(ByVal backend As Nd4jBackend)
			Dim array1() As Integer = ArrayUtil.buildHalfVector(New Random(12), 20)
			Dim array2() As Integer = ArrayUtil.buildHalfVector(New Random(75), 20)

			assertFalse(array1.SequenceEqual(array2))

			assertEquals(20, array1.Length)
			assertEquals(20, array2.Length)

			For i As Integer = 0 To array1.Length - 1
				If i >= array1.Length \ 2 Then
					assertEquals(-1, array1(i),"Failed on element [" & i & "]")
					assertEquals(-1, array2(i),"Failed on element [" & i & "]")
				Else
					assertNotEquals(-1, array1(i),"Failed on element [" & i & "]")
					assertNotEquals(-1, array2(i),"Failed on element [" & i & "]")
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInterleavedVector1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInterleavedVector1(ByVal backend As Nd4jBackend)
			Dim array1() As Integer = ArrayUtil.buildInterleavedVector(New Random(12), 20)
			Dim array2() As Integer = ArrayUtil.buildInterleavedVector(New Random(75), 20)

			assertFalse(array1.SequenceEqual(array2))

			assertEquals(20, array1.Length)
			assertEquals(20, array2.Length)

			For i As Integer = 0 To array1.Length - 1
				If i Mod 2 <> 0 Then
					assertEquals(-1, array1(i),"Failed on element [" & i & "]")
					assertEquals(-1, array2(i),"Failed on element [" & i & "]")
				Else
					assertNotEquals(-1, array1(i),"Failed on element [" & i & "]")
					assertNotEquals(-1, array2(i),"Failed on element [" & i & "]")
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInterleavedVector3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInterleavedVector3(ByVal backend As Nd4jBackend)
			For e As Integer = 0 To 999
				Dim length As Integer = e + 256 'RandomUtils.nextInt(121, 2073);
				Dim array1() As Integer = ArrayUtil.buildInterleavedVector(New Random(DateTimeHelper.CurrentUnixTimeMillis()), length)
				Dim set As val = New HashSet(Of Integer)()

				For i As Integer = 0 To length - 1
					Dim v As val = array1(i)

					' skipping passive swap step
					If v < 0 Then
						Continue For
					End If

					' checking that each swap pair is unique
					If set.contains(Convert.ToInt32(v)) Then
						Throw New System.InvalidOperationException("Duplicate found")
					End If

					set.add(Convert.ToInt32(v))

					' checking that each swap pair is unidirectional
					assertEquals(-1, array1(v))
				Next i

				' set should have half of length defined
				assertTrue(set.size() >= length \ 2 - 1)
			Next e
		End Sub


		Public Class OrderScanner3D
			Friend map() As Single

			Public Sub New(ByVal data As INDArray)
				map = measureState(data)
			End Sub

			Public Overridable Function measureState(ByVal data As INDArray) As Single()
				' for 3D we save 0 element for each slice.
				Dim result(CInt(data.shape()(0)) - 1) As Single

				Dim x As Integer = 0
				Do While x < data.shape()(0)
					result(x) = data.slice(x).getFloat(0)
					x += 1
				Loop

				Return result
			End Function

			Public Overridable Function compareSlice(ByVal data As INDArray) As Boolean
				Dim newMap() As Single = measureState(data)

				If newMap.Length <> map.Length Then
					Console.WriteLine("Different map lengths")
					Return False
				End If

				If map.SequenceEqual(newMap) Then
	'                System.out.println("Maps are equal");
					Return False
				End If

				Dim x As Integer = 0
				Do While x < data.shape()(0)
					Dim slice As INDArray = data.slice(x)

					For y As Integer = 0 To slice.length() - 1
						If Math.Abs(slice.getFloat(y) - newMap(x)) > Nd4j.EPS_THRESHOLD Then
							Console.Write("Different data in a row")
							Return False
						End If
					Next y
					x += 1
				Loop


				Return True
			End Function
		End Class


		Public Class OrderScanner2D
'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend map_Conflict() As Single

			Public Sub New(ByVal data As INDArray)
				map_Conflict = measureState(data)
			End Sub

			Public Overridable Function measureState(ByVal data As INDArray) As Single()
				Dim result(data.rows() - 1) As Single

				Dim x As Integer = 0
				Do While x < data.rows()
					result(x) = data.getRow(x).getFloat(0)
					x += 1
				Loop

				Return result
			End Function

			Public Overridable Function compareRow(ByVal newData As INDArray) As Boolean
				Dim newMap() As Single = measureState(newData)

				If newMap.Length <> map_Conflict.Length Then
					Console.WriteLine("Different map lengths")
					Return False
				End If

				If map_Conflict.SequenceEqual(newMap) Then
	'                System.out.println("Maps are equal");
					Return False
				End If

				Dim x As Integer = 0
				Do While x < newData.rows()
					Dim row As INDArray = newData.getRow(x)
					For y As Integer = 0 To row.length() - 1
						If Math.Abs(row.getFloat(y) - newMap(x)) > Nd4j.EPS_THRESHOLD Then
							Console.Write("Different data in a row")
							Return False
						End If
					Next y
					x += 1
				Loop

				Return True
			End Function

			Public Overridable Function compareColumn(ByVal newData As INDArray) As Boolean
				Dim newMap() As Single = measureState(newData)

				If newMap.Length <> map_Conflict.Length Then
					Console.WriteLine("Different map lengths")
					Return False
				End If

				If map_Conflict.SequenceEqual(newMap) Then
	'                System.out.println("Maps are equal");
					Return False
				End If

				Dim x As Integer = 0
				Do While x < newData.rows()
					Dim column As INDArray = newData.getColumn(x)
					Dim val As Double = column.getDouble(0)
					For y As Integer = 0 To column.length() - 1
						If Math.Abs(column.getFloat(y) - val) > Nd4j.EPS_THRESHOLD Then
							Console.Write("Different data in a column: " & column.getFloat(y))
							Return False
						End If
					Next y
					x += 1
				Loop

				Return True
			End Function

			Public Overridable ReadOnly Property Map As Single()
				Get
					Return map_Conflict
				End Get
			End Property
		End Class

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace