Imports System
Imports System.Collections.Generic
Imports System.Text

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

Namespace org.nd4vb.linalg.profiler.data.primitives

    ''' <summary>
    ''' One node of a <see cref="StackTree"/>: a stack element and the elements called from it (Java <c>StackNode</c>).
    ''' Nodes sort in descending order of their counter.
    ''' </summary>
    Public Class StackNode
        Implements IComparable(Of StackNode)

        Private ReadOnly nodeURI As String
        Protected Friend entries As IDictionary(Of String, StackNode) = New Dictionary(Of String, StackNode)()
        Protected Friend counter As New ComparableAtomicLong(0)

        Public Sub New(ByVal uri As String)
            If uri Is Nothing Then Throw New ArgumentNullException(NameOf(uri))
            Me.nodeURI = uri
        End Sub

        Public Overridable ReadOnly Property Nodes As ICollection(Of StackNode)
            Get
                Return entries.Values
            End Get
        End Property

        ''' <summary>Java <c>getNodes()</c>.</summary>
        Public Overridable Function getNodes() As ICollection(Of StackNode)
            Return Nodes
        End Function

        ''' <summary>Writes this node and its children to the console, indented by level.</summary>
        Public Overridable Sub traverse(ByVal ownLevel As Integer, ByVal displayCounts As Boolean)
            Dim builder As New StringBuilder()

            For x As Integer = 0 To ownLevel - 1
                builder.Append("   ")
            Next x

            builder.Append("").Append(nodeURI)

            If displayCounts Then
                builder.Append("  ").Append(counter.get()).Append(" us")
            End If

            Console.WriteLine(builder.ToString())

            For Each node As StackNode In entries.Values
                node.traverse(ownLevel + 1, displayCounts)
            Next node
        End Sub

        Public Overridable Sub consume(ByVal descriptor As StackDescriptor, ByVal lastLevel As Integer)
            consume(descriptor, lastLevel, 1)
        End Sub

        Public Overridable Sub consume(ByVal descriptor As StackDescriptor, ByVal lastLevel As Integer, ByVal delta As Long)
            If descriptor Is Nothing Then Throw New ArgumentNullException(NameOf(descriptor))
            Dim gotEntry As Boolean = False
            For e As Integer = 0 To descriptor.size() - 1
                Dim entryName As String = descriptor.getElementName(e)

                ' we look for current entry first
                If Not gotEntry Then
                    If entryName.Equals(nodeURI, StringComparison.OrdinalIgnoreCase) AndAlso e >= lastLevel Then
                        gotEntry = True
                        counter.addAndGet(delta)
                    End If
                Else
                    ' after current entry is found, we just fill first node after it
                    If Not entries.ContainsKey(entryName) Then
                        entries(entryName) = New StackNode(entryName)
                    End If

                    entries(entryName).consume(descriptor, e)
                    Exit For
                End If
            Next e
        End Sub

        Public Overridable Function CompareTo(ByVal o As StackNode) As Integer Implements IComparable(Of StackNode).CompareTo
            Return o.counter.get().CompareTo(Me.counter.get())
        End Function

    End Class

End Namespace
