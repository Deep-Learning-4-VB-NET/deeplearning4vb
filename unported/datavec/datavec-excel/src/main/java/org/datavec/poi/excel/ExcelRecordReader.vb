Imports System
Imports System.Collections.Generic
Imports System.IO
Imports org.apache.poi.ss.usermodel
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports FileRecordReader = org.datavec.api.records.reader.impl.FileRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports BooleanWritable = org.datavec.api.writable.BooleanWritable
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.poi.excel


	<Serializable>
	Public Class ExcelRecordReader
		Inherits FileRecordReader

		'originally from CSVRecordReader
		Private skippedLines As Boolean = False
		Protected Friend skipNumLines As Integer = 0
		Public Shared ReadOnly SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"

		Private sheetIterator As IEnumerator(Of Sheet)
		Private rows As IEnumerator(Of Row)
		' Create a DataFormatter to format and get each cell's value as String
		Private dataFormatter As New DataFormatter()
		Private currWorkBook As Workbook
		'we should ensure that the number of columns is consistent across all worksheets
		Private numColumns As Integer = -1

		''' <summary>
		''' Skip skipNumLines number of lines </summary>
		''' <param name="skipNumLines"> the number of lines to skip </param>
		Public Sub New(ByVal skipNumLines As Integer)
			Me.skipNumLines = skipNumLines
		End Sub



		Public Sub New()
			Me.New(0)
		End Sub

		Public Overrides Function hasNext() As Boolean
			If Not skipLines() Then
				Throw New NoSuchElementException("No next element found!")
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return skipLines() AndAlso MyBase.hasNext() OrElse sheetIterator IsNot Nothing AndAlso sheetIterator.hasNext() OrElse rows IsNot Nothing AndAlso rows.hasNext()
		End Function


		Private Function skipLines() As Boolean
			If Not skippedLines AndAlso skipNumLines > 0 Then
				For i As Integer = 0 To skipNumLines - 1
					If Not MyBase.hasNext() Then
						Return False
					End If
					MyBase.next()
				Next i
				skippedLines = True
			End If
			Return True
		End Function

		Public Overrides Function [next]() As IList(Of Writable)
			Return nextRecord().getRecord()
		End Function

		Public Overrides Function nextRecord() As Record
			'start at top tracking rows
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If rows IsNot Nothing AndAlso rows.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim currRow As Row = rows.next()
				Dim ret As IList(Of Writable) = New List(Of Writable)(currRow.getLastCellNum())
				For Each cell As Cell In currRow
					Dim cellValue As String = dataFormatter.formatCellValue(cell)
					ret.Add(New Text(cellValue))
				Next cell
				Dim record As Record = New org.datavec.api.records.impl.Record(ret, New RecordMetaDataIndex(currRow.getRowNum(), MyBase.currentUri, GetType(ExcelRecordReader)))
				Return record
			' next track sheets
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			ElseIf sheetIterator IsNot Nothing AndAlso sheetIterator.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim sheet As Sheet = sheetIterator.next()
				rows = sheet.rowIterator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim currRow As Row = rows.next()
				Dim record As Record = New org.datavec.api.records.impl.Record(rowToRecord(currRow), New RecordMetaDataIndex(currRow.getRowNum(), MyBase.currentUri, GetType(ExcelRecordReader)))
				Return record

			End If


			'finally extract workbooks from files and iterate over those starting again at top
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Using [is] As Stream = streamCreatorFn.apply(MyBase.locationsIterator.next())
					' Creating a Workbook from an Excel file (.xls or .xlsx)
					Try
						If currWorkBook IsNot Nothing Then
							currWorkBook.close()
						End If
        
						Me.currWorkBook = WorkbookFactory.create([is])
						Me.sheetIterator = currWorkBook.sheetIterator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim sheet As Sheet = sheetIterator.next()
						rows = sheet.rowIterator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim currRow As Row = rows.next()
						Dim record As Record = New org.datavec.api.records.impl.Record(rowToRecord(currRow), New RecordMetaDataIndex(currRow.getRowNum(), MyBase.currentUri, GetType(ExcelRecordReader)))
						Return record
        
					Catch e As Exception
						Throw New System.InvalidOperationException("Error processing row", e)
					End Try
					End Using
			Catch e As IOException
				Throw New Exception("Error reading from stream", e)
			End Try

		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			MyBase.initialize(conf, split)
			Me.skipNumLines = conf.getInt(SKIP_NUM_LINES,0)
		End Sub

		Public Overrides Sub reset()
			MyBase.reset()
			skippedLines = False
		End Sub



		Private Function rowToRecord(ByVal currRow As Row) As IList(Of Writable)
			If numColumns < 0 Then
				numColumns = currRow.getLastCellNum()
			End If

			If currRow.getLastCellNum() <> numColumns Then
				Throw New System.InvalidOperationException("Invalid number of columns for row. First number of columns found was " & numColumns & " but row " & currRow.getRowNum() & " was " & currRow.getLastCellNum())
			End If

			Dim ret As IList(Of Writable) = New List(Of Writable)(currRow.getLastCellNum())
			For Each cell As Cell In currRow
				Dim cellValue As String = dataFormatter.formatCellValue(cell)
				Select Case cell.getCellType()
					Case BLANK
						ret.Add(New Text(""))
					Case [STRING]
						ret.Add(New Text(""))
					Case [BOOLEAN]
						ret.Add(New BooleanWritable(Convert.ToBoolean(cellValue)))
					Case NUMERIC
						ret.Add(New DoubleWritable(Double.Parse(cellValue)))
					Case Else
						ret.Add(New Text(cellValue))
				End Select
			Next cell

			Return ret

		End Function


	End Class

End Namespace