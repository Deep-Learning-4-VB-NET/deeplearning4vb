Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection

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

Namespace org.nd4j.linalg.api.ops.performance.primitives


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AveragingTransactionsHolder
	Public Class AveragingTransactionsHolder
		Private ReadOnly storage As IList(Of IList(Of Long)) = New List(Of IList(Of Long))(System.Enum.GetValues(GetType(MemcpyDirection)).length)
		Private ReadOnly locks As IList(Of ReentrantReadWriteLock)= New List(Of ReentrantReadWriteLock)(System.Enum.GetValues(GetType(MemcpyDirection)).length)

		Public Sub New()
			init()
		End Sub

		Protected Friend Overridable Sub init()
			' filling map withi initial keys
			For Each v As val In System.Enum.GetValues(GetType(MemcpyDirection))
				Dim o As val = v.ordinal()
				storage.Insert(o, New List(Of Long)())

				locks.Insert(o, New ReentrantReadWriteLock())
			Next v
		End Sub

		Public Overridable Sub clear()
			For Each v As val In System.Enum.GetValues(GetType(MemcpyDirection))
				Dim o As val = v.ordinal()
				Try
					locks(o).writeLock().lock()

					storage(o).Clear()
				Finally
					locks(o).writeLock().unlock()
				End Try
			Next v
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addValue(@NonNull MemcpyDirection direction, System.Nullable<Long> value)
		Public Overridable Sub addValue(ByVal direction As MemcpyDirection, ByVal value As Long?)
			Dim o As val = direction.ordinal()
			Try
				locks(o).writeLock().lock()

				storage(o).Add(value)
			Finally
				locks(o).writeLock().unlock()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public System.Nullable<Long> getAverageValue(@NonNull MemcpyDirection direction)
		Public Overridable Function getAverageValue(ByVal direction As MemcpyDirection) As Long?
			Dim o As val = direction.ordinal()
			Try
				Dim r As Long? = 0L
				locks(o).readLock().lock()

				Dim list As val = storage(o)

				If list.isEmpty() Then
					Return 0L
				End If

				For Each v As val In list
					r += v
				Next v

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return r.Value / list.size()
			Finally
				locks(o).readLock().unlock()
			End Try
		End Function
	End Class

End Namespace