Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter

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

Namespace org.deeplearning4j.models.sequencevectors.sequence


	<Serializable>
	Public Class Sequence(Of T As SequenceElement)

		Private Const serialVersionUID As Long = 2223750736522624735L

		Protected Friend elements As IList(Of T) = New List(Of T)()

		' elements map needed to speedup searches against elements in sequence
		Protected Friend elementsMap As IDictionary(Of String, T) = New LinkedHashMap(Of String, T)()

		' each document can have multiple labels
		Protected Friend labels As IList(Of T) = New List(Of T)()

		Protected Friend label As T

		Protected Friend hash As Integer = 0
		Protected Friend hashCached As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int sequenceId;
		Protected Friend sequenceId As Integer

		''' <summary>
		''' Creates new empty sequence
		''' 
		''' </summary>
		Public Sub New()

		End Sub

		''' <summary>
		''' Creates new sequence from collection of elements
		''' </summary>
		''' <param name="set"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Sequence(@NonNull Collection<T> set)
		Public Sub New(ByVal set As ICollection(Of T))
			Me.New()
			addElements(set)
		End Sub

		''' <summary>
		''' Adds single element to sequence
		''' </summary>
		''' <param name="element"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void addElement(@NonNull T element)
		Public Overridable Sub addElement(ByVal element As T)
			SyncLock Me
				hashCached = False
				Me.elementsMap(element.Label) = element
				Me.elements.Add(element)
			End SyncLock
		End Sub

		''' <summary>
		''' Adds collection of elements to the sequence
		''' </summary>
		''' <param name="set"> </param>
		Public Overridable Sub addElements(ByVal set As ICollection(Of T))
			For Each element As T In set
				addElement(element)
			Next element
		End Sub

		''' <summary>
		''' Returns this sequence as list of labels
		''' @return
		''' </summary>
		Public Overridable Function asLabels() As IList(Of String)
			Dim labels As IList(Of String) = New List(Of String)()
			For Each element As T In getElements()
				labels.Add(element.Label)
			Next element
			Return labels
		End Function

		''' <summary>
		''' Returns single element out of this sequence by its label
		''' </summary>
		''' <param name="label">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public T getElementByLabel(@NonNull String label)
		Public Overridable Function getElementByLabel(ByVal label As String) As T
			Return elementsMap(label)
		End Function

		''' <summary>
		''' Returns an ordered unmodifiable list of elements from this sequence
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Elements As IList(Of T)
			Get
				Return Collections.unmodifiableList(elements)
			End Get
		End Property

		''' <summary>
		''' Returns label for this sequence
		''' </summary>
		''' <returns> label for this sequence, null if label was not defined </returns>
		Public Overridable Property SequenceLabel As T
			Get
				Return label
			End Get
			Set(ByVal label As T)
				Me.label = label
				If Not labels.Contains(label) Then
					labels.Add(label)
				End If
			End Set
		End Property

		''' <summary>
		''' Returns all labels for this sequence
		''' 
		''' @return
		''' </summary>
		Public Overridable Property SequenceLabels As IList(Of T)
			Get
				Return labels
			End Get
			Set(ByVal labels As IList(Of T))
				Me.labels = labels
			End Set
		End Property



		''' <summary>
		'''  Adds sequence label. In this case sequence will have multiple labels
		''' </summary>
		''' <param name="label"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addSequenceLabel(@NonNull T label)
		Public Overridable Sub addSequenceLabel(ByVal label As T)
			Me.labels.Add(label)
			If Me.label Is Nothing Then
				Me.label = label
			End If
		End Sub

		''' <summary>
		''' Checks, if sequence is empty
		''' </summary>
		''' <returns> TRUE if empty, FALSE otherwise </returns>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return elements.Count = 0
			End Get
		End Property

		''' <summary>
		''' This method returns number of elements in this sequence
		''' 
		''' @return
		''' </summary>
		Public Overridable Function size() As Integer
			Return elements.Count
		End Function

		''' <summary>
		''' This method returns  sequence element by index
		''' </summary>
		''' <param name="index">
		''' @return </param>
		Public Overridable Function getElementByIndex(ByVal index As Integer) As T
			Return elements(index)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Sequence<?> sequence = (Sequence<?>) o;
			Dim sequence As Sequence(Of Object) = DirectCast(o, Sequence(Of Object))

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return elements != null ? elements.equals(sequence.elements) : sequence.elements == null;
			Return If(elements IsNot Nothing, elements.SequenceEqual(sequence.elements), sequence.elements Is Nothing)

		End Function

		Public Overrides Function GetHashCode() As Integer
			If hashCached Then
				Return hash
			End If

			For Each element As T In elements
				hash += 31 * element.GetHashCode()
			Next element

			hashCached = True

			Return hash
		End Function
	End Class

End Namespace