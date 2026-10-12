Imports System
Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
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
	Public Class CSVLineSequenceRecordReader
		Inherits CSVRecordReader
		Implements SequenceRecordReader

		''' <summary>
		''' Default settings: skip 0 lines, use ',' as the delimiter, and '"' for quotes
		''' </summary>
		Public Sub New()
			Me.New(0, DEFAULT_DELIMITER, DEFAULT_QUOTE)
		End Sub

		''' <summary>
		''' Skip lines and use delimiter </summary>
		''' <param name="skipNumLines"> the number of lines to skip </param>
		''' <param name="delimiter"> the delimiter </param>
		Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As Char)
			Me.New(skipNumLines, delimiter, """"c)
		End Sub

		''' <summary>
		''' Skip lines, use delimiter, and strip quotes </summary>
		''' <param name="skipNumLines"> the number of lines to skip </param>
		''' <param name="delimiter"> the delimiter </param>
		''' <param name="quote"> the quote to strip </param>
		Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As Char, ByVal quote As Char)
			MyBase.New(skipNumLines, delimiter, quote)
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return nextSequence().getSequenceRecord()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Dim l As IList(Of Writable) = record(uri, dataInputStream)
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each w As Writable In l
				[out].Add(Collections.singletonList(w))
			Next w
			Return [out]
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Return convert(MyBase.nextRecord())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return convert(MyBase.loadFromMetaData(recordMetaData))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Dim toConvert As IList(Of Record) = MyBase.loadFromMetaData(recordMetaDatas)
			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			For Each r As Record In toConvert
				[out].Add(convert(r))
			Next r
			Return [out]
		End Function

		Protected Friend Overridable Function convert(ByVal r As Record) As SequenceRecord
			Dim line As IList(Of Writable) = r.getRecord()
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each w As Writable In line
				[out].Add(Collections.singletonList(w))
			Next w
			Return New org.datavec.api.records.impl.SequenceRecord([out], r.MetaData)
		End Function
	End Class

End Namespace