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
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.LongQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.longq

    ''' <summary>
    ''' Running quality report of a long column: folds values in with <see cref="LongQualityAddFunction"/> and merges
    ''' states with <see cref="LongQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class LongQualityAnalysisState
        Implements QualityAnalysisState(Of LongQualityAnalysisState)

        ' Java: @Getter
        Private longQuality As LongQuality
        Private addFunction As LongQualityAddFunction
        Private mergeFunction As LongQualityMergeFunction

        Public Sub New(ByVal longMetaData As LongMetaData)
            Me.longQuality = New LongQuality()
            Me.addFunction = New LongQualityAddFunction(longMetaData)
            Me.mergeFunction = New LongQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As LongQualityAnalysisState Implements QualityAnalysisState(Of LongQualityAnalysisState).add
            longQuality = addFunction.apply(longQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As LongQualityAnalysisState) As LongQualityAnalysisState Implements QualityAnalysisState(Of LongQualityAnalysisState).merge
            longQuality = mergeFunction.apply(longQuality, other.getLongQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, LongQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return longQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getLongQuality() As LongQuality
            Return longQuality
        End Function
    End Class

End Namespace
