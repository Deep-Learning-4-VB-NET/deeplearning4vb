Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports Schema = org.datavec.api.transform.schema.Schema

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

Namespace org.datavec.api.transform.quality


	''' <summary>
	'''A report outlining number of invalid and missing features
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class DataQualityAnalysis
	Public Class DataQualityAnalysis

		Private schema As Schema
		Private columnQualityList As IList(Of ColumnQuality)


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			Dim nCol As Integer = schema.numColumns()

			Dim maxNameLength As Integer = 0
			For Each s As String In schema.getColumnNames()
				maxNameLength = Math.Max(maxNameLength, s.Length)
			Next s

			'Header:
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			sb.Append(String.Format("{0,-6}", "idx")).Append(String.Format("%-" & (maxNameLength + 8) & "s", "name")).Append(String.Format("{0,-15}", "type")).Append(String.Format("{0,-10}", "quality")).Append("details").Append(vbLf)

			For i As Integer = 0 To nCol - 1
				Dim colName As String = schema.getName(i)
				Dim type As ColumnType = schema.getType(i)
				Dim columnQuality As ColumnQuality = columnQualityList(i)
				Dim pass As Boolean = columnQuality.getCountInvalid() = 0L AndAlso columnQuality.getCountMissing() = 0L
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Dim paddedName As String = String.Format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
				sb.Append(String.Format("{0,-6:D}", i)).Append(paddedName).Append(String.Format("{0,-15}", type)).Append(String.Format("{0,-10}", (If(pass, "ok", "FAIL")))).Append(columnQuality).Append(vbLf)
			Next i

			Return sb.ToString()
		End Function

	End Class

End Namespace