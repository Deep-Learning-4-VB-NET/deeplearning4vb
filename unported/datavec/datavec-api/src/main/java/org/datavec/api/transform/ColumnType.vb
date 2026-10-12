Imports System.Collections.Generic
Imports org.datavec.api.transform.metadata
Imports WritableType = org.datavec.api.writable.WritableType

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

Namespace org.datavec.api.transform

	Public NotInheritable Class ColumnType
		Public Shared ReadOnly String As New ColumnType("String", InnerEnum.String)
		Public Shared ReadOnly Integer As New ColumnType("Integer", InnerEnum.Integer)
		Public Shared ReadOnly Long As New ColumnType("Long", InnerEnum.Long)
		Public Shared ReadOnly Double As New ColumnType("Double", InnerEnum.Double)
		Public Shared ReadOnly Float As New ColumnType("Float", InnerEnum.Float)
		Public Shared ReadOnly Categorical As New ColumnType("Categorical", InnerEnum.Categorical)
		Public Shared ReadOnly Time As New ColumnType("Time", InnerEnum.Time)
		Public Shared ReadOnly Bytes As New ColumnType("Bytes", InnerEnum.Bytes) 'Arbitrary byte[] data
		Public Shared ReadOnly Boolean As New ColumnType("Boolean", InnerEnum.Boolean)
		Public Shared ReadOnly NDArray As New ColumnType("NDArray", InnerEnum.NDArray)

		Private Shared ReadOnly valueList As New List(Of ColumnType)()

		Shared Sub New()
			valueList.Add(String)
			valueList.Add(Integer)
			valueList.Add(Long)
			valueList.Add(Double)
			valueList.Add(Float)
			valueList.Add(Categorical)
			valueList.Add(Time)
			valueList.Add(Bytes)
			valueList.Add(Boolean)
			valueList.Add(NDArray)
		End Sub

		Public Enum InnerEnum
			String
			Integer
			Long
			Double
			Float
			Categorical
			Time
			Bytes
			Boolean
			NDArray
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

		Public Function newColumnMetaData(ByVal columnName As String) As ColumnMetaData
			Select Case Me
				Case String
					Return New StringMetaData(columnName)
				Case Integer?
					Return New IntegerMetaData(columnName)
				Case Long?
					Return New LongMetaData(columnName)
				Case Double?
					Return New DoubleMetaData(columnName)
				Case Single?
					Return New FloatMetaData(columnName)
				Case Time
					Return New TimeMetaData(columnName)
				Case Boolean?
					Return New CategoricalMetaData(columnName, "true", "false")
				Case Categorical
					Throw New System.NotSupportedException("Cannot create new categorical column using this method: categorical state names would be unknown")
				Case NDArray
					Throw New System.NotSupportedException("Cannot create new NDArray column using this method: shape information would be unknown")
				Case Else 'And Bytes
					Throw New System.NotSupportedException("Unknown or not supported column type: " & Me)
			End Select
		End Function

		''' <returns> The type of writable for this column </returns>
		Public ReadOnly Property WritableType As org.datavec.api.writable.WritableType
			Get
				Select Case Me
					Case String
						Return WritableType.Text
					Case Integer?
						Return WritableType.Int
					Case Long?
						Return WritableType.Long
					Case Double?
						Return WritableType.Double
					Case Single?
						Return WritableType.Float
					Case Categorical
						Return WritableType.Text
					Case Time
						Return WritableType.Long
					Case Bytes
						Return WritableType.Byte
					Case Boolean?
						Return WritableType.Boolean
					Case NDArray
						Return WritableType.Image
					Case Else
						Throw New System.InvalidOperationException("Unknown writable type for column type: " & Me)
				End Select
			End Get
		End Property


		Public Shared Function values() As ColumnType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As ColumnType, ByVal two As ColumnType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As ColumnType, ByVal two As ColumnType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As ColumnType
			For Each enumInstance As ColumnType In ColumnType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace