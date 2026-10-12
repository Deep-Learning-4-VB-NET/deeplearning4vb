Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLineInterval = org.datavec.api.records.metadata.RecordMetaDataLineInterval
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.primitives

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

Namespace org.datavec.api.records.reader.impl.csv


	<Serializable>
	Public Class CSVNLinesSequenceRecordReader
		Inherits CSVRecordReader
		Implements SequenceRecordReader

		Public Shared ReadOnly LINES_PER_SEQUENCE As String = NAME_SPACE & ".nlinespersequence"

		Private nLinesPerSequence As Integer
		Private delimiter As String

		''' <summary>
		''' No-arg constructor with the default number of lines per sequence (10)
		''' </summary>
		Public Sub New()
			Me.New(10)
		End Sub

		''' <param name="nLinesPerSequence">    Number of lines in each sequence, use default delemiter(,) between entries in the same line </param>
		Public Sub New(ByVal nLinesPerSequence As Integer)
			Me.New(nLinesPerSequence, 0, CSVRecordReader.DEFAULT_DELIMITER.ToString())
		End Sub

		''' 
		''' <param name="nLinesPerSequence">    Number of lines in each sequences </param>
		''' <param name="skipNumLines">         Number of lines to skip at the start of the file (only skipped once, not per sequence) </param>
		''' <param name="delimiter">            Delimiter between entries in the same line, for example "," </param>
		Public Sub New(ByVal nLinesPerSequence As Integer, ByVal skipNumLines As Integer, ByVal delimiter As String)
			MyBase.New(skipNumLines)
			Me.delimiter = delimiter
			Me.nLinesPerSequence = nLinesPerSequence
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			MyBase.initialize(conf, split)
			Me.nLinesPerSequence = conf.getInt(LINES_PER_SEQUENCE, nLinesPerSequence)
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable))
			If Not MyBase.hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim count As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (count++ < nLinesPerSequence && super.hasNext())
			Do While count < nLinesPerSequence AndAlso MyBase.hasNext()
					count += 1
				sequence.Add(MyBase.next())
			Loop
				count += 1

			Return sequence
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException("Reading CSV data from DataInputStream not yet implemented")
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Dim lineBefore As Integer = lineIndex
			Dim record As IList(Of IList(Of Writable)) = sequenceRecord()
			Dim lineAfter As Integer = lineIndex
			Dim uri As URI = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
			Dim meta As RecordMetaData = New RecordMetaDataLineInterval(lineBefore, lineAfter - 1, uri, GetType(CSVNLinesSequenceRecordReader))
			Return New org.datavec.api.records.impl.SequenceRecord(record, meta)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return loadSequenceFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord)
			'First: create a sorted list of the RecordMetaData
			Dim list As IList(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))) = New List(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))))()
			Dim iter As IEnumerator(Of RecordMetaData) = recordMetaDatas.GetEnumerator()
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim rmd As RecordMetaData = iter.Current
				If Not (TypeOf rmd Is RecordMetaDataLineInterval) Then
					Throw New System.ArgumentException("Invalid metadata; expected RecordMetaDataLineInterval instance; got: " & rmd)
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: list.add(new org.nd4j.common.primitives.Triple<>(count++, (org.datavec.api.records.metadata.RecordMetaDataLineInterval) rmd, (List<List<org.datavec.api.writable.Writable>>) new ArrayList<List<org.datavec.api.writable.Writable>>()));
				list.Add(New Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))(count, DirectCast(rmd, RecordMetaDataLineInterval), CType(New List(Of IList(Of Writable))(), IList(Of IList(Of Writable)))))
					count += 1
			Loop

			'Sort by starting line number:
			list.Sort(New ComparatorAnonymousInnerClass(Me))

			Dim lineIter As IEnumerator(Of String) = getIterator(0) 'TODO handle multi file case...
			Dim currentLineIdx As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim line As String = lineIter.next()
			Do While currentLineIdx < skipNumLines
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				line = lineIter.next()
				currentLineIdx += 1
			Loop
			For Each [next] As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))) In list
				Dim nextStartLine As Integer = [next].getSecond().getLineNumberStart()
				Dim nextEndLine As Integer = [next].getSecond().getLineNumberEnd()
				Do While currentLineIdx < nextStartLine AndAlso lineIter.MoveNext()
					line = lineIter.Current
					currentLineIdx += 1
				Loop
				Do While currentLineIdx <= nextEndLine AndAlso (lineIter.MoveNext() OrElse currentLineIdx = nextEndLine)
					Dim split() As String = line.Split(Me.delimiter, False)
					Dim writables As IList(Of Writable) = New List(Of Writable)()
					For Each s As String In split
						writables.Add(New Text(s))
					Next s
					[next].getThird().add(writables)
					currentLineIdx += 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If lineIter.hasNext() Then
						line = lineIter.Current
					End If
				Loop
			Next [next]
			closeIfRequired(lineIter)

			'Now, sort by the original order:
			list.Sort(New ComparatorAnonymousInnerClass2(Me))

			'And return...
			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			For Each t As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))) In list
				[out].Add(New org.datavec.api.records.impl.SequenceRecord(t.getThird(), t.getSecond()))
			Next t

			Return [out]
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))))

			Private ReadOnly outerInstance As CSVNLinesSequenceRecordReader

			Public Sub New(ByVal outerInstance As CSVNLinesSequenceRecordReader)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))), ByVal o2 As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))) As Integer Implements IComparer(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))).Compare
				Return Integer.compare(o1.getSecond().getLineNumberStart(), o2.getSecond().getLineNumberStart())
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))))

			Private ReadOnly outerInstance As CSVNLinesSequenceRecordReader

			Public Sub New(ByVal outerInstance As CSVNLinesSequenceRecordReader)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable))), ByVal o2 As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))) As Integer Implements IComparer(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of Writable)))).Compare
				Return Integer.compare(o1.getFirst(), o2.getFirst())
			End Function
		End Class

		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace