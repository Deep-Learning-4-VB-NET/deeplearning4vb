Imports System.Collections.Generic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.analysis.columns
Imports org.datavec.api.transform.analysis.counter
Imports HistogramCounter = org.datavec.api.transform.analysis.histogram.HistogramCounter

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

Namespace org.datavec.api.transform.analysis


	Public Class DataVecAnalysisUtils

		Private Sub New()
		End Sub


		Public Shared Sub mergeCounters(ByVal columnAnalysis As IList(Of ColumnAnalysis), ByVal histogramCounters As IList(Of HistogramCounter))
			If histogramCounters Is Nothing Then
				Return
			End If

			'Merge analysis values and histogram values
			For i As Integer = 0 To columnAnalysis.Count - 1
				Dim hc As HistogramCounter = histogramCounters(i)
				Dim ca As ColumnAnalysis = columnAnalysis(i)
				If TypeOf ca Is IntegerAnalysis Then
					DirectCast(ca, IntegerAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, IntegerAnalysis).setHistogramBucketCounts(hc.Counts)
				ElseIf TypeOf ca Is DoubleAnalysis Then
					DirectCast(ca, DoubleAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, DoubleAnalysis).setHistogramBucketCounts(hc.Counts)
				ElseIf TypeOf ca Is LongAnalysis Then
					DirectCast(ca, LongAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, LongAnalysis).setHistogramBucketCounts(hc.Counts)
				ElseIf TypeOf ca Is TimeAnalysis Then
					DirectCast(ca, TimeAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, TimeAnalysis).setHistogramBucketCounts(hc.Counts)
				ElseIf TypeOf ca Is StringAnalysis Then
					DirectCast(ca, StringAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, StringAnalysis).setHistogramBucketCounts(hc.Counts)
				ElseIf TypeOf ca Is NDArrayAnalysis Then
					DirectCast(ca, NDArrayAnalysis).setHistogramBuckets(hc.Bins)
					DirectCast(ca, NDArrayAnalysis).setHistogramBucketCounts(hc.Counts)
				End If
			Next i
		End Sub


		Public Shared Function convertCounters(ByVal counters As IList(Of AnalysisCounter), ByVal minsMaxes()() As Double, ByVal columnTypes As IList(Of ColumnType)) As IList(Of ColumnAnalysis)
			Dim nColumns As Integer = columnTypes.Count

			Dim list As IList(Of ColumnAnalysis) = New List(Of ColumnAnalysis)()

			For i As Integer = 0 To nColumns - 1
				Dim ct As ColumnType = columnTypes(i)

				Select Case ct.innerEnumValue
					Case ColumnType.InnerEnum.String
						Dim sac As StringAnalysisCounter = CType(counters(i), StringAnalysisCounter)
						list.Add((New StringAnalysis.Builder()).countTotal(sac.CountTotal).minLength(sac.MinLengthSeen).maxLength(sac.MaxLengthSeen).meanLength(sac.Mean).sampleStdevLength(sac.SampleStdev).sampleVarianceLength(sac.SampleVariance).build())
						minsMaxes(i)(0) = sac.MinLengthSeen
						minsMaxes(i)(1) = sac.MaxLengthSeen
					Case Integer?
						Dim iac As IntegerAnalysisCounter = CType(counters(i), IntegerAnalysisCounter)
						Dim ia As IntegerAnalysis = (New IntegerAnalysis.Builder()).min(iac.MinValueSeen).max(iac.MaxValueSeen).mean(iac.Mean).sampleStdev(iac.SampleStdev).sampleVariance(iac.SampleVariance).countZero(iac.getCountZero()).countNegative(iac.getCountNegative()).countPositive(iac.getCountPositive()).countMinValue(iac.getCountMinValue()).countMaxValue(iac.getCountMaxValue()).countTotal(iac.CountTotal).digest(iac.getDigest()).build()
						list.Add(ia)

						minsMaxes(i)(0) = iac.MinValueSeen
						minsMaxes(i)(1) = iac.MaxValueSeen

					Case Long?
						Dim lac As LongAnalysisCounter = CType(counters(i), LongAnalysisCounter)

						Dim la As LongAnalysis = (New LongAnalysis.Builder()).min(lac.MinValueSeen).max(lac.MaxValueSeen).mean(lac.Mean).sampleStdev(lac.SampleStdev).sampleVariance(lac.SampleVariance).countZero(lac.getCountZero()).countNegative(lac.getCountNegative()).countPositive(lac.getCountPositive()).countMinValue(lac.getCountMinValue()).countMaxValue(lac.getCountMaxValue()).countTotal(lac.CountTotal).digest(lac.getDigest()).build()

						list.Add(la)

						minsMaxes(i)(0) = lac.MinValueSeen
						minsMaxes(i)(1) = lac.MaxValueSeen

					Case Single?, System.Nullable<Double>
						Dim dac As DoubleAnalysisCounter = CType(counters(i), DoubleAnalysisCounter)
						Dim da As DoubleAnalysis = (New DoubleAnalysis.Builder()).min(dac.MinValueSeen).max(dac.MaxValueSeen).mean(dac.Mean).sampleStdev(dac.SampleStdev).sampleVariance(dac.SampleVariance).countZero(dac.getCountZero()).countNegative(dac.getCountNegative()).countPositive(dac.getCountPositive()).countMinValue(dac.getCountMinValue()).countMaxValue(dac.getCountMaxValue()).countNaN(dac.getCountNaN()).digest(dac.getDigest()).countTotal(dac.CountTotal).build()
						list.Add(da)

						minsMaxes(i)(0) = dac.MinValueSeen
						minsMaxes(i)(1) = dac.MaxValueSeen

					Case ColumnType.InnerEnum.Categorical
						Dim cac As CategoricalAnalysisCounter = CType(counters(i), CategoricalAnalysisCounter)
						Dim ca As New CategoricalAnalysis(cac.getCounts())
						list.Add(ca)

					Case ColumnType.InnerEnum.Time
						Dim lac2 As LongAnalysisCounter = CType(counters(i), LongAnalysisCounter)

						Dim la2 As TimeAnalysis = (New TimeAnalysis.Builder()).min(lac2.MinValueSeen).max(lac2.MaxValueSeen).mean(lac2.Mean).sampleStdev(lac2.SampleStdev).sampleVariance(lac2.SampleVariance).countZero(lac2.getCountZero()).countNegative(lac2.getCountNegative()).countPositive(lac2.getCountPositive()).countMinValue(lac2.getCountMinValue()).countMaxValue(lac2.getCountMaxValue()).countTotal(lac2.CountTotal).digest(lac2.getDigest()).build()

						list.Add(la2)

						minsMaxes(i)(0) = lac2.MinValueSeen
						minsMaxes(i)(1) = lac2.MaxValueSeen

					Case ColumnType.InnerEnum.Bytes
						Dim bac As BytesAnalysisCounter = CType(counters(i), BytesAnalysisCounter)
						list.Add((New BytesAnalysis.Builder()).countTotal(bac.getCountTotal()).build())
					Case ColumnType.InnerEnum.NDArray
						Dim nac As NDArrayAnalysisCounter = CType(counters(i), NDArrayAnalysisCounter)
						Dim nda As NDArrayAnalysis = nac.toAnalysisObject()
						list.Add(nda)

						minsMaxes(i)(0) = nda.getMinValue()
						minsMaxes(i)(1) = nda.getMaxValue()

					Case Boolean?
						Dim iac2 As IntegerAnalysisCounter = CType(counters(i), IntegerAnalysisCounter)
						Dim ia2 As IntegerAnalysis = (New IntegerAnalysis.Builder()).min(iac2.MinValueSeen).max(iac2.MaxValueSeen).mean(iac2.Mean).sampleStdev(iac2.SampleStdev).sampleVariance(iac2.SampleVariance).countZero(iac2.getCountZero()).countNegative(iac2.getCountNegative()).countPositive(iac2.getCountPositive()).countMinValue(iac2.getCountMinValue()).countMaxValue(iac2.getCountMaxValue()).countTotal(iac2.CountTotal).digest(iac2.getDigest()).build()
						list.Add(ia2)

						minsMaxes(i)(0) = iac2.MinValueSeen
						minsMaxes(i)(1) = iac2.MaxValueSeen

					Case Else
						Throw New System.InvalidOperationException("Unknown column type: " & ct)
				End Select
			Next i

			Return list
		End Function

	End Class

End Namespace