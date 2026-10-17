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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality

    ''' <summary>
    ''' The untyped view of a quality analysis state: Java's raw <c>QualityAnalysisState</c> type, used for the
    ''' per-column lists of states of mixed types. Every <see cref="QualityAnalysisState(Of T)"/> implements it;
    ''' <see cref="merge"/> expects a state of the same class.
    ''' </summary>
    Public Interface QualityAnalysisState

        ''' <summary>Folds one value into the state and returns the state.</summary>
        Function add(ByVal writable As WritableT) As QualityAnalysisState

        ''' <summary>Merges a state of the same class into this one and returns this state.</summary>
        Function merge(ByVal other As QualityAnalysisState) As QualityAnalysisState

        ''' <summary>The quality report accumulated so far.</summary>
        ReadOnly Property ColumnQuality As ColumnQualityT

    End Interface

    ''' <summary>The running data-quality report of one column, built up value by value and merged across partitions.</summary>
    Public Interface QualityAnalysisState(Of T As QualityAnalysisState)
        Inherits QualityAnalysisState

        Shadows Function add(ByVal writable As WritableT) As T

        Overloads Function merge(ByVal other As T) As T

    End Interface

End Namespace
