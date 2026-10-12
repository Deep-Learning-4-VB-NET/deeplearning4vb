Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
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

Namespace org.deeplearning4j.spark.datavec.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SparkSourceDummyReader implements org.datavec.api.records.reader.RecordReader, java.io.Serializable
	<Serializable>
	Public Class SparkSourceDummyReader
		Implements RecordReader

		Private readerIdx As Integer

		''' <param name="readerIdx"> Index of the reader, in terms of the RDD that matches it. For a single RDD as input, this
		'''                  is always 0; for 2 RDDs used as input, this would be 0 or 1, depending on whether it should pull
		'''                  values from the first or second RDD. Note that the indexing for RDDs doesn't depend on the
		'''                  presence of sequence RDDs - they are indexed separately. </param>
		Public Sub New(ByVal readerIdx As Integer)
			Me.readerIdx = readerIdx
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit inputSplit) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal inputSplit As InputSplit) Implements RecordReader.initialize
			' No op 
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration configuration, org.datavec.api.split.InputSplit inputSplit) throws IOException, InterruptedException
		Public Overridable Sub initialize(ByVal configuration As Configuration, ByVal inputSplit As InputSplit) Implements RecordReader.initialize
			' No op 
		End Sub

		Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
			Return False
		End Function

		Public Overridable Function [next](ByVal i As Integer) As IList(Of IList(Of Writable)) Implements RecordReader.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function [next]() As IList(Of Writable) Implements RecordReader.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
			Return False
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Sub reset() Implements RecordReader.reset
		End Sub
		Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable) Implements RecordReader.record
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> list) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal list As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.getListeners
			Get
				Return Collections.emptyList()
			End Get
			Set(ByVal recordListeners() As RecordListener)
			End Set
		End Property
		Public Overridable WriteOnly Property Listeners Implements RecordReader.setListeners As ICollection(Of RecordListener)
			Set(ByVal collection As ICollection(Of RecordListener))
			End Set
		End Property
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose()
		End Sub
		Public Overridable Property Conf As Configuration
			Set(ByVal configuration As Configuration)
			End Set
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property
	End Class
End Namespace