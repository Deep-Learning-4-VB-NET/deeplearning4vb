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
    ''' A tree of the call stacks the profiler has seen, with a counter per node (Java <c>StackTree</c>).
    ''' </summary>
    Public Class StackTree

        Protected Friend basement As IDictionary(Of String, StackNode) = New Dictionary(Of String, StackNode)()
        Protected Friend eventsCount As New ComparableAtomicLong(0)
        Protected Friend branches As IDictionary(Of StackDescriptor, ComparableAtomicLong) = New Dictionary(Of StackDescriptor, ComparableAtomicLong)()
        Protected Friend lastDescriptor As StackDescriptor

        Public Sub New()
        End Sub

        ''' <summary>The descriptor consumed last (Java <c>getLastDescriptor()</c>).</summary>
        Public Overridable Function getLastDescriptor() As StackDescriptor
            Return lastDescriptor
        End Function

        ''' <summary>
        ''' Writes the tree to the console. Like Java, the returned string is empty (the nodes print themselves).
        ''' </summary>
        Public Overridable Function renderTree(ByVal displayCounts As Boolean) As String
            Dim builder As New StringBuilder()

            ' we'll always have single entry here, but let's keep loop here
            For Each cNode As StackNode In basement.Values
                cNode.traverse(0, displayCounts)
            Next cNode

            Return builder.ToString()
        End Function

        Public Overridable Sub consumeStackTrace(ByVal descriptor As StackDescriptor)
            consumeStackTrace(descriptor, 1)
        End Sub

        Public Overridable Sub consumeStackTrace(ByVal descriptor As StackDescriptor, ByVal increment As Long)
            If descriptor Is Nothing Then Throw New ArgumentNullException(NameOf(descriptor))
            eventsCount.incrementAndGet()

            lastDescriptor = descriptor

            If Not branches.ContainsKey(descriptor) Then
                branches(descriptor) = New ComparableAtomicLong(0)
            End If

            branches(descriptor).incrementAndGet()

            Dim entry As String = descriptor.EntryName
            If Not basement.ContainsKey(entry) Then
                basement(entry) = New StackNode(entry)
            End If

            ' propagate stack trace across tree
            basement(entry).consume(descriptor, 0, increment)
        End Sub

        Public Overridable ReadOnly Property TotalEventsNumber As Long
            Get
                Return eventsCount.get()
            End Get
        End Property

        Public Overridable ReadOnly Property UniqueBranchesNumber As Integer
            Get
                Return branches.Count
            End Get
        End Property

        Public Overridable Sub reset()
            basement.Clear()
            eventsCount.set(0)
            branches.Clear()
        End Sub

    End Class

End Namespace
