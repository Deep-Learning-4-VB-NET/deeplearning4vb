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

Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.datavec.api.transform.analysis.histogram
Imports DeepLearning4VB.org.nd4vb.common.function
Imports CategoricalMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.analysis.histogram

    ''' <summary>
    ''' Adds one record to the per-column histogram counters, creating the counters when the list is <c>Nothing</c>:
    ''' <c>nBins</c> buckets between the minimum and maximum of each column (from the first analysis pass,
    ''' see <c>DataVecAnalysisUtils.convertCounters</c>), one bucket per state for categorical columns and no
    ''' histogram (<c>Nothing</c>) for bytes columns. As in Java, float and boolean columns are rejected with an
    ''' <see cref="ArgumentException"/>; NDArray columns throw <see cref="NotSupportedException"/>.
    ''' </summary>
    Public Class HistogramAddFunction
        Implements BiFunction(Of IList(Of HistogramCounter), IList(Of WritableT), IList(Of HistogramCounter))

        Private ReadOnly nBins As Integer
        Private ReadOnly schema As SchemaT
        Private ReadOnly minsMaxes()() As Double

        Public Sub New(ByVal nBins As Integer, ByVal schema As SchemaT, ByVal minsMaxes()() As Double)
            Me.nBins = nBins
            Me.schema = schema
            Me.minsMaxes = minsMaxes
        End Sub

        Public Overridable Function apply(ByVal histogramCounters As IList(Of HistogramCounter), ByVal writables As IList(Of WritableT)) As IList(Of HistogramCounter) Implements BiFunction(Of IList(Of HistogramCounter), IList(Of WritableT), IList(Of HistogramCounter)).apply
            If histogramCounters Is Nothing Then
                histogramCounters = New List(Of HistogramCounter)()
                Dim columnTypes As IList(Of ColumnTypeT) = schema.getColumnTypes()
                Dim i As Integer = 0
                For Each ct As ColumnTypeT In columnTypes
                    Select Case ct
                        Case ColumnTypeT.String
                            ' Java: (int) minsMaxes[i][0], a truncating cast
                            histogramCounters.Add(New StringHistogramCounter(CInt(Global.System.Math.Truncate(minsMaxes(i)(0))), CInt(Global.System.Math.Truncate(minsMaxes(i)(1))), nBins))
                        Case ColumnTypeT.Integer, ColumnTypeT.Long, ColumnTypeT.Double, ColumnTypeT.Time
                            histogramCounters.Add(New DoubleHistogramCounter(minsMaxes(i)(0), minsMaxes(i)(1), nBins))
                        Case ColumnTypeT.Categorical
                            Dim meta As CategoricalMetaDataT = DirectCast(schema.getMetaData(i), CategoricalMetaDataT)
                            histogramCounters.Add(New CategoricalHistogramCounter(meta.getStateNames()))
                        Case ColumnTypeT.Bytes
                            histogramCounters.Add(Nothing) 'TODO
                        Case ColumnTypeT.NDArray
                            ' Java: new NDArrayHistogramCounter(minsMaxes[i][0], minsMaxes[i][1], nBins)
                            Throw New NotSupportedException("NDArray column histograms are not supported yet: NDArrayHistogramCounter needs the INDArray tensor API")
                        Case Else
                            Throw New ArgumentException("Unknown column type: " & ct.ToString())
                    End Select

                    i += 1
                Next ct
            End If

            Dim size As Integer = histogramCounters.Count
            If size <> writables.Count Then
                Throw New InvalidOperationException("Writables list and number of counters does not match (" & writables.Count & " vs " & size & ")")
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
