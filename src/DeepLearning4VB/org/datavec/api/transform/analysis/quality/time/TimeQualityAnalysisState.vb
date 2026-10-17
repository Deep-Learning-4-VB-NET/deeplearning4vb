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
Imports ColumnQualityT = DeepLearning4VB.org.datavec.api.transform.quality.columns.ColumnQuality
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports TimeQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.TimeQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.time

    ''' <summary>
    ''' Running quality report of a time column: folds values in with <see cref="TimeQualityAddFunction"/> and merges
    ''' states with <see cref="TimeQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class TimeQualityAnalysisState
        Implements QualityAnalysisState(Of TimeQualityAnalysisState)

        ' Java: @Getter
        Private timeQuality As TimeQuality
        Private addFunction As TimeQualityAddFunction
        Private mergeFunction As TimeQualityMergeFunction

        Public Sub New(ByVal timeMetaData As TimeMetaData)
            Me.timeQuality = New TimeQuality()
            Me.addFunction = New TimeQualityAddFunction(timeMetaData)
            Me.mergeFunction = New TimeQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As TimeQualityAnalysisState Implements QualityAnalysisState(Of TimeQualityAnalysisState).add
            timeQuality = addFunction.apply(timeQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As TimeQualityAnalysisState) As TimeQualityAnalysisState Implements QualityAnalysisState(Of TimeQualityAnalysisState).merge
            timeQuality = mergeFunction.apply(timeQuality, other.getTimeQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, TimeQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return timeQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getTimeQuality() As TimeQuality
            Return timeQuality
        End Function
    End Class

End Namespace
