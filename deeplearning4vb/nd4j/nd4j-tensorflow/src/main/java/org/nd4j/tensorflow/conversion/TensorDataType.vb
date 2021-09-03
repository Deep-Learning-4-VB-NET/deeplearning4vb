Imports System.Collections.Generic
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor

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

Namespace org.nd4j.tensorflow.conversion

	Public NotInheritable Class TensorDataType
		Public Shared ReadOnly INVALID As New TensorDataType("INVALID", InnerEnum.INVALID)
		Public Shared ReadOnly FLOAT As New TensorDataType("FLOAT", InnerEnum.FLOAT)
		Public Shared ReadOnly [DOUBLE] As New TensorDataType("@DOUBLE", InnerEnum.DOUBLE)
		Public Shared ReadOnly INT32 As New TensorDataType("INT32", InnerEnum.INT32)
		Public Shared ReadOnly UINT8 As New TensorDataType("UINT8", InnerEnum.UINT8)
		Public Shared ReadOnly INT16 As New TensorDataType("INT16", InnerEnum.INT16)
		Public Shared ReadOnly INT8 As New TensorDataType("INT8", InnerEnum.INT8)
		Public Shared ReadOnly [STRING] As New TensorDataType("@STRING", InnerEnum.STRING)
		Public Shared ReadOnly COMPLEX64 As New TensorDataType("COMPLEX64", InnerEnum.COMPLEX64)
		Public Shared ReadOnly INT64 As New TensorDataType("INT64", InnerEnum.INT64)
		Public Shared ReadOnly BOOL As New TensorDataType("BOOL", InnerEnum.BOOL)
		Public Shared ReadOnly QINT8 As New TensorDataType("QINT8", InnerEnum.QINT8)
		Public Shared ReadOnly QUINT8 As New TensorDataType("QUINT8", InnerEnum.QUINT8)
		Public Shared ReadOnly QINT32 As New TensorDataType("QINT32", InnerEnum.QINT32)
		Public Shared ReadOnly BFLOAT16 As New TensorDataType("BFLOAT16", InnerEnum.BFLOAT16)
		Public Shared ReadOnly QINT16 As New TensorDataType("QINT16", InnerEnum.QINT16)
		Public Shared ReadOnly QUINT16 As New TensorDataType("QUINT16", InnerEnum.QUINT16)
		Public Shared ReadOnly UINT16 As New TensorDataType("UINT16", InnerEnum.UINT16)
		Public Shared ReadOnly COMPLEX128 As New TensorDataType("COMPLEX128", InnerEnum.COMPLEX128)
		Public Shared ReadOnly HALF As New TensorDataType("HALF", InnerEnum.HALF)
		Public Shared ReadOnly RESOURCE As New TensorDataType("RESOURCE", InnerEnum.RESOURCE)
		Public Shared ReadOnly [VARIANT] As New TensorDataType("@VARIANT", InnerEnum.VARIANT)
		Public Shared ReadOnly UINT32 As New TensorDataType("UINT32", InnerEnum.UINT32)
		Public Shared ReadOnly UINT64 As New TensorDataType("UINT64", InnerEnum.UINT64)

		Private Shared ReadOnly valueList As New List(Of TensorDataType)()

		Shared Sub New()
			valueList.Add(INVALID)
			valueList.Add(FLOAT)
			valueList.Add([DOUBLE])
			valueList.Add(INT32)
			valueList.Add(UINT8)
			valueList.Add(INT16)
			valueList.Add(INT8)
			valueList.Add([STRING])
			valueList.Add(COMPLEX64)
			valueList.Add(INT64)
			valueList.Add(BOOL)
			valueList.Add(QINT8)
			valueList.Add(QUINT8)
			valueList.Add(QINT32)
			valueList.Add(BFLOAT16)
			valueList.Add(QINT16)
			valueList.Add(QUINT16)
			valueList.Add(UINT16)
			valueList.Add(COMPLEX128)
			valueList.Add(HALF)
			valueList.Add(RESOURCE)
			valueList.Add([VARIANT])
			valueList.Add(UINT32)
			valueList.Add(UINT64)
		End Sub

		Public Enum InnerEnum
			INVALID
			FLOAT
			[DOUBLE]
			INT32
			UINT8
			INT16
			INT8
			[STRING]
			COMPLEX64
			INT64
			BOOL
			QINT8
			QUINT8
			QINT32
			BFLOAT16
			QINT16
			QUINT16
			UINT16
			COMPLEX128
			HALF
			RESOURCE
			[VARIANT]
			UINT32
			UINT64
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


		''' <summary>
		''' Map a tensor data type to a proto value found in tensorflow.
		''' Generally, this is just replacing DT_ with empty
		''' and returning enum.valueOf(string) </summary>
		''' <param name="value"> the input string </param>
		''' <returns> the associated <seealso cref="TensorDataType"/> </returns>
		Public Shared Function fromProtoValue(ByVal value As String) As TensorDataType
			Dim valueReplace As String = value.Replace("DT_","")
			Return TensorDataType.valueOf(valueReplace)
		End Function



		''' <summary>
		''' Get the python name for the given data type </summary>
		''' <param name="tensorDataType"> the python name for the given data type </param>
		''' <returns> float64 for double, float32 for double, float16 for half, otherwise
		''' the type's name converted to lower case </returns>
		Public Shared Function toPythonName(ByVal tensorDataType As TensorDataType) As String
			Select Case tensorDataType.innerEnumValue
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.DOUBLE
					Return "float64"
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.FLOAT
					Return "float32"
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.HALF
					Return "float16"

				Case Else
					Return tensorDataType.ToString().ToLower()
			End Select
		End Function

		Public Shared Function toNd4jType(ByVal tensorDataType As TensorDataType) As org.nd4j.linalg.api.buffer.DataType
			Select Case tensorDataType.innerEnumValue
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.FLOAT
					Return DataType.FLOAT
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.DOUBLE
					Return DataType.DOUBLE
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.BOOL
					Return DataType.BOOL
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.INT32
					Return DataType.INT
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.INT64
					Return DataType.LONG
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.STRING
					Return DataType.UTF8
				Case org.nd4j.tensorflow.conversion.TensorDataType.InnerEnum.HALF
					Return DataType.HALF
				Case Else
					Throw New System.ArgumentException("Unsupported type " & tensorDataType.ToString())
			End Select
		End Function


		Public Shared Function fromNd4jType(ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As TensorDataType
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.FLOAT
					Return TensorDataType.FLOAT
				Case DataType.InnerEnum.LONG
					Return TensorDataType.INT64
				Case DataType.InnerEnum.INT
					Return TensorDataType.INT32
				Case DataType.InnerEnum.BOOL
					Return TensorDataType.BOOL
				Case DataType.InnerEnum.DOUBLE
					Return TensorDataType.DOUBLE
				Case DataType.InnerEnum.HALF
					Return TensorDataType.HALF
				Case DataType.InnerEnum.UTF8
					Return TensorDataType.STRING
				Case DataType.InnerEnum.COMPRESSED
					Throw New System.InvalidOperationException("Unable to work with compressed data type. Could be 1 or more types.")
				Case DataType.InnerEnum.SHORT
					Return TensorDataType.INT16
				Case Else
					Throw New System.ArgumentException("Unknown data type " & dataType)
			End Select
		End Function

		Public Shared Function fromNd4jType(ByVal array As org.nd4j.linalg.api.ndarray.INDArray) As TensorDataType
			Dim dataType As DataType = array.dataType()
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.COMPRESSED
					Dim compressedData As CompressedDataBuffer = DirectCast(array.data(), CompressedDataBuffer)
					Dim desc As CompressionDescriptor = compressedData.getCompressionDescriptor()
					Dim algo As String = desc.getCompressionAlgorithm()
					Select Case algo
						Case "FLOAT16"
							Return HALF
						Case "INT8"
							Return INT8
						Case "UINT8"
							Return UINT8
						Case "INT16"
							Return INT16
						Case "UINT16"
							Return UINT16
						Case Else
							Throw New System.ArgumentException("Unsupported compression algorithm: " & algo)
					End Select

				Case Else
					Return fromNd4jType(dataType)
			End Select
		End Function


		Public Shared Function values() As TensorDataType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As TensorDataType, ByVal two As TensorDataType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As TensorDataType, ByVal two As TensorDataType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As TensorDataType
			For Each enumInstance As TensorDataType In TensorDataType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace