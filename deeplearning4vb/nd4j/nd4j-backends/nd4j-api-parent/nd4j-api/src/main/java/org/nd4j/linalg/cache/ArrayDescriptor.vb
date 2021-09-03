Imports System.Linq
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.linalg.cache


	Public Class ArrayDescriptor
		Friend boolArray() As Boolean = Nothing
		Friend intArray() As Integer = Nothing
		Friend floatArray() As Single = Nothing
		Friend doubleArray() As Double = Nothing
		Friend longArray() As Long = Nothing

		Private dtype As DataType

		Public Sub New(ByVal array() As Boolean, ByVal dtype As DataType)
			Me.boolArray = array
			Me.dtype = dtype
		End Sub

		Public Sub New(ByVal array() As Integer, ByVal dtype As DataType)
			Me.intArray = array
			Me.dtype = dtype
		End Sub

		Public Sub New(ByVal array() As Single, ByVal dtype As DataType)
			Me.floatArray = array
			Me.dtype = dtype
		End Sub

		Public Sub New(ByVal array() As Double, ByVal dtype As DataType)
			Me.doubleArray = array
			Me.dtype = dtype
		End Sub

		Public Sub New(ByVal array() As Long, ByVal dtype As DataType)
			Me.longArray = array
			Me.dtype = dtype
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As ArrayDescriptor = DirectCast(o, ArrayDescriptor)

			If Me.dtype <> that.dtype Then
				Return False
			End If

			If intArray IsNot Nothing AndAlso that.intArray IsNot Nothing Then
				Return intArray.SequenceEqual(that.intArray)
			ElseIf boolArray IsNot Nothing AndAlso that.boolArray IsNot Nothing Then
				Return intArray.SequenceEqual(that.intArray)
			ElseIf floatArray IsNot Nothing AndAlso that.floatArray IsNot Nothing Then
				Return floatArray.SequenceEqual(that.floatArray)
			ElseIf doubleArray IsNot Nothing AndAlso that.doubleArray IsNot Nothing Then
				Return doubleArray.SequenceEqual(that.doubleArray)
			ElseIf longArray IsNot Nothing AndAlso that.longArray IsNot Nothing Then
				Return longArray.SequenceEqual(that.longArray)
			Else
				Return False
			End If
		End Function

		Public Overrides Function GetHashCode() As Integer
			If intArray IsNot Nothing Then
				Return intArray.GetType().GetHashCode() + 31 * Arrays.hashCode(intArray) + 31 * dtype.ordinal()
			ElseIf floatArray IsNot Nothing Then
				Return floatArray.GetType().GetHashCode() + 31 * Arrays.hashCode(floatArray) + 31 * dtype.ordinal()
			ElseIf boolArray IsNot Nothing Then
				Return boolArray.GetType().GetHashCode() + 31 * Arrays.hashCode(boolArray) + 31 * dtype.ordinal()
			ElseIf doubleArray IsNot Nothing Then
				Return doubleArray.GetType().GetHashCode() + 31 * Arrays.hashCode(doubleArray) + 31 * dtype.ordinal()
			ElseIf longArray IsNot Nothing Then
				Return longArray.GetType().GetHashCode() + 31 * Arrays.hashCode(longArray) + 31 * dtype.ordinal()
			Else
				Return 0
			End If
		End Function
	End Class

End Namespace