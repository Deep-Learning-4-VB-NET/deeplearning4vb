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
Imports BytesQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.BytesQuality
Imports ColumnQualityT = DeepLearning4VB.org.datavec.api.transform.quality.columns.ColumnQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.bytes

    ''' <summary>Quality state of a bytes column. As in Java, nothing is analysed: the report always has zero counts.</summary>
    <Serializable>
    Public Class BytesQualityAnalysisState
        Implements QualityAnalysisState(Of BytesQualityAnalysisState)

        ' Java: @Getter
        Private bytesQuality As BytesQuality

        Public Sub New()
            Me.bytesQuality = New BytesQuality()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As BytesQualityAnalysisState Implements QualityAnalysisState(Of BytesQualityAnalysisState).add
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As BytesQualityAnalysisState) As BytesQualityAnalysisState Implements QualityAnalysisState(Of BytesQualityAnalysisState).merge
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, BytesQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return bytesQuality
            End Get
        End Property

        ''' <summary>The (empty) quality report (Lombok <c>@Getter</c>).</summary>
        Public Function getBytesQuality() As BytesQuality
            Return bytesQuality
        End Function
    End Class

End Namespace
