Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp

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

Namespace org.nd4j.linalg.api.memory.pointers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class PagedPointer extends Pointer
	Public Class PagedPointer
		Inherits Pointer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Pointer originalPointer;
		Private originalPointer As Pointer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private boolean leaked = false;
		Private leaked As Boolean = False

		Private Sub New()

		End Sub


		Public Sub New(ByVal address As Long)
			Me.originalPointer = Nothing

			Me.address = address

			Me.capacity = 0
			Me.limit = 0
			Me.position = 0
		End Sub

		Public Sub New(ByVal pointer As Pointer)
			Me.originalPointer = pointer

			Me.address = pointer.address()

			Me.capacity = pointer.capacity()
			Me.limit = pointer.limit()
			Me.position = 0
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal capacity As Long)
			Me.originalPointer = pointer

			Me.address = If(pointer Is Nothing, 0, pointer.address())

			Me.capacity = capacity
			Me.limit = capacity
			Me.position = 0
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal capacity As Long, ByVal offset As Long)
			Me.address = pointer.address() + offset

			Me.capacity = capacity
			Me.limit = capacity
			Me.position = 0
		End Sub


		Public Overridable Function withOffset(ByVal offset As Long, ByVal capacity As Long) As PagedPointer
			Return New PagedPointer(Me, capacity, offset)
		End Function


		Public Overridable Function asFloatPointer() As FloatPointer
			Return New ImmortalFloatPointer(Me)
		End Function

		Public Overridable Function asDoublePointer() As DoublePointer
			Return New DoublePointer(Me)
		End Function

		Public Overridable Function asIntPointer() As IntPointer
			Return New IntPointer(Me)
		End Function

		Public Overridable Function asLongPointer() As LongPointer
			Return New LongPointer(Me)
		End Function

		Public Overridable Function asBytePointer() As BytePointer
			Return New BytePointer(Me)
		End Function

		Public Overridable Function asShortPointer() As ShortPointer
			Return New ShortPointer(Me)
		End Function

		Public Overridable Function asBoolPointer() As BooleanPointer
			Return New BooleanPointer(Me)
		End Function

		Public Overrides Sub deallocate()
			MyBase.deallocate()
		End Sub

		Public Overrides Sub deallocate(ByVal deallocate As Boolean)
			MyBase.deallocate(True)
		End Sub
	End Class

End Namespace