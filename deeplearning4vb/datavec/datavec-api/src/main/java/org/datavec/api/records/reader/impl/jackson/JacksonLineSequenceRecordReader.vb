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
Imports Writable = org.datavec.api.writable.Writable
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.datavec.api.records.reader.impl.jackson


	<Serializable>
	Public Class JacksonLineSequenceRecordReader
		Inherits FileRecordReader
		Implements SequenceRecordReader

		Private selection As FieldSelection
		Private mapper As ObjectMapper

		''' 
		''' <param name="selection"> Fields to return </param>
		''' <param name="mapper">    Object mapper to use. For example, {@code new ObjectMapper(new JsonFactory())} for JSON </param>
		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper)
			Me.selection = selection
			Me.mapper = mapper
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return nextSequence().getSequenceRecord()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))
			Return loadAndClose(dataInputStream)
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As URI = locationsIterator.next()
			Dim [out] As IList(Of IList(Of Writable)) = loadAndClose(streamCreatorFn.apply([next]))
			Return New org.datavec.api.records.impl.SequenceRecord([out], New RecordMetaDataURI([next]))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			For Each m As RecordMetaData In recordMetaDatas
				[out].Add(loadSequenceFromMetaData(m))
			Next m
			Return [out]
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
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While lineIter.MoveNext()
				[out].Add(JacksonReaderUtils.parseRecord(lineIter.Current, selection, mapper))
			Loop
			Return [out]
		End Function
	End Class

End Namespace