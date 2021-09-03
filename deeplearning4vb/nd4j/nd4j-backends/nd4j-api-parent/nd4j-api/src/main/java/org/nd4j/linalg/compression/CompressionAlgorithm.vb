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

Namespace org.nd4j.linalg.compression

	Public NotInheritable Class CompressionAlgorithm
		Public Shared ReadOnly FLOAT8 As New CompressionAlgorithm("FLOAT8", InnerEnum.FLOAT8)
		Public Shared ReadOnly FLOAT16 As New CompressionAlgorithm("FLOAT16", InnerEnum.FLOAT16)
		Public Shared ReadOnly GZIP As New CompressionAlgorithm("GZIP", InnerEnum.GZIP)
		Public Shared ReadOnly INT8 As New CompressionAlgorithm("INT8", InnerEnum.INT8)
		Public Shared ReadOnly INT16 As New CompressionAlgorithm("INT16", InnerEnum.INT16)
		Public Shared ReadOnly NOOP As New CompressionAlgorithm("NOOP", InnerEnum.NOOP)
		Public Shared ReadOnly UNIT8 As New CompressionAlgorithm("UNIT8", InnerEnum.UNIT8)
		Public Shared ReadOnly CUSTOM As New CompressionAlgorithm("CUSTOM", InnerEnum.CUSTOM)

		Private Shared ReadOnly valueList As New List(Of CompressionAlgorithm)()

		Shared Sub New()
			valueList.Add(FLOAT8)
			valueList.Add(FLOAT16)
			valueList.Add(GZIP)
			valueList.Add(INT8)
			valueList.Add(INT16)
			valueList.Add(NOOP)
			valueList.Add(UNIT8)
			valueList.Add(CUSTOM)
		End Sub

		Public Enum InnerEnum
			FLOAT8
			FLOAT16
			GZIP
			INT8
			INT16
			NOOP
			UNIT8
			CUSTOM
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
		''' Return the appropriate compression algorithm
		''' from the given string </summary>
		''' <param name="algorithm"> the algorithm to return </param>
		''' <returns> the compression algorithm from the given string
		''' or an IllegalArgumentException if the algorithm is invalid </returns>
		Public Shared Function fromString(ByVal algorithm As String) As CompressionAlgorithm
			Select Case algorithm.ToUpper()
				Case "FP16"
					Return FLOAT8
				Case "FP32"
					Return FLOAT16
				Case "FLOAT8"
					Return FLOAT8
				Case "FLOAT16"
					Return FLOAT16
				Case "GZIP"
					Return GZIP
				Case "INT8"
					Return INT8
				Case "INT16"
					Return INT16
				Case "NOOP"
					Return NOOP
				Case "UNIT8"
					Return UNIT8
				Case "CUSTOM"
					Return CUSTOM
				Case Else
					Throw New System.ArgumentException("Wrong algorithm " & algorithm)
			End Select
		End Function


		Public Shared Function values() As CompressionAlgorithm()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As CompressionAlgorithm, ByVal two As CompressionAlgorithm) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As CompressionAlgorithm, ByVal two As CompressionAlgorithm) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As CompressionAlgorithm
			For Each enumInstance As CompressionAlgorithm In CompressionAlgorithm.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace