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

Namespace org.nd4j.linalg.api.buffer

	''' <summary>
	''' Enum lists supported data types.
	''' 
	''' </summary>
	Public NotInheritable Class DataType

		Public Shared ReadOnly [DOUBLE] As New DataType("@DOUBLE", InnerEnum.DOUBLE)
		Public Shared ReadOnly FLOAT As New DataType("FLOAT", InnerEnum.FLOAT)

		''' @deprecated Replaced by <seealso cref="DataType.FLOAT16"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.FLOAT16""/>, use that instead")>
		Public Shared ReadOnly HALF As New DataType("HALF", InnerEnum.HALF)

		''' @deprecated Replaced by <seealso cref="DataType.INT64"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.INT64""/>, use that instead")>
		Public Shared ReadOnly [LONG] As New DataType("@LONG", InnerEnum.LONG)

		''' @deprecated Replaced by <seealso cref="DataType.INT32"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.INT32""/>, use that instead")>
		Public Shared ReadOnly INT As New DataType("INT", InnerEnum.INT)

		''' @deprecated Replaced by <seealso cref="DataType.INT16"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.INT16""/>, use that instead")>
		Public Shared ReadOnly [SHORT] As New DataType("@SHORT", InnerEnum.SHORT)

		''' @deprecated Replaced by <seealso cref="DataType.UINT8"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.UINT8""/>, use that instead")>
		Public Shared ReadOnly UBYTE As New DataType("UBYTE", InnerEnum.UBYTE)

		''' @deprecated Replaced by <seealso cref="DataType.INT8"/>, use that instead 
		<System.Obsolete("Replaced by <seealso cref=""DataType.INT8""/>, use that instead")>
		Public Shared ReadOnly [BYTE] As New DataType("@BYTE", InnerEnum.BYTE)

		Public Shared ReadOnly BOOL As New DataType("BOOL", InnerEnum.BOOL)
		Public Shared ReadOnly UTF8 As New DataType("UTF8", InnerEnum.UTF8)
		Public Shared ReadOnly COMPRESSED As New DataType("COMPRESSED", InnerEnum.COMPRESSED)
		Public Shared ReadOnly BFLOAT16 As New DataType("BFLOAT16", InnerEnum.BFLOAT16)
		Public Shared ReadOnly UINT16 As New DataType("UINT16", InnerEnum.UINT16)
		Public Shared ReadOnly UINT32 As New DataType("UINT32", InnerEnum.UINT32)
		Public Shared ReadOnly UINT64 As New DataType("UINT64", InnerEnum.UINT64)
		Public Shared ReadOnly UNKNOWN As New DataType("UNKNOWN", InnerEnum.UNKNOWN)

		Private Shared ReadOnly valueList As New List(Of DataType)()

		Shared Sub New()
			valueList.Add([DOUBLE])
			valueList.Add(FLOAT)
			valueList.Add(HALF)
			valueList.Add([LONG])
			valueList.Add(INT)
			valueList.Add([SHORT])
			valueList.Add(UBYTE)
			valueList.Add([BYTE])
			valueList.Add(BOOL)
			valueList.Add(UTF8)
			valueList.Add(COMPRESSED)
			valueList.Add(BFLOAT16)
			valueList.Add(UINT16)
			valueList.Add(UINT32)
			valueList.Add(UINT64)
			valueList.Add(UNKNOWN)
		End Sub

		Public Enum InnerEnum
			[DOUBLE]
			FLOAT
			HALF
			[LONG]
			INT
			[SHORT]
			UBYTE
			[BYTE]
			BOOL
			UTF8
			COMPRESSED
			BFLOAT16
			UINT16
			UINT32
			UINT64
			UNKNOWN
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

		Public Const FLOAT16 As DataType = DataType.HALF
		Public Const INT32 As DataType = DataType.INT
		Public Const INT64 As DataType = DataType.LONG
		Public Const INT16 As DataType = DataType.SHORT
		Public Const INT8 As DataType = DataType.BYTE
		Public Const UINT8 As DataType = DataType.UBYTE


		''' <summary>
		''' Values inherited from
		'''  https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/array/DataType.h </summary>
		''' <param name="type"> the input int type </param>
		''' <returns> the appropriate data type </returns>
		Public Shared Function fromInt(ByVal type As Integer) As DataType
			Select Case type
				Case 1
					Return BOOL
				Case 2
					Return FLOAT
				Case 3
					Return HALF
				Case 4
					Return HALF
				Case 5
					Return FLOAT
				Case 6
					Return [DOUBLE]
				Case 7
					Return [BYTE]
				Case 8
					Return [SHORT]
				Case 9
					Return INT
				Case 10
					Return [LONG]
				Case 11
					Return UBYTE
				Case 12
					Return UINT16
				Case 13
					Return UINT32
				Case 14
					Return UINT64
				Case 17
					Return BFLOAT16
				Case 100
					Return DataType.UNKNOWN
				Case Else
					Throw New System.NotSupportedException("Unknown data type: [" & type & "]")
			End Select
		End Function

		Public Function toInt() As Integer
			Select Case Me
				Case BOOL
					Return 1
				Case HALF
					Return 3
				Case FLOAT
					Return 5
				Case [DOUBLE]
					Return 6
				Case [BYTE]
					Return 7
				Case [SHORT]
					Return 8
				Case INT
					Return 9
				Case [LONG]
					Return 10
				Case UBYTE
					Return 11
				Case UINT16
					Return 12
				Case UINT32
					Return 13
				Case UINT64
					Return 14
				Case BFLOAT16
					Return 17
				Case UTF8
					Return 50
				Case Else
					Throw New System.NotSupportedException("Non-covered data type: [" & Me & "]")
			End Select
		End Function

		''' <returns> Returns true if the datatype is a floating point type (double, float or half precision) </returns>
		Public ReadOnly Property FPType As Boolean
			Get
				Return Me = FLOAT OrElse Me = [DOUBLE] OrElse Me = HALF OrElse Me = BFLOAT16
			End Get
		End Property

		''' <returns> Returns true if the datatype is an integer type (long, integer, short, ubyte or byte) </returns>
		Public ReadOnly Property IntType As Boolean
			Get
				Return Me = [LONG] OrElse Me = INT OrElse Me = [SHORT] OrElse Me = UBYTE OrElse Me = [BYTE] OrElse Me = UINT16 OrElse Me = UINT32 OrElse Me = UINT64
			End Get
		End Property

		''' <summary>
		''' Return true if the value is numerical.<br>
		''' Equivalent to {@code this != UTF8 && this != COMPRESSED && this != UNKNOWN}<br>
		''' Note: Boolean values are considered numerical (0/1)<br>
		''' </summary>
		Public ReadOnly Property Numerical As Boolean
			Get
				Return Me <> UTF8 AndAlso Me <> BOOL AndAlso Me <> COMPRESSED AndAlso Me <> UNKNOWN
			End Get
		End Property

		''' <returns> True if the datatype is a numerical type and is signed (supports negative values) </returns>
		Public ReadOnly Property Signed As Boolean
			Get
				Select Case Me
					Case [DOUBLE], FLOAT, HALF, [LONG], INT, [SHORT], [BYTE], BFLOAT16
						Return True
					Case Else
						Return False
				End Select
			End Get
		End Property

		''' <returns> the max number of significant decimal digits </returns>
		Public Function precision() As Integer
			Select Case Me
				Case [DOUBLE]
					Return 17
				Case FLOAT
					Return 9
				Case HALF
					Return 5
				Case BFLOAT16
					Return 4
				Case Else
					Return -1
			End Select
		End Function

		''' <returns> For fixed-width types, this returns the number of bytes per array element </returns>
		Public Function width() As Integer
			Select Case Me
				Case [DOUBLE], [LONG], UINT64
					Return 8
				Case FLOAT, INT, UINT32
					Return 4
				Case HALF, [SHORT], BFLOAT16, UINT16
					Return 2
				Case UBYTE, [BYTE], BOOL
					Return 1
				Case Else
					Return -1
			End Select
		End Function

		Public Shared Function fromNumpy(ByVal numpyDtypeName As String) As DataType
			Select Case numpyDtypeName.ToLower()
				Case "bool"
					Return BOOL
				Case "byte", "int8"
					Return INT8
				Case "int16"
					Return INT16
				Case "int32"
					Return INT32
				Case "int64"
					Return INT64
				Case "uint8"
					Return UINT8
				Case "float16"
					Return FLOAT16
				Case "float32"
					Return FLOAT
				Case "float64"
					Return [DOUBLE]
				Case "uint16"
					Return UINT16
				Case "uint32"
					Return UINT32
				Case "uint64"
					Return UINT64
				Case Else
					Throw New System.InvalidOperationException("Unknown datatype or no ND4J equivalent datatype exists: " & numpyDtypeName)
			End Select
		End Function

		Public Shared Function values() As DataType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As DataType, ByVal two As DataType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As DataType, ByVal two As DataType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As DataType
			For Each enumInstance As DataType In DataType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace