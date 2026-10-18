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
Imports System.Text.Json.Serialization
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' The objects lists are written in by <see cref="BaseSerializer"/>: <c>{"list" : [ ... ]}</c>, so that the element
    ''' type (and with it the <c>"@class"</c> of each element) is known when the list is read back.
    ''' </summary>
    Public NotInheritable Class ListWrappers

        Private Sub New()
        End Sub

        ''' <summary>A list of transforms.</summary>
        Public Class TransformList
            Friend list As IList(Of TransformT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of TransformT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of TransformT)
                Return list
            End Function
        End Class

        ''' <summary>A list of filters.</summary>
        Public Class FilterList
            Friend list As IList(Of FilterT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of FilterT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of FilterT)
                Return list
            End Function
        End Class

        ''' <summary>A list of conditions.</summary>
        Public Class ConditionList
            Friend list As IList(Of ConditionT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of ConditionT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of ConditionT)
                Return list
            End Function
        End Class

        ''' <summary>A list of reducers.</summary>
        Public Class ReducerList
            Friend list As IList(Of IAssociativeReducerT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of IAssociativeReducerT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of IAssociativeReducerT)
                Return list
            End Function
        End Class

        ''' <summary>A list of sequence comparators.</summary>
        Public Class SequenceComparatorList
            Friend list As IList(Of SequenceComparatorT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of SequenceComparatorT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of SequenceComparatorT)
                Return list
            End Function
        End Class

        ''' <summary>A list of data actions.</summary>
        Public Class DataActionList
            Friend list As IList(Of DataActionT)

            <JsonConstructor>
            Public Sub New(ByVal list As IList(Of DataActionT))
                Me.list = list
            End Sub

            ''' <summary>Java <c>getList()</c> (Lombok).</summary>
            Public Function getList() As IList(Of DataActionT)
                Return list
            End Function
        End Class

    End Class

End Namespace
