Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports BytesQualityAnalysisState = org.datavec.api.transform.analysis.quality.bytes.BytesQualityAnalysisState
Imports CategoricalQualityAnalysisState = org.datavec.api.transform.analysis.quality.categorical.CategoricalQualityAnalysisState
Imports IntegerQualityAnalysisState = org.datavec.api.transform.analysis.quality.integer.IntegerQualityAnalysisState
Imports LongQualityAnalysisState = org.datavec.api.transform.analysis.quality.longq.LongQualityAnalysisState
Imports RealQualityAnalysisState = org.datavec.api.transform.analysis.quality.real.RealQualityAnalysisState
Imports StringQualityAnalysisState = org.datavec.api.transform.analysis.quality.string.StringQualityAnalysisState
Imports TimeQualityAnalysisState = org.datavec.api.transform.analysis.quality.time.TimeQualityAnalysisState
Imports org.datavec.api.transform.metadata
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function

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

Namespace org.datavec.api.transform.analysis.quality


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class QualityAnalysisAddFunction implements org.nd4j.common.function.BiFunction<java.util.List<QualityAnalysisState>, java.util.List<org.datavec.api.writable.Writable>, java.util.List<QualityAnalysisState>>, java.io.Serializable
	<Serializable>
	Public Class QualityAnalysisAddFunction
		Implements BiFunction(Of IList(Of QualityAnalysisState), IList(Of Writable), IList(Of QualityAnalysisState))

		Private schema As Schema

		Public Overridable Function apply(ByVal analysisStates As IList(Of QualityAnalysisState), ByVal writables As IList(Of Writable)) As IList(Of QualityAnalysisState)
			If analysisStates Is Nothing Then
				analysisStates = New List(Of QualityAnalysisState)()
				Dim columnTypes As IList(Of ColumnType) = schema.getColumnTypes()
				Dim columnMetaDatas As IList(Of ColumnMetaData) = schema.getColumnMetaData()
				For i As Integer = 0 To columnTypes.Count - 1
					Select Case columnTypes(i)
						Case String
							analysisStates.Add(New StringQualityAnalysisState(DirectCast(columnMetaDatas(i), StringMetaData)))
						Case Integer?
							analysisStates.Add(New IntegerQualityAnalysisState(DirectCast(columnMetaDatas(i), IntegerMetaData)))
						Case Long?
							analysisStates.Add(New LongQualityAnalysisState(DirectCast(columnMetaDatas(i), LongMetaData)))
						Case Double?
							analysisStates.Add(New RealQualityAnalysisState(DirectCast(columnMetaDatas(i), DoubleMetaData)))
						Case Categorical
							analysisStates.Add(New CategoricalQualityAnalysisState(DirectCast(columnMetaDatas(i), CategoricalMetaData)))
						Case Time
							analysisStates.Add(New TimeQualityAnalysisState(DirectCast(columnMetaDatas(i), TimeMetaData)))
						Case Bytes
							analysisStates.Add(New BytesQualityAnalysisState()) 'TODO
						Case Else
							Throw New System.ArgumentException("Unknown column type: " & columnTypes(i))
					End Select
				Next i
			End If

			Dim size As Integer = analysisStates.Count
			If size <> writables.Count Then
				Throw New System.InvalidOperationException("Writables list and number of states does not match (" & writables.Count & " vs " & size & ")")
			End If
			For i As Integer = 0 To size - 1
				analysisStates(i).add(writables(i))
			Next i

			Return analysisStates
		End Function
	End Class

End Namespace