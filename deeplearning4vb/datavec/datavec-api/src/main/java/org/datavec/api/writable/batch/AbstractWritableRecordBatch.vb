Imports System.Collections.Generic
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.writable.batch


	Public MustInherit Class AbstractWritableRecordBatch
		Implements IList(Of IList(Of Writable))


		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return size() = 0
			End Get
		End Property

		Public Overridable Function Contains(ByVal o As Object) As Boolean Implements ICollection(Of IList(Of Writable)).Contains
			Return False
		End Function

		Public Overridable Function GetEnumerator() As IEnumerator(Of IList(Of Writable)) Implements IEnumerator(Of IList(Of Writable)).GetEnumerator
			Return listIterator()
		End Function

		Public Overrides Function listIterator() As IEnumerator(Of IList(Of Writable))
			Return New RecordBatchListIterator(Me)
		End Function

		Public Overrides Function toArray() As Object()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function toArray(Of T)(ByVal ts() As T) As T()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function add(ByVal writable As IList(Of Writable)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return False
		End Function

		Public Overrides Function addAll(Of T1 As IList(Of Writable)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function addAll(Of T1 As IList(Of Writable)(ByVal i As Integer, ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub Clear() Implements ICollection(Of IList(Of Writable)).Clear

		End Sub

		Public Overrides Function set(ByVal i As Integer, ByVal writable As IList(Of Writable)) As IList(Of Writable)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub Insert(ByVal i As Integer, ByVal writable As IList(Of Writable)) Implements IList(Of IList(Of Writable)).Insert
			Throw New System.NotSupportedException()

		End Sub

		Public Overrides Function remove(ByVal i As Integer) As IList(Of Writable)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function IndexOf(ByVal o As Object) As Integer Implements IList(Of IList(Of Writable)).IndexOf
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function lastIndexOf(ByVal o As Object) As Integer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function listIterator(ByVal i As Integer) As IEnumerator(Of IList(Of Writable))
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function subList(ByVal i As Integer, ByVal i1 As Integer) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException()
		End Function


		Public Class RecordBatchListIterator
			Implements IEnumerator(Of IList(Of Writable))

			Friend index As Integer
			Friend underlying As AbstractWritableRecordBatch

			Public Sub New(ByVal underlying As AbstractWritableRecordBatch)
				Me.underlying = underlying
			End Sub

			Public Overrides Function hasNext() As Boolean
				Return index < underlying.Count
			End Function

			Public Overrides Function [next]() As IList(Of Writable)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return underlying.get(index++);
				Dim tempVar = underlying(index)
					index += 1
					Return tempVar
			End Function

			Public Overrides Function hasPrevious() As Boolean
				Return index > 0
			End Function

			Public Overrides Function previous() As IList(Of Writable)
				Return underlying(index - 1)
			End Function

			Public Overrides Function nextIndex() As Integer
				Return index + 1
			End Function

			Public Overrides Function previousIndex() As Integer
				Return index - 1
			End Function

			Public Overrides Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub set(ByVal writables As IList(Of Writable))
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub add(ByVal writables As IList(Of Writable))
				Throw New System.NotSupportedException()

			End Sub
		End Class
	End Class

End Namespace