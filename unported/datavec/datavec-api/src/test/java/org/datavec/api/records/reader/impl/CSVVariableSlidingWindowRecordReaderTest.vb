Imports System.Collections.Generic
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVVariableSlidingWindowRecordReader = org.datavec.api.records.reader.impl.csv.CSVVariableSlidingWindowRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.datavec.api.records.reader.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Csv Variable Sliding Window Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVVariableSlidingWindowRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVVariableSlidingWindowRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CSV Variable Sliding Window Record Reader") void testCSVVariableSlidingWindowRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCSVVariableSlidingWindowRecordReader()
			Dim maxLinesPerSequence As Integer = 3
			Dim seqRR As SequenceRecordReader = New CSVVariableSlidingWindowRecordReader(maxLinesPerSequence)
			seqRR.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim rr As New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim count As Integer = 0
			Do While seqRR.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = seqRR.sequenceRecord()
				If count = maxLinesPerSequence - 1 Then
					Dim expected As New LinkedList(Of IList(Of Writable))()
					For i As Integer = 0 To maxLinesPerSequence - 1
						expected.AddFirst(rr.next())
					Next i
					assertEquals(expected, [next])
				End If
				If count = maxLinesPerSequence Then
					assertEquals(maxLinesPerSequence, [next].Count)
				End If
				If count = 0 Then
					' first seq should be length 1
					assertEquals(1, [next].Count)
				End If
				If count > 151 Then
					' last seq should be length 1
					assertEquals(1, [next].Count)
				End If
				count += 1
			Loop
			assertEquals(152, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CSV Variable Sliding Window Record Reader Stride") void testCSVVariableSlidingWindowRecordReaderStride() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCSVVariableSlidingWindowRecordReaderStride()
			Dim maxLinesPerSequence As Integer = 3
			Dim stride As Integer = 2
			Dim seqRR As SequenceRecordReader = New CSVVariableSlidingWindowRecordReader(maxLinesPerSequence, stride)
			seqRR.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim rr As New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim count As Integer = 0
			Do While seqRR.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = seqRR.sequenceRecord()
				If count = maxLinesPerSequence - 1 Then
					Dim expected As New LinkedList(Of IList(Of Writable))()
					For s As Integer = 0 To stride - 1
						expected = New LinkedList(Of IList(Of Writable))()
						For i As Integer = 0 To maxLinesPerSequence - 1
							expected.AddFirst(rr.next())
						Next i
					Next s
					assertEquals(expected, [next])
				End If
				If count = maxLinesPerSequence Then
					assertEquals(maxLinesPerSequence, [next].Count)
				End If
				If count = 0 Then
					' first seq should be length 2
					assertEquals(2, [next].Count)
				End If
				If count > 151 Then
					' last seq should be length 1
					assertEquals(1, [next].Count)
				End If
				count += 1
			Loop
			assertEquals(76, count)
		End Sub
	End Class

End Namespace