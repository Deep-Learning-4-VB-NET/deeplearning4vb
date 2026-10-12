Imports System
Imports System.IO
Imports Configuration = org.datavec.api.conf.Configuration
Imports InputSplit = org.datavec.api.split.InputSplit

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

Namespace org.datavec.api.split.partition


	Public Class NumberOfRecordsPartitioner
		Implements Partitioner

		Private locations() As URI
		Private recordsPerFile As Integer = DEFAULT_RECORDS_PER_FILE
		'all records in to 1 file
		Public Const DEFAULT_RECORDS_PER_FILE As Integer = -1

		Public Const RECORDS_PER_FILE_CONFIG As String = "org.datavec.api.split.partition.numrecordsperfile"
		Private numRecordsSoFar As Integer = 0
		Private currLocation As Integer
		Private inputSplit As org.datavec.api.Split.InputSplit
		Private current As Stream
		Private doneWithCurrentLocation As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field totalRecordsWritten was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalRecordsWritten_Conflict As Integer

		Public Overridable Function totalRecordsWritten() As Integer Implements Partitioner.totalRecordsWritten
			Return totalRecordsWritten_Conflict
		End Function

		Public Overridable Function numRecordsWritten() As Integer Implements Partitioner.numRecordsWritten
			Return numRecordsSoFar
		End Function

		Public Overridable Function numPartitions() As Integer Implements Partitioner.numPartitions
			'possible it's a directory
			If locations.Length < 2 Then

				If locations.Length > 0 AndAlso locations(0).isAbsolute() Then
					Return recordsPerFile
				'append all results to 1 file when -1
				Else
					Return 1
				End If
			End If

			'otherwise it's a series of specified files.
			Return locations.Length \ recordsPerFile
		End Function

		Public Overridable Sub init(ByVal inputSplit As InputSplit) Implements Partitioner.init
			Me.locations = inputSplit.locations()
			Me.inputSplit = inputSplit

		End Sub

		Public Overridable Sub init(ByVal configuration As Configuration, ByVal split As InputSplit) Implements Partitioner.init
			init(split)
			Me.recordsPerFile = configuration.getInt(RECORDS_PER_FILE_CONFIG,DEFAULT_RECORDS_PER_FILE)
		End Sub

		Public Overridable Sub updatePartitionInfo(ByVal metadata As PartitionMetaData) Implements Partitioner.updatePartitionInfo
			Me.numRecordsSoFar += metadata.getNumRecordsUpdated()
			Me.totalRecordsWritten_Conflict += metadata.getNumRecordsUpdated()
			If numRecordsSoFar >= recordsPerFile AndAlso recordsPerFile > 0 Then
				doneWithCurrentLocation = True
			End If
		End Sub

		Public Overridable Function needsNewPartition() As Boolean Implements Partitioner.needsNewPartition
			doneWithCurrentLocation = numRecordsSoFar >= recordsPerFile AndAlso recordsPerFile > 0
			Return recordsPerFile > 0 AndAlso numRecordsSoFar >= recordsPerFile OrElse doneWithCurrentLocation
		End Function

		Public Overridable Function openNewStream() As Stream Implements Partitioner.openNewStream
			'reset status of location
			doneWithCurrentLocation = False
			'ensure count is 0 for records so far for current record
			numRecordsSoFar = 0

			'only append when directory, also ensure we can bootstrap and we can write to the current location
			If currLocation >= locations.Length - 1 AndAlso locations.Length >= 1 AndAlso needsNewPartition() OrElse inputSplit.needsBootstrapForWrite() OrElse locations.Length < 1 OrElse currLocation >= locations.Length OrElse Not inputSplit.canWriteToLocation(locations(currLocation)) AndAlso needsNewPartition() Then

				Dim newInput As String = inputSplit.addNewLocation()
				Try
					Dim ret As Stream = inputSplit.openOutputStreamFor(newInput)
					Me.current = ret
					Return ret
				Catch e As Exception
					Throw New System.InvalidOperationException(e)
				End Try

			Else
				Try
					Dim ret As Stream = inputSplit.openOutputStreamFor(locations(currLocation).ToString())
					currLocation += 1
					Me.current = ret
					Return ret
				Catch e As Exception
					Throw New System.InvalidOperationException(e)
				End Try
			End If

		End Function

		Public Overridable Function currentOutputStream() As Stream Implements Partitioner.currentOutputStream
			If current Is Nothing Then
				current = openNewStream()
			End If
			Return current
		End Function
	End Class

End Namespace