Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull

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

Namespace org.nd4j.common.primitives


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class @Optional<T>
	Public Class [Optional](Of T)
'JAVA TO VB CONVERTER NOTE: The field EMPTY was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly EMPTY_Conflict As [Optional] = New [Optional]()

		Private ReadOnly value As T

		Private Sub New()
			Me.New(Nothing)
		End Sub

		Private Sub New(ByVal value As T)
			Me.value = value
		End Sub

		''' <summary>
		''' Returns an empty Optional instance. No value is present for this Optional.
		''' 
		''' </summary>
		Public Shared Function empty(Of T)() As [Optional](Of T)
			Return CType(EMPTY_Conflict, [Optional](Of T))
		End Function

		''' <summary>
		''' Returns an Optional with the specified present non-null value.
		''' </summary>
		''' <param name="value"> the value to be present, which must be non-null </param>
		''' <returns> an Optional with the value present </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T> @Optional<T> of(@NonNull T value)
		Public Shared Function [of](Of T)(ByVal value As T) As [Optional](Of T)
			Return New [Optional](Of T)(value)
		End Function

		''' <summary>
		''' Returns an Optional describing the specified value, if non-null, otherwise returns an empty Optional.
		''' </summary>
		''' <param name="value"> the possibly-null value to describe </param>
		''' <returns> an Optional with a present value if the specified value is non-null, otherwise an empty Optional </returns>
		Public Shared Function ofNullable(Of T)(ByVal value As T) As [Optional](Of T)
			If value Is Nothing Then
				Return empty()
			End If
			Return New [Optional](Of T)(value)
		End Function

		''' <summary>
		''' If a value is present in this Optional, returns the value, otherwise throws NoSuchElementException.
		''' </summary>
		''' <returns> the non-null value held by this Optional </returns>
		''' <exception cref="NoSuchElementException"> - if there is no value present </exception>
		Public Overridable Function get() As T
			If Not Present Then
				Throw New NoSuchElementException("Optional is empty")
			End If
			Return value
		End Function

		''' <summary>
		''' Return true if there is a value present, otherwise false.
		''' </summary>
		''' <returns> true if there is a value present, otherwise false </returns>
		Public Overridable ReadOnly Property Present As Boolean
			Get
				Return value IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Return the value if present, otherwise return other.
		''' </summary>
		''' <param name="other">  the value to be returned if there is no value present, may be null
		''' @return </param>
		Public Overridable Function [orElse](ByVal other As T) As T
			If Present Then
				Return get()
			End If
			Return other
		End Function

		Public Overrides Function ToString() As String
			If Present Then
				Return "Optional(" & value.ToString() & ")"
			End If
			Return "Optional()"
		End Function
	End Class

End Namespace