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
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports StringQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.StringQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.[string]

    ''' <summary>
    ''' Running quality report of a string column: folds values in with <see cref="StringQualityAddFunction"/> and merges
    ''' states with <see cref="StringQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class StringQualityAnalysisState
        Implements QualityAnalysisState(Of StringQualityAnalysisState)

        ' Java: @Getter
        Private stringQuality As StringQuality
        Private addFunction As StringQualityAddFunction
        Private mergeFunction As StringQualityMergeFunction

        Public Sub New(ByVal stringMetaData As StringMetaData)
            Me.stringQuality = New StringQuality()
            Me.addFunction = New StringQualityAddFunction(stringMetaData)
            Me.mergeFunction = New StringQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As StringQualityAnalysisState Implements QualityAnalysisState(Of StringQualityAnalysisState).add
            stringQuality = addFunction.apply(stringQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As StringQualityAnalysisState) As StringQualityAnalysisState Implements QualityAnalysisState(Of StringQualityAnalysisState).merge
            stringQuality = mergeFunction.apply(stringQuality, other.getStringQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, StringQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return stringQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getStringQuality() As StringQuality
            Return stringQuality
        End Function
    End Class

End Namespace
