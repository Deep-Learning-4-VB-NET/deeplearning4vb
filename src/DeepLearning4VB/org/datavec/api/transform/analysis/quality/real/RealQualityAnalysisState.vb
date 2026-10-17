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
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.DoubleQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.real

    ''' <summary>
    ''' Running quality report of a double column: folds values in with <see cref="RealQualityAddFunction"/> and merges
    ''' states with <see cref="RealQualityMergeFunction"/>.
    ''' </summary>
    <Serializable>
    Public Class RealQualityAnalysisState
        Implements QualityAnalysisState(Of RealQualityAnalysisState)

        ' Java: @Getter
        Private realQuality As DoubleQuality
        Private addFunction As RealQualityAddFunction
        Private mergeFunction As RealQualityMergeFunction

        Public Sub New(ByVal realMetaData As DoubleMetaData)
            Me.realQuality = New DoubleQuality()
            Me.addFunction = New RealQualityAddFunction(realMetaData)
            Me.mergeFunction = New RealQualityMergeFunction()
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As RealQualityAnalysisState Implements QualityAnalysisState(Of RealQualityAnalysisState).add
            realQuality = addFunction.apply(realQuality, writable)
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As RealQualityAnalysisState) As RealQualityAnalysisState Implements QualityAnalysisState(Of RealQualityAnalysisState).merge
            realQuality = mergeFunction.apply(realQuality, other.getRealQuality())
            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As QualityAnalysisState Implements QualityAnalysisState.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As QualityAnalysisState) As QualityAnalysisState Implements QualityAnalysisState.merge
            Return merge(DirectCast(other, RealQualityAnalysisState))
        End Function

        Public Overridable ReadOnly Property ColumnQuality As ColumnQualityT Implements QualityAnalysisState.ColumnQuality
            Get
                Return realQuality
            End Get
        End Property

        ''' <summary>The quality report accumulated so far (Lombok <c>@Getter</c>).</summary>
        Public Function getRealQuality() As DoubleQuality
            Return realQuality
        End Function
    End Class

End Namespace
