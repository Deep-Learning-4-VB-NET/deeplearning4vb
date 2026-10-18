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
Imports DeepLearning4VB.org.datavec.local.transforms.functions
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform

    ''' <summary>Splits one sequence into several with a <see cref="SequenceSplitT"/>.</summary>
    <Serializable>
    Public Class SequenceSplitFunctionAdapter
        Implements FlatMapFunctionAdapter(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))

        Private ReadOnly split As SequenceSplitT

        Public Sub New(ByVal split As SequenceSplitT)
            Me.split = split
        End Sub

        Public Overridable Function [call](ByVal collections As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT))) Implements FlatMapFunctionAdapter(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))).call
            Return split.split(collections)
        End Function

    End Class

End Namespace
