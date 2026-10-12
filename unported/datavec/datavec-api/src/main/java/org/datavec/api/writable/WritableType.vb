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

Namespace org.datavec.api.writable

	Public NotInheritable Class WritableType
		Public Shared ReadOnly Boolean As New WritableType("Boolean", InnerEnum.Boolean)
		Public Shared ReadOnly Byte As New WritableType("Byte", InnerEnum.Byte)
		Public Shared ReadOnly Double As New WritableType("Double", InnerEnum.Double)
		Public Shared ReadOnly Float As New WritableType("Float", InnerEnum.Float)
		Public Shared ReadOnly Int As New WritableType("Int", InnerEnum.Int)
		Public Shared ReadOnly Long As New WritableType("Long", InnerEnum.Long)
		Public Shared ReadOnly Null As New WritableType("Null", InnerEnum.Null)
		Public Shared ReadOnly Text As New WritableType("Text", InnerEnum.Text)
		Public Shared ReadOnly NDArray As New WritableType("NDArray", InnerEnum.NDArray)
		Public Shared ReadOnly Image As New WritableType("Image", InnerEnum.Image)
		Public Shared ReadOnly Arrow As New WritableType("Arrow", InnerEnum.Arrow)
		Public Shared ReadOnly Bytes As New WritableType("Bytes", InnerEnum.Bytes)

		Private Shared ReadOnly valueList As New List(Of WritableType)()

		Shared Sub New()
			valueList.Add(Boolean)
			valueList.Add(Byte)
			valueList.Add(Double)
			valueList.Add(Float)
			valueList.Add(Int)
			valueList.Add(Long)
			valueList.Add(Null)
			valueList.Add(Text)
			valueList.Add(NDArray)
			valueList.Add(Image)
			valueList.Add(Arrow)
			valueList.Add(Bytes)
		End Sub

		Public Enum InnerEnum
			Boolean
			Byte
			Double
			Float
			Int
			Long
			Null
			Text
			NDArray
			Image
			Arrow
			Bytes
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub

		'NOTE TO DEVELOPERS:
		'In the current implementation, the order (ordinal idx) for the WritableType values matters.
		'New writables can be added to the end of the list, but not between exiting types, as this will change the
		'ordinal value for all writable types that follow, which will mess up serialization in some cases (like Spark
		' sequence and map files)
		'Alternatively, modify WritableType.typeIdx() to ensure backward compatibility


		''' 
		''' <returns> True if Writable is defined in datavec-api, false otherwise </returns>
		Public ReadOnly Property CoreWritable As Boolean
			Get
				Select Case Me
					Case Image, Arrow
						Return False
					Case Else
						Return True
				End Select
			End Get
		End Property

		''' <summary>
		''' Return a unique type index for the given writable
		''' </summary>
		''' <returns> Type index for the writable </returns>
		Public Function typeIdx() As Short
			Return CShort(Math.Truncate(Me.ordinal()))
		End Function

		''' <summary>
		''' Return the class of the implementation corresponding to each WritableType.
		''' Note that if <seealso cref="isCoreWritable()"/> returns false, null will be returned by this method.
		''' </summary>
		''' <returns> Class for the given WritableType </returns>
		Public ReadOnly Property WritableClass As Type
			Get
				Select Case Me
					Case Boolean?
						Return GetType(BooleanWritable)
					Case SByte?
						Return GetType(ByteWritable)
					Case Double?
						Return GetType(DoubleWritable)
					Case Single?
						Return GetType(FloatWritable)
					Case Int
						Return GetType(IntWritable)
					Case Long?
						Return GetType(LongWritable)
					Case Null
						Return GetType(NullWritable)
					Case Text
						Return GetType(Text)
					Case NDArray
						Return GetType(NDArrayWritable)
					Case Bytes
						Return GetType(ByteWritable)
					Case Else
						Return Nothing
				End Select
			End Get
		End Property


		Public Shared Function values() As WritableType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As WritableType, ByVal two As WritableType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As WritableType, ByVal two As WritableType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As WritableType
			For Each enumInstance As WritableType In WritableType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace