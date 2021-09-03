Imports System.Collections.Generic
Imports HSSFWorkbook = org.apache.poi.hssf.usermodel.HSSFWorkbook
Imports Cell = org.apache.poi.ss.usermodel.Cell
Imports Row = org.apache.poi.ss.usermodel.Row
Imports Sheet = org.apache.poi.ss.usermodel.Sheet
Imports Workbook = org.apache.poi.ss.usermodel.Workbook
Imports XSSFWorkbook = org.apache.poi.xssf.usermodel.XSSFWorkbook
Imports Configuration = org.datavec.api.conf.Configuration
Imports FileRecordWriter = org.datavec.api.records.writer.impl.FileRecordWriter
Imports InputSplit = org.datavec.api.split.InputSplit
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = org.datavec.api.split.partition.Partitioner
Imports org.datavec.api.writable

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

Namespace org.datavec.poi.excel


	Public Class ExcelRecordWriter
		Inherits FileRecordWriter

		Public Const WORKSHEET_NAME As String = "org.datavec.poi.excel.worksheet.name"
		Public Const FILE_TYPE As String = "org.datavec.poi.excel.type"


		Public Const DEFAULT_FILE_TYPE As String = "xlsx"
		Public Const DEFAULT_WORKSHEET_NAME As String = "datavec-worksheet"


		Private workBookName As String = DEFAULT_WORKSHEET_NAME
		Private fileTypeToUse As String = DEFAULT_FILE_TYPE

		Private sheet As Sheet
		Private workbook As Workbook



		Private Sub createRow(ByVal rowNum As Integer, ByVal numCols As Integer, ByVal value As IList(Of Writable))
			' Create a Row
			Dim headerRow As Row = sheet.createRow(rowNum)
			Dim col As Integer = 0
			For Each writable As Writable In value
				' Creating cells
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.apache.poi.ss.usermodel.Cell cell = headerRow.createCell(col++);
				Dim cell As Cell = headerRow.createCell(col)
					col += 1
				setValueForCell(cell,writable)


			Next writable

			' Resize all columns to fit the content size
			For i As Integer = 0 To numCols - 1
				sheet.autoSizeColumn(i)
			Next i
		End Sub

		Private Sub setValueForCell(ByVal cell As Cell, ByVal value As Writable)
			If TypeOf value Is DoubleWritable OrElse TypeOf value Is LongWritable OrElse TypeOf value Is FloatWritable OrElse TypeOf value Is IntWritable Then
				cell.setCellValue(value.toDouble())
			ElseIf TypeOf value Is BooleanWritable Then
				cell.setCellValue(DirectCast(value, BooleanWritable).get())
			ElseIf TypeOf value Is Text Then
				cell.setCellValue(value.ToString())
			End If

		End Sub


		Public Overrides Function supportsBatch() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit inputSplit, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overrides Sub initialize(ByVal inputSplit As InputSplit, ByVal partitioner As Partitioner)
			Me.conf_Conflict = New Configuration()
			Me.partitioner = partitioner
			partitioner.init(inputSplit)
			[out] = New DataOutputStream(partitioner.currentOutputStream())
			initPoi()


		End Sub

		Private Sub initPoi()
			If fileTypeToUse.Equals("xlsx") Then
				workbook = New XSSFWorkbook()
			Else
				'xls
				workbook = New HSSFWorkbook()
			End If

			Me.sheet = workbook.createSheet(workBookName)


		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration configuration, org.datavec.api.split.InputSplit split, org.datavec.api.split.partition.Partitioner partitioner) throws Exception
		Public Overrides Sub initialize(ByVal configuration As Configuration, ByVal split As InputSplit, ByVal partitioner As Partitioner)
			Me.workBookName = configuration.get(WORKSHEET_NAME,DEFAULT_WORKSHEET_NAME)
			Me.fileTypeToUse = configuration.get(FILE_TYPE,DEFAULT_FILE_TYPE)
			Me.conf_Conflict = configuration
			partitioner.init(split)
			[out] = New DataOutputStream(partitioner.currentOutputStream())
			initPoi()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData write(java.util.List<Writable> record) throws java.io.IOException
		Public Overridable Overloads Function write(ByVal record As IList(Of Writable)) As PartitionMetaData
			createRow(partitioner.numRecordsWritten(),record.Count,record)
			reinitIfNecessary()
			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(1).build()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<Writable>> batch) throws java.io.IOException
		Public Overridable Overloads Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData
			Dim numSoFar As Integer = 0
			For Each record As IList(Of Writable) In batch
				createRow(partitioner.numRecordsWritten() + numSoFar, record.Count, record)
				reinitIfNecessary()
				numSoFar += 1
			Next record

			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(batch.Count).build()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void reinitIfNecessary() throws java.io.IOException
		Private Sub reinitIfNecessary()
			If partitioner.needsNewPartition() Then
				workbook.write([out])
				[out].flush()
				[out].close()
				workbook.close()
				initPoi()
				Me.out = New DataOutputStream(partitioner.openNewStream())
			End If
		End Sub

		Public Overrides Sub Dispose()
			If workbook IsNot Nothing Then
				Try
					If [out] IsNot Nothing Then
						workbook.write([out])
						[out].flush()
						[out].close()
					End If

					workbook.close()

				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try
			End If
		End Sub

	End Class

End Namespace