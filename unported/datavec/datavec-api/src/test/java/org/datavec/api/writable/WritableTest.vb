Imports System.Collections.Generic
Imports NDArrayRecordBatch = org.datavec.api.writable.batch.NDArrayRecordBatch
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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
'ORIGINAL LINE: @DisplayName("Writable Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class WritableTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class WritableTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Writable Equality Reflexive") void testWritableEqualityReflexive()
		Friend Overridable Sub testWritableEqualityReflexive()
			assertEquals(New IntWritable(1), New IntWritable(1))
			assertEquals(New LongWritable(1), New LongWritable(1))
			assertEquals(New DoubleWritable(1), New DoubleWritable(1))
			assertEquals(New FloatWritable(1), New FloatWritable(1))
			assertEquals(New Text("Hello"), New Text("Hello"))
			assertEquals(New BytesWritable("Hello".GetBytes()), New BytesWritable("Hello".GetBytes()))
			Dim ndArray As INDArray = Nd4j.rand(New Integer() { 1, 100 })
			assertEquals(New NDArrayWritable(ndArray), New NDArrayWritable(ndArray))
			assertEquals(New NullWritable(), New NullWritable())
			assertEquals(New BooleanWritable(True), New BooleanWritable(True))
			Dim b As SByte = 0
			assertEquals(New ByteWritable(b), New ByteWritable(b))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Bytes Writable Indexing") void testBytesWritableIndexing()
		Friend Overridable Sub testBytesWritableIndexing()
			Dim doubleWrite(15) As SByte
			Dim wrapped As ByteBuffer = ByteBuffer.wrap(doubleWrite)
			Dim buffer As Buffer = CType(wrapped, Buffer)
			wrapped.putDouble(1.0)
			wrapped.putDouble(2.0)
			buffer.rewind()
			Dim byteWritable As New BytesWritable(doubleWrite)
			assertEquals(2, byteWritable.getDouble(1), 1e-1)
			Dim dataBuffer As DataBuffer = Nd4j.createBuffer(New Double() { 1, 2 })
			Dim d1() As Double = dataBuffer.asDouble()
			Dim d2() As Double = byteWritable.asNd4jBuffer(DataType.DOUBLE, 8).asDouble()
			assertArrayEquals(d1, d2, 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Byte Writable") void testByteWritable()
		Friend Overridable Sub testByteWritable()
			Dim b As SByte = &HfffffffeL
			assertEquals(New IntWritable(-2), New ByteWritable(b))
			assertEquals(New LongWritable(-2), New ByteWritable(b))
			assertEquals(New ByteWritable(b), New IntWritable(-2))
			assertEquals(New ByteWritable(b), New LongWritable(-2))
			' those would cast to the same Int
			Dim minus126 As SByte = &Hffffff82L
			assertNotEquals(New ByteWritable(minus126), New IntWritable(130))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Int Long Writable") void testIntLongWritable()
		Friend Overridable Sub testIntLongWritable()
			assertEquals(New IntWritable(1), New LongWritable(1l))
			assertEquals(New LongWritable(2l), New IntWritable(2))
			Dim l As Long = 1L << 34
			' those would cast to the same Int
			assertNotEquals(New LongWritable(l), New IntWritable(4))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Double Float Writable") void testDoubleFloatWritable()
		Friend Overridable Sub testDoubleFloatWritable()
			assertEquals(New DoubleWritable(1R), New FloatWritable(1f))
			assertEquals(New FloatWritable(2f), New DoubleWritable(2R))
			' we defer to Java equality for Floats
			assertNotEquals(New DoubleWritable(1.1R), New FloatWritable(1.1f))
			' same idea as above
			assertNotEquals(New DoubleWritable(1.1R), New FloatWritable(CSng(1.1R)))
			assertNotEquals(New DoubleWritable(CDbl(Single.MaxValue) + 1), New FloatWritable(Single.PositiveInfinity))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Fuzzies") void testFuzzies()
		Friend Overridable Sub testFuzzies()
			assertTrue((New DoubleWritable(1.1R)).fuzzyEquals(New FloatWritable(1.1f), 1e-6R))
			assertTrue((New FloatWritable(1.1f)).fuzzyEquals(New DoubleWritable(1.1R), 1e-6R))
			Dim b As SByte = &HfffffffeL
			assertTrue((New ByteWritable(b)).fuzzyEquals(New DoubleWritable(-2.0), 1e-6R))
			assertFalse((New IntWritable(1)).fuzzyEquals(New FloatWritable(1.1f), 1e-2R))
			assertTrue((New IntWritable(1)).fuzzyEquals(New FloatWritable(1.05f), 1e-1R))
			assertTrue((New LongWritable(1)).fuzzyEquals(New DoubleWritable(1.05f), 1e-1R))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Record Batch") void testNDArrayRecordBatch()
		Friend Overridable Sub testNDArrayRecordBatch()
			Nd4j.Random.setSeed(12345)
			' Outer list over writables/columns, inner list over examples
			Dim orig As IList(Of IList(Of INDArray)) = New List(Of IList(Of INDArray))()
			For i As Integer = 0 To 2
				orig.Add(New List(Of INDArray)())
			Next i
			For i As Integer = 0 To 4
				orig(0).Add(Nd4j.rand(1, 10))
				orig(1).Add(Nd4j.rand(New Integer() { 1, 5, 6 }))
				orig(2).Add(Nd4j.rand(New Integer() { 1, 3, 4, 5 }))
			Next i
			' Outer list over examples, inner list over writables
			Dim origByExample As IList(Of IList(Of INDArray)) = New List(Of IList(Of INDArray))()
			For i As Integer = 0 To 4
				origByExample.Add(New List(Of INDArray) From {orig(0)(i), orig(1)(i), orig(2)(i)})
			Next i
			Dim batched As IList(Of INDArray) = New List(Of INDArray)()
			For Each l As IList(Of INDArray) In orig
				batched.Add(Nd4j.concat(0, CType(l, List(Of INDArray)).ToArray()))
			Next l
			Dim batch As New NDArrayRecordBatch(batched)
			assertEquals(5, batch.Count)
			For i As Integer = 0 To 4
				Dim act As IList(Of Writable) = batch(i)
				Dim unboxed As IList(Of INDArray) = New List(Of INDArray)()
				For Each w As Writable In act
					unboxed.Add(DirectCast(w, NDArrayWritable).get())
				Next w
				Dim exp As IList(Of INDArray) = origByExample(i)
				assertEquals(exp.Count, unboxed.Count)
				For j As Integer = 0 To exp.Count - 1
					assertEquals(exp(j), unboxed(j))
				Next j
			Next i
			Dim iter As IEnumerator(Of IList(Of Writable)) = batch.GetEnumerator()
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim [next] As IList(Of Writable) = iter.Current
				Dim unboxed As IList(Of INDArray) = New List(Of INDArray)()
				For Each w As Writable In [next]
					unboxed.Add(DirectCast(w, NDArrayWritable).get())
				Next w
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.util.List<org.nd4j.linalg.api.ndarray.INDArray> exp = origByExample.get(count++);
				Dim exp As IList(Of INDArray) = origByExample(count)
					count += 1
				assertEquals(exp.Count, unboxed.Count)
				For j As Integer = 0 To exp.Count - 1
					assertEquals(exp(j), unboxed(j))
				Next j
			Loop
			assertEquals(5, count)
		End Sub
	End Class

End Namespace