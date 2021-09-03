Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
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
	Public Class FileBatchRecordReader
		Implements RecordReader

		Private ReadOnly recordReader As RecordReader
		Private ReadOnly fileBatch As FileBatch
		Private position As Integer = 0

		''' <param name="rr">        Underlying record reader to read files from </param>
		''' <param name="fileBatch"> File batch to read files from </param>
		Public Sub New(ByVal rr As RecordReader, ByVal fileBatch As FileBatch)
			Me.recordReader = rr
			Me.fileBatch = fileBatch
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
			'No op
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize
			'No op
		End Sub

		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return False
		End Function

		Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable)) Implements RecordReader.next
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(Math.Min(num, 10000))
			Dim i As Integer=0
			Do While i<num AndAlso hasNext()
				[out].Add([next]())
				i += 1
			Loop
			Return [out]
		End Function

		Public Overridable Function [next]() As IList(Of Writable) Implements RecordReader.next
			Preconditions.checkState(hasNext(), "No next element")

			Dim fileBytes() As SByte = fileBatch.getFileBytes().get(position)
			Dim origPath As String = fileBatch.getOriginalUris().get(position)

			Dim [out] As IList(Of Writable)
			Try
				[out] = recordReader.record(URI.create(origPath), New DataInputStream(New MemoryStream(fileBytes)))
			Catch e As IOException
				Throw New Exception("Error reading from file bytes")
			End Try

			position += 1
			Return [out]
		End Function

		Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
			Return position < fileBatch.getFileBytes().size()
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.getLabels
			Get
				Return recordReader.getLabels()
			End Get
		End Property

		Public Overridable Sub reset() Implements RecordReader.reset
			position = 0
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable) Implements RecordReader.record
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
			Return New org.datavec.api.records.impl.Record([next](), Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
			Return recordReader.loadFromMetaData(recordMetaData)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
			Return recordReader.loadFromMetaData(recordMetaDatas)
		End Function

		Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.getListeners
			Get
				Return Nothing
			End Get
			Set(ByVal listeners() As RecordListener)
				recordReader.setListeners(listeners)
			End Set
		End Property


		Public Overridable WriteOnly Property Listeners Implements RecordReader.setListeners As ICollection(Of RecordListener)
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