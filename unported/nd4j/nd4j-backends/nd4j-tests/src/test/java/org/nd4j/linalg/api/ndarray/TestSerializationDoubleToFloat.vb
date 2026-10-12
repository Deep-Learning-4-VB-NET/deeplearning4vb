Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.api.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_SERDE) public class TestSerializationDoubleToFloat extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestSerializationDoubleToFloat
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			DataTypeUtil.setDTypeForContext(Me.initialType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationFullArrayNd4jWriteRead(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationFullArrayNd4jWriteRead(ByVal backend As Nd4jBackend)
			Dim length As Integer = 4

			'WRITE OUT A DOUBLE ARRAY
			'Hack before setting datatype - fix already in r119_various branch
			Nd4j.create(1)
			Dim initialType As val = Nd4j.dataType()

			Nd4j.DataType = DataType.DOUBLE
			Dim arr As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 2, 2)
			arr.subi(50.0123456) 'assures positive and negative numbers with decimal points

			Dim baos As New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write(arr, dos)
			End Using
			Dim bytes() As SByte = baos.toByteArray()

			'SET DATA TYPE TO FLOAT and initialize another array with the same contents
			'Nd4j.create(1);
			DataTypeUtil.setDTypeForContext(DataType.FLOAT)
			Console.WriteLine("The data opType is " & Nd4j.dataType())
			Dim arr1 As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 2, 2)
			arr1.subi(50.0123456)

			log.info("A  ---------------")

			Dim arr2 As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytes))
				arr2 = Nd4j.read(dis)
			End Using

			'System.out.println(new NDArrayStrings(6).format(arr2.sub(arr1).mul(100).div(arr1)));
			arr1 = arr1.castTo(DataType.DOUBLE)
			assertTrue(Transforms.abs(arr1.sub(arr2).div(arr1)).maxNumber().doubleValue() < 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationFullArrayJava(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationFullArrayJava(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Nd4j.create(1)
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Dim arr As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			arr.subi(50.0123456) 'assures positive and negative numbers with decimal points

			Dim baos As New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject(arr)
			End Using
			Dim bytes() As SByte = baos.toByteArray()

			'SET DATA TYPE TO FLOAT and initialize another array with the same contents
			'Nd4j.create(1);
			DataTypeUtil.setDTypeForContext(DataType.FLOAT)
			Console.WriteLine("The data opType is " & Nd4j.dataType())
			Dim arr1 As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			arr1.subi(50.0123456)

			Dim arr2 As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytes))
				arr2 = DirectCast(ois.readObject(), INDArray)
			End Using

			arr1 = arr1.castTo(DataType.DOUBLE)
			assertTrue(Transforms.abs(arr1.sub(arr2).div(arr1)).maxNumber().doubleValue() < 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationOnViewsNd4jWriteRead(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationOnViewsNd4jWriteRead(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Nd4j.create(1)
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Dim arr As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			Dim [sub] As INDArray = arr.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim baos As New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write([sub], dos)
			End Using
			Dim bytes() As SByte = baos.toByteArray()

			'SET DATA TYPE TO FLOAT and initialize another array with the same contents
			'Nd4j.create(1);
			DataTypeUtil.setDTypeForContext(DataType.FLOAT)
			Console.WriteLine("The data opType is " & Nd4j.dataType())
			Dim arr1 As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, 10, 10)
			Dim sub1 As INDArray = arr1.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim arr2 As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytes))
				arr2 = Nd4j.read(dis)
			End Using

			'assertEquals(sub,arr2);
			assertTrue(Transforms.abs(sub1.sub(arr2).div(sub1)).maxNumber().doubleValue() < 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationOnViewsJava(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationOnViewsJava(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Nd4j.create(1)
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Dim arr As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)

			Dim [sub] As INDArray = arr.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim baos As New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject([sub])
			End Using
			Dim bytes() As SByte = baos.toByteArray()
			DataTypeUtil.setDTypeForContext(DataType.FLOAT)
			Console.WriteLine("The data opType is " & Nd4j.dataType())
			Dim arr1 As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE).reshape("c"c, 10, 10)
			Dim sub1 As INDArray = arr1.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim arr2 As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytes))
				arr2 = DirectCast(ois.readObject(), INDArray)
			End Using

			'assertEquals(sub,arr2);
			assertTrue(Transforms.abs(sub1.sub(arr2).div(sub1)).maxNumber().doubleValue() < 0.01)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace