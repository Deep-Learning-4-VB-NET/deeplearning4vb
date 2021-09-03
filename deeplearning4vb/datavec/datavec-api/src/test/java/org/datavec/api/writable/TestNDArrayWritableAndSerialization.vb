Imports System.IO
Imports NDArrayMetaData = org.datavec.api.transform.metadata.NDArrayMetaData
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.api.writable


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestNDArrayWritableAndSerialization extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestNDArrayWritableAndSerialization
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIsValid()
		Public Overridable Sub testIsValid()

			Dim meta As New NDArrayMetaData("col", New Long() {1, 10})

			Dim valid As New NDArrayWritable(Nd4j.create(1, 10))
			Dim invalid As New NDArrayWritable(Nd4j.create(1, 5))
			Dim invalid2 As New NDArrayWritable(Nothing)


			assertTrue(meta.isValid(valid))
			assertFalse(meta.isValid(invalid))
			assertFalse(meta.isValid(invalid2))

			assertTrue(meta.isValid(valid.get()))
			assertFalse(meta.isValid(invalid.get()))
			assertFalse(meta.isValid(invalid2.get()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritableSerializationSingle() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritableSerializationSingle()

			Dim arrC As INDArray = Nd4j.rand(New Integer() {1, 10}, "c"c)

			Dim baos As New MemoryStream()
			Dim da As DataOutput = New DataOutputStream(baos)

			Dim wC As New NDArrayWritable(arrC)
			wC.write(da)

			Dim b() As SByte = baos.toByteArray()

			Dim w2C As New NDArrayWritable()

			Dim bais As New MemoryStream(b)
			Dim din As DataInput = New DataInputStream(bais)

			w2C.readFields(din)


			assertEquals(arrC, w2C.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritableSerialization() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritableSerialization()

			Dim arrC As INDArray = Nd4j.rand(New Integer() {10, 20}, "c"c)
			Dim arrF As INDArray = Nd4j.rand(New Integer() {10, 20}, "f"c)


			Dim baos As New MemoryStream()
			Dim da As DataOutput = New DataOutputStream(baos)

			Dim wC As New NDArrayWritable(arrC)
			Dim wF As New NDArrayWritable(arrF)

			wC.write(da)
			wF.write(da)

			Dim b() As SByte = baos.toByteArray()

			Dim w2C As New NDArrayWritable()
			Dim w2F As New NDArrayWritable()

			Dim bais As New MemoryStream(b)
			Dim din As DataInput = New DataInputStream(bais)

			w2C.readFields(din)
			w2F.readFields(din)


			assertEquals(arrC, w2C.get())
			assertEquals(arrF, w2F.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritableEqualsHashCodeOrdering() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritableEqualsHashCodeOrdering()
			'NDArrayWritable implements WritableComparable - we need to make sure this operates as expected...

			'First: check C vs. F order, same contents
			Dim arrC As INDArray = Nd4j.rand(New Integer() {10, 20}, "c"c)
			Dim arrF As INDArray = arrC.dup("f"c)

			Dim wC As New NDArrayWritable(arrC)
			Dim wF As New NDArrayWritable(arrF)

			assertEquals(wC, wF)
			assertEquals(wC.GetHashCode(), wF.GetHashCode())

			Dim compare As Integer = wC.compareTo(wF)
			assertEquals(0, compare)


			'Check order conventions:
			'Null first
			'Then smallest rank first
			'Then smallest length first
			'Then sort by shape
			'Then sort by contents, element-wise

			assertEquals(-1, (New NDArrayWritable(Nothing)).compareTo(New NDArrayWritable(Nd4j.create(1))))
			assertEquals(-1, (New NDArrayWritable(Nd4j.create(1, 1))).compareTo(New NDArrayWritable(Nd4j.create(1, 1, 1))))
			assertEquals(-1, (New NDArrayWritable(Nd4j.create(1, 1))).compareTo(New NDArrayWritable(Nd4j.create(1, 2))))
			assertEquals(-1, (New NDArrayWritable(Nd4j.create(1, 3))).compareTo(New NDArrayWritable(Nd4j.create(3, 1))))
			assertEquals(-1, (New NDArrayWritable(Nd4j.create(New Double() {1.0, 2.0, 3.0}))).compareTo(New NDArrayWritable(Nd4j.create(New Double() {1.0, 2.0, 3.1}))))

			assertEquals(1, (New NDArrayWritable(Nd4j.create(1))).compareTo(New NDArrayWritable(Nothing)))
			assertEquals(1, (New NDArrayWritable(Nd4j.create(1, 1, 1))).compareTo(New NDArrayWritable(Nd4j.create(1, 1))))
			assertEquals(1, (New NDArrayWritable(Nd4j.create(1, 2))).compareTo(New NDArrayWritable(Nd4j.create(1, 1))))
			assertEquals(1, (New NDArrayWritable(Nd4j.create(3, 1))).compareTo(New NDArrayWritable(Nd4j.create(1, 3))))
			assertEquals(1, (New NDArrayWritable(Nd4j.create(New Double() {1.0, 2.0, 3.1}))).compareTo(New NDArrayWritable(Nd4j.create(New Double() {1.0, 2.0, 3.0}))))
		End Sub

	End Class

End Namespace