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

Imports System.Collections.Generic
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports DeepLearning4VB.org.nd4vb.common.function
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.reduce

    ''' <summary>
    ''' Reduces a group of records into one record with the reducer's aggregable operation.
    ''' <para>
    ''' The Java version fed each record to a new aggregation (it called <c>reducer.aggregableReducer()</c> inside the
    ''' loop) and then read the result of yet another, empty one; this port feeds all the records to one aggregation.
    ''' </para>
    ''' </summary>
    Public Class ReducerFunction
        Implements [Function](Of IEnumerable(Of IList(Of WritableT)), IList(Of WritableT))

        Private ReadOnly reducer As IAssociativeReducerT

        Public Sub New(ByVal reducer As IAssociativeReducerT)
            Me.reducer = reducer
        End Sub

        Public Overridable Function apply(ByVal lists As IEnumerable(Of IList(Of WritableT))) As IList(Of WritableT) Implements [Function](Of IEnumerable(Of IList(Of WritableT)), IList(Of WritableT)).apply
            Dim op As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            For Each c As IList(Of WritableT) In lists
                op.accept(c)
            Next c
            Return op.get()
        End Function

    End Class

End Namespace
