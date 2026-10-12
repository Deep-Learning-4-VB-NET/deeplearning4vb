Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports FileRecordWriter = org.datavec.api.records.writer.impl.FileRecordWriter
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
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

Namespace org.datavec.api.records.writer.impl.misc



	Public Class MatlabRecordWriter
		Inherits FileRecordWriter

		Public Sub New()
		End Sub


		Public Overrides Function supportsBatch() As Boolean
			Return False
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData write(java.util.List<org.datavec.api.writable.Writable> record) throws java.io.IOException
		Public Overridable Overloads Function write(ByVal record As IList(Of Writable)) As PartitionMetaData
			Dim result As New StringBuilder()

			Dim count As Integer = 0
			For Each w As Writable In record
				' attributes
				If count > 0 Then
					Dim tabs As Boolean = False
					result.Append((If(tabs, vbTab, " ")))
				End If
				result.Append(w.ToString())
				count += 1

			Next w

			[out].write(result.ToString().GetBytes())
			[out].write(NEW_LINE.GetBytes())

			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(1).build()

		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<org.datavec.api.writable.Writable>> batch) throws java.io.IOException
		Public Overridable Overloads Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData
			Throw New NotImplementedException("writeBatch is not supported on " & Me.GetType().Name)
		End Function
	End Class

End Namespace