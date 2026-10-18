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
Imports JsonMappers = DeepLearning4VB.org.datavec.api.transform.serde.JsonMappers
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis

Namespace org.datavec.api.transform.analysis

    ''' <summary>The analysis of a sequence data set: the per-column analysis plus the sequence length analysis.</summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class SequenceDataAnalysis
        Inherits DataAnalysis

        Private sequenceLengthAnalysis As SequenceLengthAnalysisT

        Public Sub New(ByVal schema As SchemaT, ByVal columnAnalysis As IList(Of ColumnAnalysisT), ByVal sequenceAnalysis As SequenceLengthAnalysisT)
            MyBase.New(schema, columnAnalysis)
            Me.sequenceLengthAnalysis = sequenceAnalysis
        End Sub

        Protected Friend Sub New()
            'No arg for JSON
        End Sub

        ''' <summary>
        ''' Deserialize a JSON SequenceDataAnalysis written by <see cref="DataAnalysis.toJson"/> (or by Java DataVec).
        ''' YAML is not supported: the Java <c>fromYaml</c> method is not ported.
        ''' </summary>
        ''' <exception cref="System.Text.Json.JsonException">if the JSON is not a valid sequence data analysis</exception>
        Public Shared Shadows Function fromJson(ByVal json As String) As SequenceDataAnalysis
            Return JsonMappers.readValue(Of SequenceDataAnalysis)(json)
        End Function

        Public Overrides Function ToString() As String
            Return Objects.toString(sequenceLengthAnalysis) & ControlChars.Lf & MyBase.ToString()
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getSequenceLengthAnalysis() As SequenceLengthAnalysisT
            Return sequenceLengthAnalysis
        End Function

        Public Sub setSequenceLengthAnalysis(ByVal sequenceLengthAnalysis As SequenceLengthAnalysisT)
            Me.sequenceLengthAnalysis = sequenceLengthAnalysis
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Return Object.Equals(sequenceLengthAnalysis, DirectCast(obj, SequenceDataAnalysis).sequenceLengthAnalysis)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), sequenceLengthAnalysis)
        End Function
    End Class

End Namespace
