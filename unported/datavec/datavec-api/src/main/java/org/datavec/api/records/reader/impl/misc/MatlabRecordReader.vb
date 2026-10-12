Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileRecordReader = org.datavec.api.records.reader.impl.FileRecordReader
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
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

Namespace org.datavec.api.records.reader.impl.misc



	<Serializable>
	Public Class MatlabRecordReader
		Inherits FileRecordReader

		Private records As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
		Private currIter As IEnumerator(Of IList(Of Writable))

		Public Overrides Function hasNext() As Boolean
			Return MyBase.hasNext()
		End Function

		Public Overrides Function [next]() As IList(Of Writable)
			'use the current iterator
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If currIter IsNot Nothing AndAlso currIter.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return New List(Of Writable)(currIter.next())
			End If
			records.Clear()
			'next file
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As IList(Of Writable) = MyBase.next()
			Dim val As String = next_Conflict.GetEnumerator().next().ToString()
			Dim reader As New StringReader(val)
			Dim c As Integer
			Dim chr As Char
			Dim fileContent As StringBuilder
			Dim isComment As Boolean


			Dim currRecord As IList(Of Writable) = New List(Of Writable)()
			fileContent = New StringBuilder()
			isComment = False
			records.Add(currRecord)
			Try
				' determine number of attributes
				c = reader.read()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((c = reader.read()) != -1)
				Do While c <> -1
					chr = ChrW(c)

					' comment found?
					If chr = "%"c Then
						isComment = True
					End If

					' end of line reached
					If (chr = ControlChars.Lf) OrElse (chr = ControlChars.Cr) Then
						isComment = False
						If fileContent.Length > 0 Then
							currRecord.Add(New DoubleWritable(Convert.ToDouble(fileContent.ToString())))
						End If

						If currRecord.Count > 0 Then
							currRecord = New List(Of Writable)()
							records.Add(currRecord)
						End If
						fileContent = New StringBuilder()
						Continue Do
					End If

					' skip till end of comment line
					If isComment Then
						Continue Do
					End If

					' separator found?
					If (chr = ControlChars.Tab) OrElse (chr = " "c) Then
						If fileContent.Length > 0 Then
							currRecord.Add(New DoubleWritable(Convert.ToDouble(fileContent.ToString())))
							fileContent = New StringBuilder()
						End If
					Else
						fileContent.Append(chr)
					End If
						c = reader.read()
				Loop

				' last number?
				If fileContent.Length > 0 Then
					currRecord.Add(New DoubleWritable(Convert.ToDouble(fileContent.ToString())))
				End If


				currIter = records.GetEnumerator()

			Catch ex As Exception
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
				Throw New System.InvalidOperationException("Unable to determine structure as Matlab ASCII file: " & ex)
			End Try
			Throw New System.InvalidOperationException("Strange state detected")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException("Reading Matlab data from DataInputStream: not yet implemented")
		End Function
	End Class

End Namespace