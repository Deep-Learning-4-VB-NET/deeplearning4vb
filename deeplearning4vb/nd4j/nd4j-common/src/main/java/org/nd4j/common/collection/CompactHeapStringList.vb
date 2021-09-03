Imports System
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

Namespace org.nd4j.common.collection


	Public Class CompactHeapStringList
		Implements IList(Of String)

		Public Const DEFAULT_REALLOCATION_BLOCK_SIZE_BYTES As Integer = 8 * 1024 * 1024 '8MB
		Public Const DEFAULT_INTEGER_REALLOCATION_BLOCK_SIZE_BYTES As Integer = 1024 * 1024 '1MB - 262144 ints, 131k entries

		Private ReadOnly reallocationBlockSizeBytes As Integer
		Private ReadOnly reallocationIntegerBlockSizeBytes As Integer
		Private usedCount As Integer = 0
		Private nextDataOffset As Integer = 0
		Private data() As Char
		Private offsetAndLength() As Integer

		Public Sub New()
			Me.New(DEFAULT_REALLOCATION_BLOCK_SIZE_BYTES, DEFAULT_INTEGER_REALLOCATION_BLOCK_SIZE_BYTES)
		End Sub

		''' 
		''' <param name="reallocationBlockSizeBytes">    Number of bytes by which to increase the char[], when allocating a new storage array </param>
		''' <param name="intReallocationBlockSizeBytes"> Number of bytes by which to increase the int[], when allocating a new storage array </param>
		Public Sub New(ByVal reallocationBlockSizeBytes As Integer, ByVal intReallocationBlockSizeBytes As Integer)
			Me.reallocationBlockSizeBytes = reallocationBlockSizeBytes
			Me.reallocationIntegerBlockSizeBytes = intReallocationBlockSizeBytes

			Me.data = New Char((Me.reallocationBlockSizeBytes \ 2) - 1){}
			Me.offsetAndLength = New Integer((Me.reallocationIntegerBlockSizeBytes \ 4) - 1){}
		End Sub

		Public Overridable ReadOnly Property Count As Integer Implements ICollection(Of String).Count
			Get
				Return usedCount
			End Get
		End Property

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return usedCount = 0
			End Get
		End Property

		Public Overridable Function Contains(ByVal o As Object) As Boolean Implements ICollection(Of String).Contains
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerator(Of String).GetEnumerator
			Return New CompactHeapStringListIterator(Me)
		End Function

		Public Overrides Function toArray() As String()
			Dim str(usedCount - 1) As String
			For i As Integer = 0 To usedCount - 1
				str(i) = Me(i)
			Next i
			Return str
		End Function

		Public Overrides Function toArray(Of T)(ByVal a() As T) As T()
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function add(ByVal s As String) As Boolean
			Dim length As Integer = s.Length
			'3 possibilities:
			'(a) doesn't fit in char[]
			'(b) doesn't fit in int[]
			'(c) fits OK in both

			If nextDataOffset + length > data.Length Then
				'Allocate new data array, if possible
				If nextDataOffset > Integer.MaxValue - length Then
					Throw New System.NotSupportedException("Cannot allocate new data char[]: required array size exceeds Integer.MAX_VALUE")
				End If
				Dim toAdd As Integer = Math.Max(reallocationBlockSizeBytes \ 2, length)
				Dim newLength As Integer = data.Length + Math.Min(toAdd, Integer.MaxValue - data.Length)
				data = Arrays.CopyOf(data, newLength)
			End If
			If 2 * (usedCount + 1) >= offsetAndLength.Length Then
				If offsetAndLength.Length >= Integer.MaxValue - 2 Then
					'Should normally never happen
					Throw New System.NotSupportedException("Cannot allocate new offset int[]: required array size exceeds Integer.MAX_VALUE")
				End If
				Dim newLength As Integer = offsetAndLength.Length + Math.Min(reallocationIntegerBlockSizeBytes \ 4, Integer.MaxValue - offsetAndLength.Length)
				offsetAndLength = Arrays.CopyOf(offsetAndLength, newLength)
			End If


			s.CopyTo(0, data, nextDataOffset, length - 0)
			offsetAndLength(2 * usedCount) = nextDataOffset
			offsetAndLength(2 * usedCount + 1) = length
			nextDataOffset += length
			usedCount += 1

			Return True
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			'In principle we *could* do this with array copies
			Throw New System.NotSupportedException("Remove not supported")
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overrides Function addAll(Of T1 As String)(ByVal c As ICollection(Of T1)) As Boolean
			For Each s As String In c
				Add(s)
			Next s
			Return c.Count > 0
		End Function

		Public Overrides Function addAll(Of T1 As String)(ByVal index As Integer, ByVal c As ICollection(Of T1)) As Boolean
			'This is conceivably possible with array copies and adjusting the indices
			Throw New System.NotSupportedException("Add all at specified index: Not supported")
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException("Remove all: Not supported")
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException("Retain all: Not supported")
		End Function

		Public Overridable Sub Clear() Implements ICollection(Of String).Clear
			usedCount = 0
			nextDataOffset = 0
			data = New Char((reallocationBlockSizeBytes \ 2) - 1){}
			offsetAndLength = New Integer((reallocationIntegerBlockSizeBytes \ 4) - 1){}
		End Sub

		Public Overrides Function get(ByVal index As Integer) As String
			If index >= usedCount Then
				Throw New System.ArgumentException("Invalid index: " & index & " >= size(). Size = " & usedCount)
			End If
			Dim offset As Integer = offsetAndLength(2 * index)
			Dim length As Integer = offsetAndLength(2 * index + 1)
			Return New String(data, offset, length)
		End Function

		Public Overrides Function set(ByVal index As Integer, ByVal element As String) As String
			'This *could* be done with array copy ops...
			Throw New System.NotSupportedException("Set specified index: not supported due to serialized storage structure")
		End Function

		Public Overridable Sub Insert(ByVal index As Integer, ByVal element As String) Implements IList(Of String).Insert
			'This *could* be done with array copy ops...
			Throw New System.NotSupportedException("Set specified index: not supported due to serialized storage structure")
		End Sub

		Public Overrides Function remove(ByVal index As Integer) As String
			Throw New System.NotSupportedException("Remove: not supported")
		End Function

		Public Overridable Function IndexOf(ByVal o As Object) As Integer Implements IList(Of String).IndexOf
			If Not (TypeOf o Is String) Then
				Return -1
			End If

			Dim str As String = DirectCast(o, String)
			Dim ch() As Char = str.ToCharArray()


			For i As Integer = 0 To usedCount - 1
				If offsetAndLength(2 * i + 1) <> ch.Length Then
					'Can't be this one: lengths differ
					Continue For
				End If
				Dim offset As Integer = offsetAndLength(2 * i)

				Dim matches As Boolean = True
				For j As Integer = 0 To ch.Length - 1
					If data(offset + j) <> ch(j) Then
						matches = False
						Exit For
					End If
				Next j
				If matches Then
					Return i
				End If
			Next i

			Return -1
		End Function

		Public Overrides Function lastIndexOf(ByVal o As Object) As Integer
			If Not (TypeOf o Is String) Then
				Return -1
			End If

			Dim str As String = DirectCast(o, String)
			Dim ch() As Char = str.ToCharArray()


			For i As Integer = usedCount - 1 To 0 Step -1
				If offsetAndLength(2 * i + 1) <> ch.Length Then
					'Can't be this one: lengths differ
					Continue For
				End If
				Dim offset As Integer = offsetAndLength(2 * i)

				Dim matches As Boolean = True
				For j As Integer = 0 To ch.Length - 1
					If data(offset + j) <> ch(j) Then
						matches = False
						Exit For
					End If
				Next j
				If matches Then
					Return i
				End If
			Next i

			Return -1
		End Function

		Public Overrides Function listIterator() As IEnumerator(Of String)
			Return New CompactHeapStringListIterator(Me)
		End Function

		Public Overrides Function listIterator(ByVal index As Integer) As IEnumerator(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As IList(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then
				Return True
			End If
			If Not (TypeOf o Is System.Collections.IList) Then
				Return False
			End If

			Dim e1 As IEnumerator(Of String) = listIterator()
'JAVA TO VB CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: ListIterator<?> e2 = ((List<?>) o).listIterator();
			Dim e2 As IEnumerator(Of Object) = DirectCast(o, IList(Of Object)).GetEnumerator()
			Do While e1.MoveNext() AndAlso e2.MoveNext()
				Dim o1 As String = e1.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim o2 As Object = e2.next()
				If Not (If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))) Then
					Return False
				End If
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return Not (e1.hasNext() OrElse e2.hasNext())
		End Function

		Private Class CompactHeapStringListIterator
			Implements IEnumerator(Of String), IEnumerator(Of String)

			Private ReadOnly outerInstance As CompactHeapStringList

			Public Sub New(ByVal outerInstance As CompactHeapStringList)
				Me.outerInstance = outerInstance
			End Sub

			Friend currIdx As Integer = 0

			Public Overrides Function hasNext() As Boolean
				Return currIdx < outerInstance.usedCount
			End Function

			Public Overrides Function [next]() As String
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not hasNext() Then
					Throw New NoSuchElementException("No next element")
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return get(currIdx++);
				Dim tempVar = outerInstance.get(currIdx)
					currIdx += 1
					Return tempVar
			End Function

			Public Overrides Function hasPrevious() As Boolean
				Return currIdx > 0
			End Function

			Public Overrides Function previous() As String
				If Not hasPrevious() Then
					Throw New NoSuchElementException()
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return get(currIdx--);
				Dim tempVar = outerInstance.get(currIdx)
					currIdx -= 1
					Return tempVar
			End Function

			Public Overrides Function nextIndex() As Integer
				Return currIdx
			End Function

			Public Overrides Function previousIndex() As Integer
				Return currIdx
			End Function

			Public Overrides Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub set(ByVal s As String)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub add(ByVal s As String)
				Throw New System.NotSupportedException()
			End Sub
		End Class
	End Class

End Namespace