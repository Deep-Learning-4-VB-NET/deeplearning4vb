Imports System
Imports System.Collections
Imports System.Collections.Generic

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

Namespace org.datavec.api.util.files

    ''' <summary>
    ''' Iterates over a list in the order given by an index array (or in list order when the
    ''' order array is Nothing). Java's <c>Iterator&lt;T&gt;</c> is <see cref="IEnumerator(Of T)"/> here.
    ''' </summary>
    Public Class ShuffledListIterator(Of T)
        Implements IEnumerator(Of T)

        Private ReadOnly list As IList(Of T)
        Private ReadOnly order() As Integer
        Private currentPosition As Integer = 0
        Private currentItem As T

        Public Sub New(ByVal list As IList(Of T), ByVal order() As Integer)
            If order IsNot Nothing AndAlso list.Count <> order.Length Then
                Throw New ArgumentException("Order array and list sizes differ")
            End If
            Me.list = list
            Me.order = order
        End Sub

        Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
            Get
                Return currentItem
            End Get
        End Property

        Private ReadOnly Property CurrentObject As Object Implements IEnumerator.Current
            Get
                Return currentItem
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If currentPosition >= list.Count Then
                currentItem = Nothing
                Return False
            End If
            Dim nextPos As Integer = If(order IsNot Nothing, order(currentPosition), currentPosition)
            currentPosition += 1
            currentItem = list(nextPos)
            Return True
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            currentPosition = 0
            currentItem = Nothing
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class

End Namespace
