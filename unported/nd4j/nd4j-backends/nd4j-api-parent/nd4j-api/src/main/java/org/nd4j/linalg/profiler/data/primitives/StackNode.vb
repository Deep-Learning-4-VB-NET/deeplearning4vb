Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.nd4j.linalg.profiler.data.primitives


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StackNode implements Comparable<StackNode>
	Public Class StackNode
		Implements IComparable(Of StackNode)

		Private ReadOnly nodeURI As String
		Protected Friend entries As IDictionary(Of String, StackNode) = New Dictionary(Of String, StackNode)()
		Protected Friend counter As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StackNode(@NonNull String uri)
		Public Sub New(ByVal uri As String)
			Me.nodeURI = uri
		End Sub

		Public Overridable ReadOnly Property Nodes As ICollection(Of StackNode)
			Get
				Return entries.Values
			End Get
		End Property

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void consume(@NonNull StackDescriptor descriptor, int lastLevel)
		Public Overridable Sub consume(ByVal descriptor As StackDescriptor, ByVal lastLevel As Integer)
			consume(descriptor, lastLevel, 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void consume(@NonNull StackDescriptor descriptor, int lastLevel, long delta)
		Public Overridable Sub consume(ByVal descriptor As StackDescriptor, ByVal lastLevel As Integer, ByVal delta As Long)
			Dim gotEntry As Boolean = False
			Dim e As Integer = 0
			Do While e < descriptor.size()
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
					Exit Do
				End If
				e += 1
			Loop
		End Sub

		Public Overridable Function CompareTo(ByVal o As StackNode) As Integer Implements IComparable(Of StackNode).CompareTo
			Return Long.compare(o.counter.get(), Me.counter.get())
		End Function
	End Class

End Namespace