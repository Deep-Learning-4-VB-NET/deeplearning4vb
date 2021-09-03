Imports System.IO
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.api.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_SERDE) public class TestSerialization extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestSerialization
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationFullArrayNd4jWriteRead(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationFullArrayNd4jWriteRead(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Dim arrC As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			Dim arrF As INDArray = Nd4j.linspace(1, length, length).reshape("f"c, 10, 10)

			Dim baos As New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write(arrC, dos)
			End Using
			Dim bytesC() As SByte = baos.toByteArray()
			baos = New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write(arrF, dos)
			End Using
			Dim bytesF() As SByte = baos.toByteArray()

			Dim arr2C As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytesC))
				arr2C = Nd4j.read(dis)
			End Using
			Dim arr2F As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytesF))
				arr2F = Nd4j.read(dis)
			End Using

			assertEquals(arrC, arr2C)
			assertEquals(arrF, arr2F)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationFullArrayJava(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationFullArrayJava(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Dim arrC As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			Dim arrF As INDArray = Nd4j.linspace(1, length, length).reshape("f"c, 10, 10)

			Dim baos As New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject(arrC)
			End Using
			Dim bytesC() As SByte = baos.toByteArray()

			baos = New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject(arrF)
			End Using
			Dim bytesF() As SByte = baos.toByteArray()

			Dim arr2C As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytesC))
				arr2C = DirectCast(ois.readObject(), INDArray)
			End Using
			Dim arr2F As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytesF))
				arr2F = DirectCast(ois.readObject(), INDArray)
			End Using

			assertEquals(arrC, arr2C)
			assertEquals(arrF, arr2F)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationOnViewsNd4jWriteRead(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationOnViewsNd4jWriteRead(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Dim arrC As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			Dim arrF As INDArray = Nd4j.linspace(1, length, length).reshape("f"c, 10, 10)

			Dim subC As INDArray = arrC.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))
			Dim subF As INDArray = arrF.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim baos As New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write(subC, dos)
			End Using
			Dim bytesC() As SByte = baos.toByteArray()

			baos = New MemoryStream()
			Using dos As New DataOutputStream(baos)
				Nd4j.write(subF, dos)
			End Using
			Dim bytesF() As SByte = baos.toByteArray()


			Dim arr2C As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytesC))
				arr2C = Nd4j.read(dis)
			End Using

			Dim arr2F As INDArray
			Using dis As New DataInputStream(New MemoryStream(bytesF))
				arr2F = Nd4j.read(dis)
			End Using

			assertEquals(subC, arr2C)
			assertEquals(subF, arr2F)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerializationOnViewsJava(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerializationOnViewsJava(ByVal backend As Nd4jBackend)
			Dim length As Integer = 100
			Dim arrC As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, 10, 10)
			Dim arrF As INDArray = Nd4j.linspace(1, length, length).reshape("f"c, 10, 10)

			Dim subC As INDArray = arrC.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))
			Dim subF As INDArray = arrF.get(NDArrayIndex.interval(5, 10), NDArrayIndex.interval(5, 10))

			Dim baos As New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject(subC)
			End Using
			Dim bytesC() As SByte = baos.toByteArray()
			baos = New MemoryStream()
			Using oos As New ObjectOutputStream(baos)
				oos.writeObject(subF)
			End Using
			Dim bytesF() As SByte = baos.toByteArray()

			Dim arr2C As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytesC))
				arr2C = DirectCast(ois.readObject(), INDArray)
			End Using
			Dim arr2F As INDArray
			Using ois As New ObjectInputStream(New MemoryStream(bytesF))
				arr2F = DirectCast(ois.readObject(), INDArray)
			End Using

			assertEquals(subC, arr2C)
			assertEquals(subF, arr2F)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace