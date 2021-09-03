Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.analysis
Imports org.datavec.api.transform.analysis.counter
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

Namespace org.datavec.local.transforms.analysis.aggregate


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class AnalysisAddFunction implements org.nd4j.common.function.BiFunction<java.util.List<org.datavec.api.transform.analysis.AnalysisCounter>, java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.transform.analysis.AnalysisCounter>>
	Public Class AnalysisAddFunction
		Implements BiFunction(Of IList(Of AnalysisCounter), IList(Of Writable), IList(Of AnalysisCounter))

		Private schema As Schema

		Public Overridable Function apply(ByVal analysisCounters As IList(Of AnalysisCounter), ByVal writables As IList(Of Writable)) As IList(Of AnalysisCounter)
			If analysisCounters Is Nothing Then
				analysisCounters = New List(Of AnalysisCounter)()
				Dim columnTypes As IList(Of ColumnType) = schema.getColumnTypes()
				For Each ct As ColumnType In columnTypes
					Select Case ct.innerEnumValue
						Case ColumnType.InnerEnum.String
							analysisCounters.Add(New StringAnalysisCounter())
						Case Integer?
							analysisCounters.Add(New IntegerAnalysisCounter())
						Case Long?
							analysisCounters.Add(New LongAnalysisCounter())
						Case Double?
							analysisCounters.Add(New DoubleAnalysisCounter())
						Case ColumnType.InnerEnum.Categorical
							analysisCounters.Add(New CategoricalAnalysisCounter())
						Case ColumnType.InnerEnum.Time
							analysisCounters.Add(New LongAnalysisCounter())
						Case ColumnType.InnerEnum.Bytes
							analysisCounters.Add(New BytesAnalysisCounter())
						Case ColumnType.InnerEnum.NDArray
							analysisCounters.Add(New NDArrayAnalysisCounter())
						Case Else
							Throw New System.ArgumentException("Unknown column type: " & ct)
					End Select
				Next ct
			End If

			Dim size As Integer = analysisCounters.Count
			If size <> writables.Count Then
				Throw New System.InvalidOperationException("Writables list and number of counters does not match (" & writables.Count & " vs " & size & ")")
			End If
			For i As Integer = 0 To size - 1
				analysisCounters(i).add(writables(i))
			Next i

			Return analysisCounters
		End Function
	End Class

End Namespace