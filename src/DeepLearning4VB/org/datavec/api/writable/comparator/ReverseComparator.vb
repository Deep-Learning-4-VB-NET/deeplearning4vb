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

Namespace org.datavec.api.writable.comparator

    ''' <summary>Reverses the order of another comparer.</summary>
    <Serializable>
    Public Class ReverseComparator(Of T)
        Implements IComparer(Of T)

        Private ReadOnly comparator As IComparer(Of T)

        Public Sub New(ByVal comparator As IComparer(Of T))
            Me.comparator = comparator
        End Sub

        Public Function getComparator() As IComparer(Of T)
            Return comparator
        End Function

        Public Overridable Function Compare(ByVal o1 As T, ByVal o2 As T) As Integer Implements IComparer(Of T).Compare
            Return -comparator.Compare(o1, o2)
        End Function

        Public Overrides Function ToString() As String
            Return "ReverseComparator(" & If(comparator Is Nothing, "null", comparator.ToString()) & ")"
        End Function
    End Class

End Namespace
