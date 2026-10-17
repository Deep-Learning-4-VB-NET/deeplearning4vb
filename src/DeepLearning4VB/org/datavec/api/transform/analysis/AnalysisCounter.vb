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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis

    ''' <summary>
    ''' The untyped view of an analysis counter: Java's raw <c>AnalysisCounter</c> type, which the analysis
    ''' functions use for lists of counters of mixed types (<c>List&lt;AnalysisCounter&gt;</c>). Every
    ''' <see cref="AnalysisCounter(Of T)"/> implements it; <see cref="merge"/> expects a counter of the same class.
    ''' </summary>
    Public Interface AnalysisCounter

        ''' <summary>Adds one value to the counter and returns the counter.</summary>
        Function add(ByVal writable As WritableT) As AnalysisCounter

        ''' <summary>Merges a counter of the same class into this one and returns the merged counter.</summary>
        Function merge(ByVal other As AnalysisCounter) As AnalysisCounter

    End Interface

    ''' <summary>
    ''' Counts and summary statistics of one column, built up value by value (<see cref="add"/>) and combined
    ''' across partitions (<see cref="merge"/>).
    ''' </summary>
    Public Interface AnalysisCounter(Of T As AnalysisCounter)
        Inherits AnalysisCounter

        Shadows Function add(ByVal writable As WritableT) As T

        Overloads Function merge(ByVal other As T) As T

    End Interface

End Namespace
