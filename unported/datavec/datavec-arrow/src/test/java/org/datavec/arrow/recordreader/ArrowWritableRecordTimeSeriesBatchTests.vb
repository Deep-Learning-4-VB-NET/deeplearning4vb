Imports System.Collections.Generic
Imports BufferAllocator = org.apache.arrow.memory.BufferAllocator
Imports RootAllocator = org.apache.arrow.memory.RootAllocator
Imports FieldVector = org.apache.arrow.vector.FieldVector
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports ArrowConverter = org.datavec.arrow.ArrowConverter
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse

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

Namespace org.datavec.arrow.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class ArrowWritableRecordTimeSeriesBatchTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class ArrowWritableRecordTimeSeriesBatchTests
		Inherits BaseND4JTest

		Private Shared bufferAllocator As BufferAllocator = New RootAllocator(Long.MaxValue)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) @Disabled public void testBasicIndexing()
		Public Overridable Sub testBasicIndexing()
			Dim schema As New Schema.Builder()
			For i As Integer = 0 To 2
				schema.addColumnInteger(i.ToString())
			Next i


			Dim timeStep As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0),New IntWritable(1),New IntWritable(2)), Arrays.asList(New IntWritable(1),New IntWritable(2),New IntWritable(3)), Arrays.asList(New IntWritable(4),New IntWritable(5),New IntWritable(6))}

			Dim numTimeSteps As Integer = 5
			Dim timeSteps As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(numTimeSteps)
			For i As Integer = 0 To numTimeSteps - 1
				timeSteps.Add(timeStep)
			Next i

			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumnsTimeSeries(bufferAllocator, schema.build(), timeSteps)
			assertEquals(3,fieldVectors.Count)
			For Each fieldVector As FieldVector In fieldVectors
				Dim i As Integer = 0
				Do While i < fieldVector.getValueCount()
					assertFalse(fieldVector.isNull(i),"Index " & i & " was null for field vector " & fieldVector)
					i += 1
				Loop
			Next fieldVector

			Dim arrowWritableRecordTimeSeriesBatch As New ArrowWritableRecordTimeSeriesBatch(fieldVectors,schema.build(),timeStep.Count * timeStep(0).Count)
			assertEquals(timeSteps,arrowWritableRecordTimeSeriesBatch.toArrayList())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) @Disabled public void testVariableLengthTS()
		Public Overridable Sub testVariableLengthTS()
			Dim schema As Schema.Builder = (New Schema.Builder()).addColumnString("str").addColumnInteger("int").addColumnDouble("dbl")

			Dim firstSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("00"),New IntWritable(0),New DoubleWritable(2.0)), Arrays.asList(New Text("01"),New IntWritable(1),New DoubleWritable(2.1)), Arrays.asList(New Text("02"),New IntWritable(2),New DoubleWritable(2.2))}

			Dim secondSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("10"),New IntWritable(10),New DoubleWritable(12.0)), Arrays.asList(New Text("11"),New IntWritable(11),New DoubleWritable(12.1))}

			Dim sequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable))) From {firstSeq, secondSeq}


			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumnsTimeSeries(bufferAllocator, schema.build(), sequences)
			assertEquals(3,fieldVectors.Count)

			Dim timeSeriesStride As Integer = -1 'Can't sequences of different length...
			Dim arrowWritableRecordTimeSeriesBatch As New ArrowWritableRecordTimeSeriesBatch(fieldVectors,schema.build(),timeSeriesStride)

			Dim asList As IList(Of IList(Of IList(Of Writable))) = arrowWritableRecordTimeSeriesBatch.toArrayList()
			assertEquals(sequences, asList)
		End Sub


	End Class

End Namespace