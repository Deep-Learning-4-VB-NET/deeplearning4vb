Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider

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

Namespace org.nd4j.serde.jackson.shaded




	Public Class NDArrayTextSerializer
		Inherits JsonSerializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.nd4j.linalg.api.ndarray.INDArray arr, org.nd4j.shade.jackson.core.JsonGenerator jg, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws java.io.IOException
		Public Overrides Sub serialize(ByVal arr As INDArray, ByVal jg As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			jg.writeStartObject()
			jg.writeStringField("dataType", arr.dataType().ToString())
			jg.writeArrayFieldStart("shape")
			Dim i As Integer=0
			Do While i<arr.rank()
				jg.writeNumber(arr.size(i))
				i += 1
			Loop
			jg.writeEndArray()
			jg.writeArrayFieldStart("data")

			If arr.View OrElse arr.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(arr) OrElse arr.Compressed Then
				arr = arr.dup("c"c)
			End If

			Select Case arr.dataType()
				Case [DOUBLE]
					Dim d() As Double = arr.data().asDouble()
					For Each v As Double In d
						jg.writeNumber(v)
					Next v
				Case FLOAT, HALF
					Dim f() As Single = arr.data().asFloat()
					For Each v As Single In f
						jg.writeNumber(v)
					Next v
				Case [LONG]
					Dim l() As Long = arr.data().asLong()
					For Each v As Long In l
						jg.writeNumber(v)
					Next v
				Case INT, [SHORT], UBYTE
					Dim i() As Integer = arr.data().asInt()
					For Each v As Integer In i
						jg.writeNumber(v)
					Next v
				Case [BYTE], BOOL
					Dim b() As SByte = arr.data().asBytes()
					For Each v As SByte In b
						jg.writeNumber(v)
					Next v
				Case UTF8
					Dim n As val = arr.length()
					For j As Integer = 0 To n - 1
						Dim s As String = arr.getString(j)
						jg.writeString(s)
					Next j
				Case COMPRESSED, UNKNOWN
					Throw New System.NotSupportedException("Cannot JSON serialize array with datatype: " & arr.dataType())
			End Select
			jg.writeEndArray()
			jg.writeEndObject()
		End Sub
	End Class

End Namespace