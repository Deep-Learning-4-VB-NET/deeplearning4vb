Imports System.Collections.Generic
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.iter


	Public Class FlatIterator
		Implements IEnumerator(Of Integer())

		Private shape() As Integer
		Private runningDimension As Integer
		Private currentCoord() As Integer
		Private length As Integer
		Private current As Integer = 0

		Public Sub New(ByVal shape() As Integer)
			Me.shape = shape
			Me.currentCoord = New Integer(shape.Length - 1){}
			length = ArrayUtil.prod(shape)
		End Sub

		Public Overrides Sub remove()

		End Sub

		Public Overrides Function hasNext() As Boolean
			Return current < length
		End Function

		Public Overrides Function [next]() As Integer()
			If currentCoord(runningDimension) = shape(runningDimension) Then
				runningDimension -= 1
				currentCoord(runningDimension) = 0
				If runningDimension < shape.Length Then

				End If
			Else
				'bump to the next coordinate
				currentCoord(runningDimension) += 1
			End If
			current += 1
			Return currentCoord
		End Function
	End Class

End Namespace