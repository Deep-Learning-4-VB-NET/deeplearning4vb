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
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports CategoricalQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.CategoricalQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.categorical

    ''' <summary>
    ''' Running quality report of a categorical column: folds values in with <see cref="CategoricalQualityAddFunction"/> and merges
    ''' states with <see cref="CategoricalQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class CategoricalQualityAnalysisState
        Implements QualityAnalysisState(Of CategoricalQualityAnalysisState)

        ' Java: @Getter
        Private categoricalQuality As CategoricalQuality
        Private addFunction As CategoricalQualityAddFunction
        Private mergeFunction As CategoricalQualityMergeFunction

        Public Sub New(ByVal integerMetaData As CategoricalMetaData)
            Me.categoricalQuality = New CategoricalQuality()
            Me.addFunction = New CategoricalQualityAddFunction(integerMetaData)
            Me.mergeFunction = New CategoricalQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As CategoricalQualityAnalysisState Implements QualityAnalysisState(Of CategoricalQualityAnalysisState).add
            categoricalQuality = addFunction.apply(categoricalQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As CategoricalQualityAnalysisState) As CategoricalQualityAnalysisState Implements QualityAnalysisState(Of CategoricalQualityAnalysisState).merge
            categoricalQuality = mergeFunction.apply(categoricalQuality, other.getCategoricalQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, CategoricalQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return categoricalQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getCategoricalQuality() As CategoricalQuality
            Return categoricalQuality
        End Function
    End Class

End Namespace
