Imports System.Collections.Generic
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

Namespace org.datavec.api.records.writer.impl.csv



	Public Class CSVRecordWriter
		Inherits FileRecordWriter

		Public Const DEFAULT_DELIMITER As String = ","

		Private ReadOnly delimBytes() As SByte
		Private firstLine As Boolean = True

		Public Sub New()
			delimBytes = DEFAULT_DELIMITER.GetBytes(encoding)
		End Sub


		Public Overrides Function supportsBatch() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<org.datavec.api.writable.Writable>> batch) throws java.io.IOException
		Public Overridable Overloads Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData
			For Each record As IList(Of Writable) In batch
				If record.Count > 0 Then
					'Add new line before appending lines rather than after (avoids newline after last line)
					If Not firstLine Then
						[out].write(NEW_LINE.GetBytes())
					Else
						firstLine = False
					End If

					Dim count As Integer = 0
					Dim last As Integer = record.Count - 1
					For Each w As Writable In record
						[out].write(w.ToString().GetBytes(encoding))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (count++ != last)
						If count <> last Then
								count += 1
							[out].write(delimBytes)
							Else
								count += 1
							End If
					Next w

					[out].flush()
				End If
			Next record

			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(batch.Count).build()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData write(java.util.List<org.datavec.api.writable.Writable> record) throws java.io.IOException
		Public Overridable Overloads Function write(ByVal record As IList(Of Writable)) As PartitionMetaData
			If record.Count > 0 Then
				'Add new line before appending lines rather than after (avoids newline after last line)
				If Not firstLine Then
					[out].write(NEW_LINE.GetBytes())
				Else
					firstLine = False
				End If

				Dim count As Integer = 0
				Dim last As Integer = record.Count - 1
				For Each w As Writable In record
					[out].write(w.ToString().GetBytes(encoding))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (count++ != last)
					If count <> last Then
							count += 1
						[out].write(delimBytes)
						Else
							count += 1
						End If
				Next w

				[out].flush()
			End If

			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(1).build()
		End Function
	End Class

End Namespace