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


	Public Class ShuffledListIterator(Of T)
		Implements IEnumerator(Of T)

		Private ReadOnly list As IList(Of T)
		Private ReadOnly order() As Integer
		Private currentPosition As Integer = 0

		Public Sub New(ByVal list As IList(Of T), ByVal order() As Integer)
			If order IsNot Nothing AndAlso list.Count <> order.Length Then
				Throw New System.ArgumentException("Order array and list sizes differ")
			End If
			Me.list = list
			Me.order = order
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return currentPosition < list.Count
		End Function

		Public Overrides Function [next]() As T
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If

			Dim nextPos As Integer = (If(order IsNot Nothing, order(currentPosition), currentPosition))
			currentPosition += 1
			Return list(nextPos)
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub
	End Class

End Namespace