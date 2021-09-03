Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.analysis.histogram
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
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

Namespace org.datavec.local.transforms.analysis.histogram


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class HistogramAddFunction implements org.nd4j.common.function.BiFunction<java.util.List<HistogramCounter>, java.util.List<org.datavec.api.writable.Writable>, java.util.List<HistogramCounter>>
	Public Class HistogramAddFunction
		Implements BiFunction(Of IList(Of HistogramCounter), IList(Of Writable), IList(Of HistogramCounter))

		Private ReadOnly nBins As Integer
		Private ReadOnly schema As Schema
		Private ReadOnly minsMaxes()() As Double

		Public Overridable Function apply(ByVal histogramCounters As IList(Of HistogramCounter), ByVal writables As IList(Of Writable)) As IList(Of HistogramCounter)
			If histogramCounters Is Nothing Then
				histogramCounters = New List(Of HistogramCounter)()
				Dim columnTypes As IList(Of ColumnType) = schema.getColumnTypes()
				Dim i As Integer = 0
				For Each ct As ColumnType In columnTypes
					Select Case ct.innerEnumValue
						Case ColumnType.InnerEnum.String
							histogramCounters.Add(New StringHistogramCounter(CInt(Math.Truncate(minsMaxes(i)(0))), CInt(Math.Truncate(minsMaxes(i)(1))), nBins))
						Case Integer?
							histogramCounters.Add(New DoubleHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
						Case Long?
							histogramCounters.Add(New DoubleHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
						Case Double?
							histogramCounters.Add(New DoubleHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
						Case ColumnType.InnerEnum.Categorical
							Dim meta As CategoricalMetaData = DirectCast(schema.getMetaData(i), CategoricalMetaData)
							histogramCounters.Add(New CategoricalHistogramCounter(meta.getStateNames()))
						Case ColumnType.InnerEnum.Time
							histogramCounters.Add(New DoubleHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
						Case ColumnType.InnerEnum.Bytes
							histogramCounters.Add(Nothing) 'TODO
						Case ColumnType.InnerEnum.NDArray
							histogramCounters.Add(New NDArrayHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
						Case Else
							Throw New System.ArgumentException("Unknown column type: " & ct)
					End Select

					i += 1
				Next ct
			End If

			Dim size As Integer = histogramCounters.Count
			If size <> writables.Count Then
				Throw New System.InvalidOperationException("Writables list and number of counters does not match (" & writables.Count & " vs " & size & ")")
			End If
			For i As Integer = 0 To size - 1
				Dim hc As HistogramCounter = histogramCounters(i)
				If hc IsNot Nothing Then
					hc.add(writables(i))
				End If
			Next i

			Return histogramCounters
		End Function
	End Class

End Namespace