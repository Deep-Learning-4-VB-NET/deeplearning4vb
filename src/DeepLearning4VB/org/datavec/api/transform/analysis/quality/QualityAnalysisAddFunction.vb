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
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.nd4vb.common.function
Imports BytesQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.bytes.BytesQualityAnalysisState
Imports CategoricalQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.categorical.CategoricalQualityAnalysisState
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports IntegerQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.[integer].IntegerQualityAnalysisState
Imports LongQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.longq.LongQualityAnalysisState
Imports RealQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.real.RealQualityAnalysisState
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.[string].StringQualityAnalysisState
Imports TimeQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.time.TimeQualityAnalysisState
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality

    ''' <summary>
    ''' Folds one record into the per-column quality states (creating them from the schema on the first call, when the
    ''' state list is <c>Nothing</c>). As in Java, float, boolean and NDArray columns are rejected with an
    ''' <see cref="ArgumentException"/>.
    ''' </summary>
    ' Java: @AllArgsConstructor
    <Serializable>
    Public Class QualityAnalysisAddFunction
        Implements BiFunction(Of IList(Of QualityAnalysisState), IList(Of WritableT), IList(Of QualityAnalysisState))

        Private schema As SchemaT

        Public Sub New(ByVal schema As SchemaT)
            Me.schema = schema
        End Sub

        Public Overridable Function apply(ByVal analysisStates As IList(Of QualityAnalysisState), ByVal writables As IList(Of WritableT)) As IList(Of QualityAnalysisState) Implements BiFunction(Of IList(Of QualityAnalysisState), IList(Of WritableT), IList(Of QualityAnalysisState)).apply
            If analysisStates Is Nothing Then
                analysisStates = New List(Of QualityAnalysisState)()
                Dim columnTypes As IList(Of ColumnTypeEnum) = schema.getColumnTypes()
                Dim columnMetaDatas As IList(Of ColumnMetaData) = schema.getColumnMetaData()
                For i As Integer = 0 To columnTypes.Count - 1
                    Select Case columnTypes(i)
                        Case ColumnTypeEnum.String
                            analysisStates.Add(New StringQualityAnalysisState(DirectCast(columnMetaDatas(i), StringMetaData)))
                        Case ColumnTypeEnum.Integer
                            analysisStates.Add(New IntegerQualityAnalysisState(DirectCast(columnMetaDatas(i), IntegerMetaData)))
                        Case ColumnTypeEnum.Long
                            analysisStates.Add(New LongQualityAnalysisState(DirectCast(columnMetaDatas(i), LongMetaData)))
                        Case ColumnTypeEnum.Double
                            analysisStates.Add(New RealQualityAnalysisState(DirectCast(columnMetaDatas(i), DoubleMetaData)))
                        Case ColumnTypeEnum.Categorical
                            analysisStates.Add(New CategoricalQualityAnalysisState(DirectCast(columnMetaDatas(i), CategoricalMetaData)))
                        Case ColumnTypeEnum.Time
                            analysisStates.Add(New TimeQualityAnalysisState(DirectCast(columnMetaDatas(i), TimeMetaData)))
                        Case ColumnTypeEnum.Bytes
                            analysisStates.Add(New BytesQualityAnalysisState()) 'TODO
                        Case Else
                            Throw New ArgumentException("Unknown column type: " & columnTypes(i).ToString())
                    End Select
                Next i
            End If

            Dim size As Integer = analysisStates.Count
            If size <> writables.Count Then
                Throw New InvalidOperationException("Writables list and number of states does not match (" & writables.Count & " vs " & size & ")")
            End If
            For i As Integer = 0 To size - 1
                analysisStates(i).add(writables(i))
            Next i

            Return analysisStates
        End Function
    End Class

End Namespace
