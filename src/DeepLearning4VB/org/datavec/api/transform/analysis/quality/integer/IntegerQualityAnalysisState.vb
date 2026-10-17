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
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntegerQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.IntegerQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.[integer]

    ''' <summary>
    ''' Running quality report of an integer column: folds values in with <see cref="IntegerQualityAddFunction"/> and merges
    ''' states with <see cref="IntegerQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class IntegerQualityAnalysisState
        Implements QualityAnalysisState(Of IntegerQualityAnalysisState)

        ' Java: @Getter
        Private integerQuality As IntegerQuality
        Private addFunction As IntegerQualityAddFunction
        Private mergeFunction As IntegerQualityMergeFunction

        Public Sub New(ByVal integerMetaData As IntegerMetaData)
            Me.integerQuality = New IntegerQuality()
            Me.addFunction = New IntegerQualityAddFunction(integerMetaData)
            Me.mergeFunction = New IntegerQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As IntegerQualityAnalysisState Implements QualityAnalysisState(Of IntegerQualityAnalysisState).add
            integerQuality = addFunction.apply(integerQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As IntegerQualityAnalysisState) As IntegerQualityAnalysisState Implements QualityAnalysisState(Of IntegerQualityAnalysisState).merge
            integerQuality = mergeFunction.apply(integerQuality, other.getIntegerQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, IntegerQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return integerQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getIntegerQuality() As IntegerQuality
            Return integerQuality
        End Function
    End Class

End Namespace
