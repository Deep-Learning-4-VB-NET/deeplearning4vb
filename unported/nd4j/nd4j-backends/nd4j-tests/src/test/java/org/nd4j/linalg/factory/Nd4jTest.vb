Imports System.Collections.Generic
Imports val = lombok.val
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.factory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.RNG) @NativeTag @Tag(TagNames.FILE_IO) public class Nd4jTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class Nd4jTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandShapeAndRNG(Nd4jBackend backend)
		Public Overridable Sub testRandShapeAndRNG(ByVal backend As Nd4jBackend)
			Dim ret As INDArray = Nd4j.rand(New Integer() {4, 2}, Nd4j.RandomFactory.getNewRandomInstance(123))
			Dim ret2 As INDArray = Nd4j.rand(New Integer() {4, 2}, Nd4j.RandomFactory.getNewRandomInstance(123))

			assertEquals(ret, ret2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandShapeAndMinMax(Nd4jBackend backend)
		Public Overridable Sub testRandShapeAndMinMax(ByVal backend As Nd4jBackend)
			Dim ret As INDArray = Nd4j.rand(New Integer() {4, 2}, -0.125f, 0.125f, Nd4j.RandomFactory.getNewRandomInstance(123))
			Dim ret2 As INDArray = Nd4j.rand(New Integer() {4, 2}, -0.125f, 0.125f, Nd4j.RandomFactory.getNewRandomInstance(123))
			assertEquals(ret, ret2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateShape(Nd4jBackend backend)
		Public Overridable Sub testCreateShape(ByVal backend As Nd4jBackend)
			Dim ret As INDArray = Nd4j.create(New Integer() {4, 2})

			assertEquals(ret.length(), 8)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateFromList(Nd4jBackend backend)
		Public Overridable Sub testCreateFromList(ByVal backend As Nd4jBackend)
			Dim doubles As IList(Of Double) = New List(Of Double) From {1.0, 2.0}
			Dim NdarrayDobules As INDArray = Nd4j.create(doubles)

			assertEquals(CType(NdarrayDobules.getDouble(0), Double?),doubles(0))
			assertEquals(CType(NdarrayDobules.getDouble(1), Double?),doubles(1))

			Dim floats As IList(Of Single) = New List(Of Single) From {3.0f, 4.0f}
			Dim NdarrayFloats As INDArray = Nd4j.create(floats)
			assertEquals(CType(NdarrayFloats.getFloat(0), Single?),floats(0))
			assertEquals(CType(NdarrayFloats.getFloat(1), Single?),floats(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRandom(Nd4jBackend backend)
		Public Overridable Sub testGetRandom(ByVal backend As Nd4jBackend)
			Dim r As Random = Nd4j.Random
			Dim t As Random = Nd4j.Random

			assertEquals(r, t)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRandomSetSeed(Nd4jBackend backend)
		Public Overridable Sub testGetRandomSetSeed(ByVal backend As Nd4jBackend)
			Dim r As Random = Nd4j.Random
			Dim t As Random = Nd4j.Random

			assertEquals(r, t)
			r.setSeed(123)
			assertEquals(r, t)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOrdering(Nd4jBackend backend)
		Public Overridable Sub testOrdering(ByVal backend As Nd4jBackend)
			Dim fNDArray As INDArray = Nd4j.create(New Single() {1f}, NDArrayFactory.FORTRAN)
			assertEquals(NDArrayFactory.FORTRAN, fNDArray.ordering())
			Dim cNDArray As INDArray = Nd4j.create(New Single() {1f}, NDArrayFactory.C)
			assertEquals(NDArrayFactory.C, cNDArray.ordering())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean(Nd4jBackend backend)
		Public Overridable Sub testMean(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.create(New Double() {4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0}, New Integer() {2, 2, 4, 4})

			Dim actualResult As INDArray = data.mean(0)
			Dim expectedResult As INDArray = Nd4j.create(New Double() {3.0, 3.0, 3.0, 3.0, 6.0, 6.0, 6.0, 6.0, 3.0, 3.0, 3.0, 3.0, 6.0, 6.0, 6.0, 6.0, 3.0, 3.0, 3.0, 3.0, 6.0, 6.0, 6.0, 6.0, 3.0, 3.0, 3.0, 3.0, 6.0, 6.0, 6.0, 6.0}, New Integer() {2, 4, 4})
			assertEquals(expectedResult, actualResult,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVar(Nd4jBackend backend)
		Public Overridable Sub testVar(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.create(New Double() {4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8.0, 4.0, 4.0, 4.0, 4.0, 8.0, 8.0, 8.0, 8, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 4.0}, New Long() {2, 2, 4, 4})

			Dim actualResult As INDArray = data.var(False, 0)
			Dim expectedResult As INDArray = Nd4j.create(New Double() {1.0, 1.0, 1.0, 1.0, 4.0, 4.0, 4.0, 4.0, 1.0, 1.0, 1.0, 1.0, 4.0, 4.0, 4.0, 4.0, 1.0, 1.0, 1.0, 1.0, 4.0, 4.0, 4.0, 4.0, 1.0, 1.0, 1.0, 1.0, 4.0, 4.0, 4.0, 4.0}, New Long() {2, 4, 4})
			assertEquals(expectedResult, actualResult,getFailureMessage(backend))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVar2(Nd4jBackend backend)
		Public Overridable Sub testVar2(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim var As INDArray = arr.var(False, 0)
			assertEquals(Nd4j.create(New Double() {2.25, 2.25, 2.25}), var)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandDims()
		Public Overridable Sub testExpandDims()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatricesC = org.nd4j.linalg.checkutil.NDArrayCreationUtil.getAllTestMatricesWithShape("c"c, 3, 5, &HDEAD, org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim testMatricesC As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape("c"c, 3, 5, &HDEAD, DataType.DOUBLE)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatricesF = org.nd4j.linalg.checkutil.NDArrayCreationUtil.getAllTestMatricesWithShape("f"c, 7, 11, &HBEEF, org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim testMatricesF As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape("f"c, 7, 11, &HBEEF, DataType.DOUBLE)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatrices = new java.util.ArrayList<>(testMatricesC);
			Dim testMatrices As New List(Of Pair(Of INDArray, String))(testMatricesC)
			testMatrices.AddRange(testMatricesF)

			For Each testMatrixPair As Pair(Of INDArray, String) In testMatrices
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String recreation = testMatrixPair.getSecond();
				Dim recreation As String = testMatrixPair.Second
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray testMatrix = testMatrixPair.getFirst();
				Dim testMatrix As INDArray = testMatrixPair.First
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final char ordering = testMatrix.ordering();
				Dim ordering As Char = testMatrix.ordering()
				Dim shape As val = testMatrix.shape()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int rank = testMatrix.rank();
				Dim rank As Integer = testMatrix.rank()
				For i As Integer = -rank To rank
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expanded = Nd4j.expandDims(testMatrix, i);
					Dim expanded As INDArray = Nd4j.expandDims(testMatrix, i)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String message = "Expanding in Dimension " + i + "; Shape before expanding: " + java.util.Arrays.toString(shape) + " "+ordering+" Order; Shape after expanding: " + java.util.Arrays.toString(expanded.shape()) + " "+expanded.ordering()+"; Input Created via: " + recreation;
					Dim message As String = "Expanding in Dimension " & i & "; Shape before expanding: " & Arrays.toString(shape) & " " & ordering & " Order; Shape after expanding: " & Arrays.toString(expanded.shape()) & " " & expanded.ordering() & "; Input Created via: " & recreation

					Dim tmR As val = testMatrix.ravel()
					Dim expR As val = expanded.ravel()
					assertEquals(1, expanded.shape()(If(i < 0, i + rank, i)),message)
					assertEquals(tmR, expR,message)
					assertEquals(ordering, expanded.ordering(),message)

					testMatrix.assign(Nd4j.rand(DataType.DOUBLE, shape))
					assertEquals(testMatrix.ravel(), expanded.ravel(),message)
				Next i
			Next testMatrixPair
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqueeze()
		Public Overridable Sub testSqueeze()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatricesC = org.nd4j.linalg.checkutil.NDArrayCreationUtil.getAllTestMatricesWithShape("c"c, 3, 1, &HDEAD, org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim testMatricesC As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape("c"c, 3, 1, &HDEAD, DataType.DOUBLE)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatricesF = org.nd4j.linalg.checkutil.NDArrayCreationUtil.getAllTestMatricesWithShape("f"c, 7, 1, &HBEEF, org.nd4j.linalg.api.buffer.DataType.@DOUBLE);
			Dim testMatricesF As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape("f"c, 7, 1, &HBEEF, DataType.DOUBLE)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, String>> testMatrices = new java.util.ArrayList<>(testMatricesC);
			Dim testMatrices As New List(Of Pair(Of INDArray, String))(testMatricesC)
			testMatrices.AddRange(testMatricesF)

			For Each testMatrixPair As Pair(Of INDArray, String) In testMatrices
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String recreation = testMatrixPair.getSecond();
				Dim recreation As String = testMatrixPair.Second
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray testMatrix = testMatrixPair.getFirst();
				Dim testMatrix As INDArray = testMatrixPair.First
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final char ordering = testMatrix.ordering();
				Dim ordering As Char = testMatrix.ordering()
				Dim shape As val = testMatrix.shape()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray squeezed = Nd4j.squeeze(testMatrix, 1);
				Dim squeezed As INDArray = Nd4j.squeeze(testMatrix, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] expShape = org.nd4j.common.util.ArrayUtil.removeIndex(shape, 1);
				Dim expShape() As Long = ArrayUtil.removeIndex(shape, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String message = "Squeezing in dimension 1; Shape before squeezing: " + java.util.Arrays.toString(shape) + " " + ordering + " Order; Shape after expanding: " + java.util.Arrays.toString(squeezed.shape()) + " "+squeezed.ordering()+"; Input Created via: " + recreation;
				Dim message As String = "Squeezing in dimension 1; Shape before squeezing: " & Arrays.toString(shape) & " " & ordering & " Order; Shape after expanding: " & Arrays.toString(squeezed.shape()) & " " & squeezed.ordering() & "; Input Created via: " & recreation

				assertArrayEquals(expShape, squeezed.shape(),message)
				assertEquals(ordering, squeezed.ordering(),message)
				assertEquals(testMatrix.ravel(), squeezed.ravel(),message)

				testMatrix.assign(Nd4j.rand(shape))
				assertEquals(testMatrix.ravel(), squeezed.ravel(),message)

			Next testMatrixPair
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumpyConversion() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNumpyConversion()
			Dim linspace As INDArray = Nd4j.linspace(1,4,4, DataType.FLOAT)
			Dim convert As Pointer = Nd4j.NDArrayFactory.convertToNumpy(linspace)
			convert.position(0)

			Dim pointer As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().loadNpyFromHeader(convert)
			Dim pointer1 As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().dataPointForNumpyStruct(pointer)
			pointer1.capacity(linspace.data().ElementSize * linspace.data().length())
			Dim byteBuffer As ByteBuffer = linspace.data().pointer().asByteBuffer()
			Dim originalData(byteBuffer.capacity() - 1) As SByte
			byteBuffer.get(originalData)


			Dim floatBuffer As ByteBuffer = pointer1.asByteBuffer()
			Dim dataTwo(floatBuffer.capacity() - 1) As SByte
			floatBuffer.get(dataTwo)
			assertArrayEquals(originalData,dataTwo)
			Dim buffer As Buffer = CType(floatBuffer, Buffer)
			buffer.position(0)

			Dim dataBuffer As DataBuffer = Nd4j.createBuffer(New FloatPointer(floatBuffer.asFloatBuffer()),linspace.length(), DataType.FLOAT)
			assertArrayEquals(New Single(){1, 2, 3, 4}, dataBuffer.asFloat(), 1e-5f)

			Dim convertedFrom As INDArray = Nd4j.NDArrayFactory.createFromNpyHeaderPointer(convert)
			assertEquals(linspace,convertedFrom)

			Dim tmpFile As New File(System.getProperty("java.io.tmpdir"),"nd4j-numpy-tmp-" & System.Guid.randomUUID().ToString() & ".bin")
			tmpFile.deleteOnExit()
			Nd4j.writeAsNumpy(linspace,tmpFile)

			Dim numpyFromFile As INDArray = Nd4j.createFromNpyFile(tmpFile)
			assertEquals(linspace,numpyFromFile)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumpyWrite() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNumpyWrite()
			Dim linspace As INDArray = Nd4j.linspace(1,4,4, Nd4j.dataType())
			Dim tmpFile As New File(System.getProperty("java.io.tmpdir"),"nd4j-numpy-tmp-" & System.Guid.randomUUID().ToString() & ".bin")
			tmpFile.deleteOnExit()
			Nd4j.writeAsNumpy(linspace,tmpFile)

			Dim numpyFromFile As INDArray = Nd4j.createFromNpyFile(tmpFile)
			assertEquals(linspace,numpyFromFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNpyByteArray() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNpyByteArray()
			Dim linspace As INDArray = Nd4j.linspace(1,4,4, Nd4j.dataType())
			Dim bytes() As SByte = Nd4j.toNpyByteArray(linspace)
			Dim fromNpy As INDArray = Nd4j.createNpyFromByteArray(bytes)
			assertEquals(linspace,fromNpy)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChoiceDataType()
		Public Overridable Sub testChoiceDataType()
			Dim dataTypeIsDouble As INDArray = Nd4j.empty(DataType.DOUBLE)

			Dim source As INDArray = Nd4j.createFromArray(New Double() { 1.0, 0.0 })
			Dim probs As INDArray = Nd4j.valueArrayOf(New Long() { 2 }, 0.5, DataType.DOUBLE)
			Dim actual As INDArray = Nd4j.choice(source, probs, 10)


			assertEquals(dataTypeIsDouble.dataType(), actual.dataType())
		End Sub


	End Class


End Namespace