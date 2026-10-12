Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports InputSplit = org.datavec.api.split.InputSplit
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = org.datavec.api.split.partition.Partitioner
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports ArrowConverter = org.datavec.arrow.ArrowConverter

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


	Public Class ArrowRecordWriter
		Implements RecordWriter

		Private configuration As Configuration
		Private schema As Schema
		Private partitioner As org.datavec.api.Split.partition.Partitioner

		Public Sub New(ByVal schema As Schema)
			Me.schema = schema
		End Sub

		Public Overridable Function supportsBatch() As Boolean Implements RecordWriter.supportsBatch
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit inputSplit, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overridable Sub initialize(ByVal inputSplit As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
			Me.partitioner = partitioner
			partitioner.init(inputSplit)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration configuration, org.datavec.api.split.InputSplit split, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overridable Sub initialize(ByVal configuration As Configuration, ByVal split As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
			Conf = configuration
			Me.partitioner = partitioner
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData write(java.util.List<org.datavec.api.writable.Writable> record) throws java.io.IOException
		Public Overridable Function write(ByVal record As IList(Of Writable)) As PartitionMetaData Implements RecordWriter.write
			Return writeBatch(New List(Of IList(Of Writable)) From {record})
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<org.datavec.api.writable.Writable>> batch) throws java.io.IOException
		Public Overridable Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData Implements RecordWriter.writeBatch
			If partitioner.needsNewPartition() Then
				partitioner.currentOutputStream().Flush()
				partitioner.currentOutputStream().Close()
				partitioner.openNewStream()
			End If

			If TypeOf batch Is ArrowWritableRecordBatch Then
				Dim arrowWritableRecordBatch As ArrowWritableRecordBatch = CType(batch, ArrowWritableRecordBatch)
				ArrowConverter.writeRecordBatchTo(arrowWritableRecordBatch,schema,partitioner.currentOutputStream())
			Else
				ArrowConverter.writeRecordBatchTo(batch, schema, partitioner.currentOutputStream())
			End If

			partitioner.currentOutputStream().Flush()
			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(batch.Count).build()
		End Function

		Public Overridable Sub Dispose() Implements RecordWriter.close
		End Sub

		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.configuration = conf
			End Set
			Get
				Return configuration
			End Get
		End Property

	End Class

End Namespace