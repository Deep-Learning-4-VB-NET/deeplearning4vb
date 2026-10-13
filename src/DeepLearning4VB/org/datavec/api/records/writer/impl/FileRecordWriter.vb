Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Configuration = org.datavec.api.conf.Configuration
Imports RecordWriter = org.datavec.api.records.writer.RecordWriter
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Partitioner = org.datavec.api.split.partition.Partitioner

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

Namespace org.datavec.api.records.writer.impl



	Public MustInherit Class FileRecordWriter
		Implements RecordWriter

		Public MustOverride Function writeBatch(ByVal batch As IList(Of IList(Of org.datavec.api.writable.Writable))) As org.datavec.api.split.partition.PartitionMetaData
		Public MustOverride Function write(ByVal record As IList(Of org.datavec.api.writable.Writable)) As org.datavec.api.split.partition.PartitionMetaData
		Public MustOverride Function supportsBatch() As Boolean Implements RecordWriter.supportsBatch

		Public Shared ReadOnly DEFAULT_CHARSET As Charset = StandardCharsets.UTF_8

		Protected Friend [out] As DataOutputStream
		Public Shared ReadOnly NEW_LINE As String = vbLf

		Protected Friend encoding As Charset = DEFAULT_CHARSET

		Protected Friend partitioner As org.datavec.api.Split.partition.Partitioner

'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As Configuration

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit inputSplit, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overridable Sub initialize(ByVal inputSplit As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
			partitioner.init(inputSplit)
			[out] = New DataOutputStream(partitioner.currentOutputStream())
			Me.partitioner = partitioner

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration configuration, org.datavec.api.split.InputSplit split, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overridable Sub initialize(ByVal configuration As Configuration, ByVal split As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
			Conf = configuration
			partitioner.init(configuration, split)
			initialize(split, partitioner)
		End Sub

		Public Overridable Sub Dispose() Implements RecordWriter.close
			If [out] IsNot Nothing Then
				Try
					[out].flush()
					[out].close()
				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try

			End If
		End Sub

		Public Overridable Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property

	End Class

End Namespace