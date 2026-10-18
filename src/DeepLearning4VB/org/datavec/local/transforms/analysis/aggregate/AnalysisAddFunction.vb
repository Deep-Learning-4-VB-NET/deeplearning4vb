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
Imports DeepLearning4VB.org.datavec.api.transform.analysis.counter
Imports DeepLearning4VB.org.nd4vb.common.function
Imports AnalysisCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.AnalysisCounter
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.analysis.aggregate

    ''' <summary>
    ''' Adds one record to the per-column analysis counters, creating the counters (one per column, of the class
    ''' matching the column type) when the list is <c>Nothing</c>. As in Java, float and boolean columns are rejected
    ''' with an <see cref="ArgumentException"/>; NDArray columns throw <see cref="NotSupportedException"/> (their counter
    ''' needs the tensor API).
    ''' </summary>
    Public Class AnalysisAddFunction
        Implements BiFunction(Of IList(Of AnalysisCounterT), IList(Of WritableT), IList(Of AnalysisCounterT))

        Private schema As SchemaT

        Public Sub New(ByVal schema As SchemaT)
            Me.schema = schema
        End Sub

        Public Overridable Function apply(ByVal analysisCounters As IList(Of AnalysisCounterT), ByVal writables As IList(Of WritableT)) As IList(Of AnalysisCounterT) Implements BiFunction(Of IList(Of AnalysisCounterT), IList(Of WritableT), IList(Of AnalysisCounterT)).apply
            If analysisCounters Is Nothing Then
                analysisCounters = New List(Of AnalysisCounterT)()
                Dim columnTypes As IList(Of ColumnTypeT) = schema.getColumnTypes()
                For Each ct As ColumnTypeT In columnTypes
                    Select Case ct
                        Case ColumnTypeT.String
                            analysisCounters.Add(New StringAnalysisCounter())
                        Case ColumnTypeT.Integer
                            analysisCounters.Add(New IntegerAnalysisCounter())
                        Case ColumnTypeT.Long
                            analysisCounters.Add(New LongAnalysisCounter())
                        Case ColumnTypeT.Double
                            analysisCounters.Add(New DoubleAnalysisCounter())
                        Case ColumnTypeT.Categorical
                            analysisCounters.Add(New CategoricalAnalysisCounter())
                        Case ColumnTypeT.Time
                            analysisCounters.Add(New LongAnalysisCounter())
                        Case ColumnTypeT.Bytes
                            analysisCounters.Add(New BytesAnalysisCounter())
                        Case ColumnTypeT.NDArray
                            ' Java: new NDArrayAnalysisCounter()
                            Throw New NotSupportedException("NDArray column analysis is not supported yet: NDArrayAnalysisCounter needs the INDArray tensor API")
                        Case Else
                            Throw New ArgumentException("Unknown column type: " & ct.ToString())
                    End Select
                Next ct
            End If

            Dim size As Integer = analysisCounters.Count
            If size <> writables.Count Then
                Throw New InvalidOperationException("Writables list and number of counters does not match (" & writables.Count & " vs " & size & ")")
            End If
            For i As Integer = 0 To size - 1
                analysisCounters(i).add(writables(i))
            Next i

            Return analysisCounters
        End Function

    End Class

End Namespace
