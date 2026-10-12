Imports System.Collections.Generic
Imports System.Linq
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports FieldVector = org.apache.arrow.vector.FieldVector
Imports VectorSchemaRoot = org.apache.arrow.vector.VectorSchemaRoot
Imports VectorUnloader = org.apache.arrow.vector.VectorUnloader
Imports ArrowRecordBatch = org.apache.arrow.vector.ipc.message.ArrowRecordBatch
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports AbstractTimeSeriesWritableRecordBatch = org.datavec.api.writable.batch.AbstractTimeSeriesWritableRecordBatch

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

Namespace org.datavec.arrow.recordreader


	''' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public class ArrowWritableRecordTimeSeriesBatch extends org.datavec.api.writable.batch.AbstractTimeSeriesWritableRecordBatch implements java.io.Closeable
	Public Class ArrowWritableRecordTimeSeriesBatch
		Inherits AbstractTimeSeriesWritableRecordBatch
		Implements System.IDisposable

		Private list As IList(Of FieldVector)
'JAVA TO VB CONVERTER NOTE: The field size was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private size_Conflict As Integer
		Private schema As Schema
		Private arrowRecordBatch As ArrowRecordBatch
		Private vectorLoader As VectorSchemaRoot
		Private unloader As VectorUnloader
		Private timeSeriesStride As Integer

		''' <summary>
		''' An index in to an individual
		''' <seealso cref="ArrowRecordBatch"/> </summary>
		''' <param name="list"> the list of field vectors to use </param>
		''' <param name="schema"> the schema to use </param>
		Public Sub New(ByVal list As IList(Of FieldVector), ByVal schema As Schema, ByVal timeSeriesStride As Integer)
			Me.list = list
			Me.schema = schema
			'each column should have same number of rows
			Me.timeSeriesStride = timeSeriesStride
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Me.size_Conflict = list.Count * list(0).getValueCount() / timeSeriesStride

		End Sub

		Public Overridable Function toArrayList() As IList(Of IList(Of IList(Of Writable)))
			Dim ret As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim i As Integer = 0
			Do While i < Count
				Dim timeStep As IList(Of IList(Of Writable)) = Me(i)
				Dim addTimeStep As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				For j As Integer = 0 To timeStep.Count - 1
					Dim addingFrom As IList(Of Writable) = timeStep(j)
					Dim currRecord As IList(Of Writable) = New List(Of Writable)(addingFrom)
					addTimeStep.Add(currRecord)
				Next j

				ret.Add(addTimeStep)
				i += 1
			Loop

			Return ret
		End Function


		Public Overridable ReadOnly Property Count As Integer
			Get
				Return size_Conflict
			End Get
		End Property

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return size_Conflict = 0
			End Get
		End Property

		Public Overrides Function Contains(ByVal o As Object) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function GetEnumerator() As IEnumerator(Of IList(Of IList(Of Writable)))
			Return New ArrowListIterator(Me)
		End Function

		Public Overrides Function toArray() As Object()
			Dim ret(Count - 1) As Object
			For i As Integer = 0 To ret.Length - 1
				ret(i) = Me(i)
			Next i
			Return ret
		End Function

		Public Overrides Function toArray(Of T)(ByVal ts() As T) As T()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function add(ByVal writable As IList(Of IList(Of Writable))) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return False
		End Function

		Public Overrides Function addAll(Of T1 As IList(Of IList(Of Writable)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function addAll(Of T1 As IList(Of IList(Of Writable)(ByVal i As Integer, ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Sub Clear()

		End Sub

		Public Overrides Function get(ByVal i As Integer) As IList(Of IList(Of Writable))
			Dim ret As New ArrowWritableRecordBatch(list,schema,i,timeSeriesStride \ schema.numColumns())
			Return ret
		End Function

		Public Overrides Function set(ByVal i As Integer, ByVal writable As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Dim arrowWritableRecordBatch As ArrowWritableRecordBatch = CType(Me(i), ArrowWritableRecordBatch)
			For batch As Integer = 0 To writable.Count - 1
				arrowWritableRecordBatch(batch) = writable(i)
			Next batch

			Return arrowWritableRecordBatch
		End Function

		Public Overridable Sub Insert(ByVal i As Integer, ByVal writable As IList(Of IList(Of Writable)))
			Throw New System.NotSupportedException()

		End Sub

		Public Overrides Function remove(ByVal i As Integer) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function IndexOf(ByVal o As Object) As Integer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function lastIndexOf(ByVal o As Object) As Integer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function listIterator() As IEnumerator(Of IList(Of IList(Of Writable)))
			Return New ArrowListIterator(Me)
		End Function

		Public Overrides Function listIterator(ByVal i As Integer) As IEnumerator(Of IList(Of IList(Of Writable)))
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function subList(ByVal i As Integer, ByVal i1 As Integer) As IList(Of IList(Of IList(Of Writable)))
			Return Nothing
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (!super.equals(o))
			If Not MyBase.SequenceEqual(o) Then
				Return False
			End If
			Dim lists As ArrowWritableRecordTimeSeriesBatch = DirectCast(o, ArrowWritableRecordTimeSeriesBatch)
			Return size_Conflict = lists.size_Conflict AndAlso Objects.equals(list, lists.list) AndAlso Objects.equals(schema, lists.schema)
		End Function

		Public Overrides Function GetHashCode() As Integer

			Return Objects.hash(MyBase.GetHashCode(), list, size_Conflict, schema)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If arrowRecordBatch IsNot Nothing Then
				arrowRecordBatch.close()
			End If
			If vectorLoader IsNot Nothing Then
				vectorLoader.close()
			End If


		End Sub


		Private Class ArrowListIterator
			Implements IEnumerator(Of IList(Of IList(Of Writable)))

			Private ReadOnly outerInstance As ArrowWritableRecordTimeSeriesBatch

			Public Sub New(ByVal outerInstance As ArrowWritableRecordTimeSeriesBatch)
				Me.outerInstance = outerInstance
			End Sub

			Friend index As Integer

			Public Overrides Function hasNext() As Boolean
				Return index < outerInstance.size_Conflict
			End Function

			Public Overrides Function [next]() As IList(Of IList(Of Writable))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return get(index++);
				Dim tempVar = outerInstance.get(index)
					index += 1
					Return tempVar
			End Function

			Public Overrides Function hasPrevious() As Boolean
				Return index > 0
			End Function

			Public Overrides Function previous() As IList(Of IList(Of Writable))
				Return outerInstance.get(index - 1)
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

			Public Overrides Sub set(ByVal writables As IList(Of IList(Of Writable)))
				outerInstance(index) = writables
			End Sub

			Public Overrides Sub add(ByVal writables As IList(Of IList(Of Writable)))
				Throw New System.NotSupportedException()

			End Sub
		End Class


	End Class

End Namespace