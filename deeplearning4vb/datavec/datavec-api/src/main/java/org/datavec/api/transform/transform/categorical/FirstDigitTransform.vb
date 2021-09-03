Imports System
Imports System.Collections.Generic
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.categorical


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnIdx"}) public class FirstDigitTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class FirstDigitTransform
		Inherits BaseTransform

		Public Const OTHER_CATEGORY As String = "Other"

		''' <summary>
		''' Mode determines how non-numerical entries should be handled:<br>
		''' EXCEPTION_ON_INVALID: output has 10 category values ("0", ..., "9"), and any non-numerical values result in an exception<br>
		''' INCLUDE_OTHER_CATEGORY: output has 11 category values ("0", ..., "9", "Other"), all non-numerical values are mapped to "Other"<br>
		''' </summary>
		Public Enum Mode
			EXCEPTION_ON_INVALID
			INCLUDE_OTHER_CATEGORY
		End Enum

		Protected Friend inputColumn As String
		Protected Friend outputColumn As String
		Protected Friend mode As Mode
		Private columnIdx As Integer = -1

		''' <param name="inputColumn">  Input column name </param>
		''' <param name="outputColumn"> Output column name. If same as input, input column is replaced </param>
		''' <param name="mode"> See <seealso cref="FirstDigitTransform.Mode"/> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FirstDigitTransform(@JsonProperty("inputColumn") String inputColumn, @JsonProperty("outputColumn") String outputColumn, @JsonProperty("mode") Mode mode)
		Public Sub New(ByVal inputColumn As String, ByVal outputColumn As String, ByVal mode As Mode)
			Me.inputColumn = inputColumn
			Me.outputColumn = outputColumn
			Me.mode = mode
		End Sub

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			Dim i As Integer=0
			Dim inplace As Boolean = inputColumn.Equals(outputColumn)
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if(i++ == columnIdx)
				If i = columnIdx Then
						i += 1
					If Not inplace Then
						[out].Add(w)
					End If

					Dim s As String = w.ToString()
					If s.Length = 0 Then
						If mode = Mode.INCLUDE_OTHER_CATEGORY Then
							[out].Add(New Text(OTHER_CATEGORY))
						Else
							Throw New System.InvalidOperationException("Encountered empty string in FirstDigitTransform that is set to Mode.EXCEPTION_ON_INVALID." & " Either data contains an invalid (non-numerical) entry, or set FirstDigitTransform to Mode.INCLUDE_OTHER_CATEGORY")
						End If
					Else
						Dim first As Char = s.Chars(0)
						If first = "-"c AndAlso s.Length > 1 Then
							'Handle negatives
							first = s.Chars(1)
						End If
						If first >= "0"c AndAlso first <= "9"c Then
							[out].Add(New Text(first.ToString()))
						Else
							If mode = Mode.INCLUDE_OTHER_CATEGORY Then
								[out].Add(New Text(OTHER_CATEGORY))
							Else
								Dim s2 As String = s
								If s.Length > 100 Then
									s2 = s2.Substring(0, 100) & "..."
								End If
								Throw New System.InvalidOperationException("Encountered string """ & s2 & """ with non-numerical first character in " & "FirstDigitTransform that is set to Mode.EXCEPTION_ON_INVALID." & " Either data contains an invalid (non-numerical) entry, or set FirstDigitTransform to Mode.INCLUDE_OTHER_CATEGORY")
							End If
						End If
					End If
				Else
						i += 1
					[out].Add(w)
				End If
			Next w
			Return [out]
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function ToString() As String
			Return "FirstDigitTransform(input=""" & inputColumn & """,output=""" & outputColumn & """,mode=" & mode & ")"
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim origNames As IList(Of String) = inputSchema.getColumnNames()
			Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()

			Preconditions.checkState(origNames.Contains(inputColumn), "Input column with name ""%s"" not found in schema", inputColumn)
			Preconditions.checkState(inputColumn.Equals(outputColumn) OrElse Not origNames.Contains(outputColumn), "Output column with name ""%s"" already exists in schema (only allowable if input column == output column)", outputColumn)

			Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(origNames.Count+1)
			For i As Integer = 0 To origNames.Count - 1
				Dim s As String = origNames(i)
				If s.Equals(inputColumn) Then
					If Not outputColumn.Equals(inputColumn) Then
						outMeta.Add(origMeta(i))
					End If

					Dim l As IList(Of String) = Collections.unmodifiableList(If(mode = Mode.INCLUDE_OTHER_CATEGORY, Arrays.asList("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", OTHER_CATEGORY), Arrays.asList("0", "1", "2", "3", "4", "5", "6", "7", "8", "9")))

					Dim cm As New CategoricalMetaData(outputColumn, l)

					outMeta.Add(cm)
				Else
					outMeta.Add(origMeta(i))
				End If
			Next i

			Return inputSchema.newSchema(outMeta)
		End Function

		Public Overrides Function outputColumnName() As String
			Return outputColumn
		End Function

		Public Overrides Function outputColumnNames() As String()
			Return New String(){outputColumn}
		End Function

		Public Overrides Function columnNames() As String()
			Return New String(){inputColumn}
		End Function

		Public Overrides Function columnName() As String
			Return inputColumn
		End Function

		Public Overrides WriteOnly Property InputSchema As Schema
			Set(ByVal schema As Schema)
				MyBase.InputSchema = schema
    
				columnIdx = schema.getIndexOfColumn(inputColumn)
				Preconditions.checkState(columnIdx >= 0, "Input column ""%s"" not found in schema", inputColumn)
			End Set
		End Property
	End Class

End Namespace