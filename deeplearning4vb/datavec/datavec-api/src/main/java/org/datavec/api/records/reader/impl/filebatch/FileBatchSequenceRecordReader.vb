﻿Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports FileBatch = org.nd4j.common.loader.FileBatch
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.datavec.api.records.reader.impl.filebatch


	<Serializable>
	Public Class FileBatchSequenceRecordReader
		Implements SequenceRecordReader

		Private ReadOnly recordReader As SequenceRecordReader
		Private ReadOnly fileBatch As FileBatch
		Private position As Integer = 0

		''' <param name="seqRR">     Underlying record reader to read files from </param>
		''' <param name="fileBatch"> File batch to read files from </param>
		Public Sub New(ByVal seqRR As SequenceRecordReader, ByVal fileBatch As FileBatch)
			Me.recordReader = seqRR
			Me.fileBatch = fileBatch
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Preconditions.checkState(hasNext(), "No next element")

			Dim fileBytes() As SByte = fileBatch.getFileBytes().get(position)
			Dim origPath As String = fileBatch.getOriginalUris().get(position)

			Dim [out] As IList(Of IList(Of Writable))
			Try
				[out] = recordReader.sequenceRecord(URI.create(origPath), New DataInputStream(New MemoryStream(fileBytes)))
			Catch e As IOException
				Throw New Exception("Error reading from file bytes")
			End Try

			position += 1
			Return [out]
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return recordReader.sequenceRecord(uri, dataInputStream)
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Return New org.datavec.api.records.impl.SequenceRecord(sequenceRecord(), Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return recordReader.loadSequenceFromMetaData(recordMetaData)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Return recordReader.loadSequenceFromMetaData(recordMetaDatas)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal split As InputSplit)
			'No op
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			'No op
		End Sub

		Public Overridable Function batchesSupported() As Boolean
			Return False
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function [next]() As IList(Of Writable)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function hasNext() As Boolean
			Return position < fileBatch.getFileBytes().size()
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return recordReader.getLabels()
			End Get
		End Property

		Public Overridable Sub reset()
			position = 0
		End Sub

		Public Overridable Function resetSupported() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function nextRecord() As Record
			Throw New System.NotSupportedException("Not supported")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Throw New System.NotSupportedException("Not supported")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Property Listeners As IList(Of RecordListener)
			Get
				Return recordReader.getListeners()
			End Get
			Set(ByVal listeners() As RecordListener)
				recordReader.setListeners(listeners)
			End Set
		End Property


		Public Overridable WriteOnly Property Listeners As ICollection(Of RecordListener)
			Set(ByVal listeners As ICollection(Of RecordListener))
				recordReader.setListeners(listeners)
			End Set
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose()
			recordReader.Dispose()
		End Sub

		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
				recordReader.Conf = conf
			End Set
			Get
				Return recordReader.Conf
			End Get
		End Property

	End Class

End Namespace