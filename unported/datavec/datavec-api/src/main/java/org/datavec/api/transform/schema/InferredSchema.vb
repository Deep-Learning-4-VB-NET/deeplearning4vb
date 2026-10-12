Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports CSVParser = au.com.bytecode.opencsv.CSVParser
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils

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

Namespace org.datavec.api.transform.schema


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class InferredSchema
	Public Class InferredSchema
		Protected Friend schemaBuilder As Schema.Builder
		Protected Friend pathToCsv As String
		Protected Friend defaultType As DataType
		Protected Friend quote As String

		Private csvParser As New CSVParser()

		Public Sub New(ByVal pathToCsv As String)
			Me.pathToCsv = pathToCsv
			Me.defaultType = System.Enum.Parse(GetType(DataType), "STRING")
		End Sub

		Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType)
			Me.pathToCsv = pathToCsv
			Me.defaultType = defaultType
		End Sub

		Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char)
			Me.pathToCsv = pathToCsv
			Me.defaultType = defaultType
			Me.csvParser = New CSVParser(delimiter)
		End Sub

		Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char, ByVal quote As Char)
			Me.pathToCsv = pathToCsv
			Me.defaultType = defaultType
			Me.csvParser = New CSVParser(delimiter, quote)
		End Sub

		Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char, ByVal quote As Char, ByVal escape As Char)
			Me.pathToCsv = pathToCsv
			Me.defaultType = defaultType
			Me.csvParser = New CSVParser(delimiter, quote, escape)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Schema build() throws java.io.IOException
		Public Overridable Function build() As Schema
			Dim headersAndRows As IList(Of String) = Nothing
			Me.schemaBuilder = New Schema.Builder()

			Try
				headersAndRows = FileUtils.readLines(New File(pathToCsv))
			Catch e As IOException
				log.error("An error occurred while parsing sample CSV for schema", e)
			End Try
			Dim headers As IList(Of String) = parseLine(headersAndRows(0))
			Dim samples As IList(Of String) = parseLine(headersAndRows(1))

			If headers.Count <> samples.Count Then
				Throw New System.InvalidOperationException("CSV headers length does not match number of sample columns. " & "Please check that your CSV is valid, or check the delimiter used to parse the CSV.")
			End If

			For i As Integer = 0 To headers.Count - 1
				inferAndAddType(schemaBuilder, headers(i), samples(i))
			Next i
			Return schemaBuilder.build()
		End Function

		Private Function inferAndAddType(ByVal builder As Schema.Builder, ByVal header As String, ByVal sample As String) As Schema.Builder
			If isParsableAsDouble(sample) Then
				addOn(builder, header, DataType.DOUBLE)
			ElseIf isParsableAsInteger(sample) Then
				addOn(builder, header, DataType.INTEGER)
			ElseIf isParsableAsLong(sample) Then
				addOn(builder, header, DataType.LONG)
			Else
				addOn(builder, header, defaultType)
			End If

			Return schemaBuilder
		End Function

		Private Shared Function addOn(ByVal builder As Schema.Builder, ByVal columnName As String, ByVal columnType As DataType) As Schema.Builder
			Select Case columnType
				Case org.datavec.api.transform.schema.InferredSchema.DataType.DOUBLE
					Return builder.addColumnDouble(columnName, Nothing, Nothing, False, False) 'no nans/inf
				Case org.datavec.api.transform.schema.InferredSchema.DataType.INTEGER
					Return builder.addColumnInteger(columnName)
				Case org.datavec.api.transform.schema.InferredSchema.DataType.LONG
					Return builder.addColumnLong(columnName)
				Case org.datavec.api.transform.schema.InferredSchema.DataType.STRING
					Return builder.addColumnString(columnName)
				Case Else
					Throw New System.ArgumentException("Schema inputs have to be string, integer or double")
			End Select
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.util.List<String> parseLine(String line) throws java.io.IOException
		Private Function parseLine(ByVal line As String) As IList(Of String)
			Dim split() As String = csvParser.parseLine(line)
			Dim ret As New ArrayList()
			Dim var4() As String = split
			Dim var5 As Integer = split.Length

			For var6 As Integer = 0 To var5 - 1
				Dim s As String = var4(var6)
				If Me.quote IsNot Nothing AndAlso s.StartsWith(Me.quote, StringComparison.Ordinal) AndAlso s.EndsWith(Me.quote, StringComparison.Ordinal) Then
					Dim n As Integer = Me.quote.Length
					s = s.Substring(n, (s.Length - n) - n).Replace(Me.quote & Me.quote, Me.quote)
				End If
				ret.Add(s)
			Next var6

			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private static boolean isParsableAsLong(final String s)
		Private Shared Function isParsableAsLong(ByVal s As String) As Boolean
			Try
				Convert.ToInt64(s)
				Return True
			Catch numberFormatException As System.FormatException
				Return False
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private static boolean isParsableAsInteger(final String s)
		Private Shared Function isParsableAsInteger(ByVal s As String) As Boolean
			Try
				Convert.ToInt32(s)
				Return True
			Catch numberFormatException As System.FormatException
				Return False
			End Try
		End Function


'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private static boolean isParsableAsDouble(final String s)
		Private Shared Function isParsableAsDouble(ByVal s As String) As Boolean
			Try
				Convert.ToDouble(s)
				Return True
			Catch numberFormatException As System.FormatException
				Return False
			End Try
		End Function

		Private Enum DataType
			[STRING]
			[INTEGER]
			[DOUBLE]
			[LONG]
		End Enum
	End Class
End Namespace