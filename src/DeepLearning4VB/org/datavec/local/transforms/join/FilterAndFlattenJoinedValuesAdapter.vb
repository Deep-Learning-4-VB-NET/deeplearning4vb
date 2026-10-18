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
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.join

    ''' <summary>
    ''' Keeps a <see cref="JoinedValue"/> if the join type wants it (inner: both sides present; left outer: left present;
    ''' right outer: right present; full outer: always) and returns its values, or nothing.
    ''' </summary>
    <Serializable>
    Public Class FilterAndFlattenJoinedValuesAdapter
        Implements FlatMapFunctionAdapter(Of JoinedValue, IList(Of WritableT))

        Private ReadOnly joinType As JoinT.JoinType

        Public Sub New(ByVal joinType As JoinT.JoinType)
            Me.joinType = joinType
        End Sub

        Public Overridable Function [call](ByVal joinedValue As JoinedValue) As IList(Of IList(Of WritableT)) Implements FlatMapFunctionAdapter(Of JoinedValue, IList(Of WritableT)).call
            Dim keep As Boolean
            Select Case joinType
                Case JoinT.JoinType.Inner
                    'Only keep joined values where we have both left and right
                    keep = joinedValue.isHaveLeft() AndAlso joinedValue.isHaveRight()
                Case JoinT.JoinType.LeftOuter
                    'Keep all values where left is not missing/null
                    keep = joinedValue.isHaveLeft()
                Case JoinT.JoinType.RightOuter
                    'Keep all values where right is not missing/null
                    keep = joinedValue.isHaveRight()
                Case JoinT.JoinType.FullOuter
                    'Keep all values
                    keep = True
                Case Else
                    Throw New Exception("Unknown/not implemented join type: " & joinType.ToString())
            End Select

            If keep Then
                Return New List(Of IList(Of WritableT)) From {joinedValue.getValues()}
            Else
                Return New List(Of IList(Of WritableT))()
            End If
        End Function

    End Class

End Namespace
