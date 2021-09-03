Imports val = lombok.val
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports DataType = org.tensorflow.framework.DataType

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

Namespace org.nd4j.imports.descriptors.properties.adapters


	Public Class DataTypeAdapter
		Implements AttributeAdapter

		Public Overridable Sub mapAttributeFor(ByVal inputAttributeValue As Object, ByVal fieldFor As System.Reflection.FieldInfo, ByVal [on] As DifferentialFunction) Implements AttributeAdapter.mapAttributeFor
			[on].setValueFor(fieldFor,dtypeConv(DirectCast(inputAttributeValue, DataType)))
		End Sub

		Public Shared Function dtypeConv(ByVal dataType As DataType) As org.nd4j.linalg.api.buffer.DataType
			Dim x As val = dataType.getNumber()

			Return dtypeConv(x)
		End Function


		Public Shared Function dtypeConv(ByVal dataType As Integer) As org.nd4j.linalg.api.buffer.DataType
			Select Case dataType
				Case DataType.DT_FLOAT_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.FLOAT
				Case DataType.DT_DOUBLE_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.DOUBLE
				Case DataType.DT_INT32_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.INT
				Case DataType.DT_UINT8_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.UBYTE
				Case DataType.DT_INT16_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.SHORT
				Case DataType.DT_INT8_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.BYTE
				Case DataType.DT_STRING_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.UTF8
				Case DataType.DT_INT64_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.LONG
				Case DataType.DT_BOOL_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.BOOL
				Case DataType.DT_UINT16_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.UINT16
				Case DataType.DT_HALF_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.HALF
				Case DataType.DT_UINT32_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.UINT32
				Case DataType.DT_UINT64_VALUE
					Return org.nd4j.linalg.api.buffer.DataType.UINT64
				Case Else
					Throw New System.NotSupportedException("DataType isn't supported: " & dataType & " - " & DataType.forNumber(dataType))
			End Select
		End Function
	End Class

End Namespace