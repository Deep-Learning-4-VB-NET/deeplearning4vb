Imports System
Imports System.Collections.Generic
Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports FileRecordReader = org.datavec.api.records.reader.impl.FileRecordReader
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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
	Public Class CSVSequenceRecordReader
		Inherits FileRecordReader
		Implements SequenceRecordReader

		Private skipNumLines As Integer = 0
		Private delimiter As String = ","

		Public Sub New()
			Me.New(0, ",")
		End Sub

		Public Sub New(ByVal skipNumLines As Integer)
			Me.New(skipNumLines, ",")
		End Sub

		Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As String)
			Me.skipNumLines = skipNumLines
			Me.delimiter = delimiter
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))
			invokeListeners(uri)
			Return loadAndClose(dataInputStream)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public List<List<org.datavec.api.writable.Writable>> sequenceRecord()
		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable))
			Return nextSequence().getSequenceRecord()
		End Function


		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As URI = locationsIterator.next()
			invokeListeners([next])

			Dim [out] As IList(Of IList(Of Writable)) = loadAndClose(streamCreatorFn.apply([next]))
			Return New org.datavec.api.records.impl.SequenceRecord([out], New RecordMetaDataURI([next]))
		End Function

		Private Function loadAndClose(ByVal inputStream As Stream) As IList(Of IList(Of Writable))
			Dim lineIter As LineIterator = Nothing
			Try
				lineIter = IOUtils.lineIterator(New StreamReader(inputStream))
				Return load(lineIter)
			Finally
				If lineIter IsNot Nothing Then
					lineIter.close()
				End If
				IOUtils.closeQuietly(inputStream)
			End Try
		End Function

		Private Function load(ByVal lineIter As IEnumerator(Of String)) As IList(Of IList(Of Writable))
			If skipNumLines > 0 Then
				Dim count As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (count++ < skipNumLines && lineIter.hasNext())
				Do While count < skipNumLines AndAlso lineIter.MoveNext()
						count += 1
					lineIter.Current
				Loop
					count += 1
			End If

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While lineIter.MoveNext()
				Dim line As String = lineIter.Current
				Dim split() As String = line.Split(delimiter, True)
				Dim list As New List(Of Writable)()
				For Each s As String In split
					list.Add(New Text(s))
				Next s
				[out].Add(list)
			Loop
			Return [out]
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return loadSequenceFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord)
			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			For Each meta As RecordMetaData In recordMetaDatas
				Dim [next] As New File(meta.URI)
				Dim sequence As IList(Of IList(Of Writable)) = loadAndClose(New FileStream([next], FileMode.Open, FileAccess.Read))
				[out].Add(New org.datavec.api.records.impl.SequenceRecord(sequence, meta))
			Next meta
			Return [out]
		End Function
	End Class

End Namespace