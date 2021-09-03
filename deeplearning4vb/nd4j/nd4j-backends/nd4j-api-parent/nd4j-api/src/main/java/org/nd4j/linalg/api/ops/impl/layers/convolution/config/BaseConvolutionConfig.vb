Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution.config



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseConvolutionConfig
	Public MustInherit Class BaseConvolutionConfig

		Public MustOverride Function toProperties() As IDictionary(Of String, Object)

		''' <summary>
		''' Get the value for a given property
		''' for this function
		''' </summary>
		''' <param name="property"> the property to get </param>
		''' <returns> the value for the function if it exists </returns>
		Public Overridable Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			Try
				Return [property].get(Me)
			Catch e As IllegalAccessException
				Throw New Exception(e)
			End Try
		End Function


		''' <summary>
		''' Set the value for this function.
		''' Note that if value is null an <seealso cref="ND4JIllegalStateException"/>
		''' will be thrown.
		''' </summary>
		''' <param name="target"> the target field </param>
		''' <param name="value">  the value to set </param>
		Public Overridable Sub setValueFor(ByVal target As System.Reflection.FieldInfo, ByVal value As Object)
			If value Is Nothing Then
				Throw New ND4JIllegalStateException("Unable to set field " & target & " using null value!")
			End If

			value = ensureProperType(target, value)

			Try
				target.set(Me, value)
			Catch e As IllegalAccessException
				log.error("",e)
			End Try
		End Sub

		Private Function ensureProperType(ByVal targetType As System.Reflection.FieldInfo, ByVal value As Object) As Object
			Dim firstClass As val = targetType.getType()
			Dim valueType As val = value.GetType()
			If Not firstClass.Equals(valueType) Then
				If firstClass.Equals(GetType(Integer())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.intValue()
					End If

					Dim otherValue As Integer = DirectCast(value, Integer)
					Dim setValue() As Integer = {otherValue}
					Return setValue
				ElseIf firstClass.Equals(GetType(Integer?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.intValue()
					End If

					Dim otherValue As Integer? = DirectCast(value, Integer?)
					Dim setValue() As Integer? = {otherValue}
					Return setValue
				ElseIf firstClass.Equals(GetType(Long())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.longValue()
					End If

					Dim otherValue As Long = DirectCast(value, Long)
					Dim setValue() As Long = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Long?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.longValue()
					End If

					Dim otherValue As Long? = DirectCast(value, Long?)
					Dim setValue() As Long? = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Double())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.doubleValue()
					End If


					Dim otherValue As Double = DirectCast(value, Double)
					Dim setValue() As Double = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Double?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.doubleValue()
					End If


					Dim otherValue As Double? = DirectCast(value, Double?)
					Dim setValue() As Double? = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Single())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.floatValue()
					End If


					Dim otherValue As Single = DirectCast(value, Single)
					Dim setValue() As Single = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Single?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.floatValue()
					End If


					Dim otherValue As Single? = DirectCast(value, Single?)
					Dim setValue() As Single? = {otherValue}
					Return setValue

				End If
			End If

			Return value
		End Function


		Protected Friend MustOverride Sub validate()
	End Class

End Namespace