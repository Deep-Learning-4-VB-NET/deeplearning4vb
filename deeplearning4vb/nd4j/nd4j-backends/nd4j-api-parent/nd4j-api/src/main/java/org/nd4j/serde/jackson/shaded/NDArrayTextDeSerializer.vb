Imports System
Imports System.Collections.Generic
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode

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


	''' <summary>
	''' @author Adam Gibson
	''' </summary>

	Public Class NDArrayTextDeSerializer
		Inherits JsonDeserializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws java.io.IOException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As INDArray
			Dim n As JsonNode = jp.getCodec().readTree(jp)
			Return deserialize(n)
		End Function

		Public Overridable Function deserialize(ByVal n As JsonNode) As INDArray

			'First: check for backward compatilibity (RowVectorSerializer/Deserializer)
			If Not n.has("dataType") Then
				Dim size As Integer = n.size()
				Dim d(size - 1) As Double
				For i As Integer = 0 To size - 1
					d(i) = n.get(i).asDouble()
				Next i

				Return Nd4j.create(d)
			End If

			'Normal deserialize
			Dim dtype As String = n.get("dataType").asText()
			Dim dt As DataType = DataType.valueOf(dtype)
			Dim shapeNode As ArrayNode = CType(n.get("shape"), ArrayNode)
			Dim shape(shapeNode.size() - 1) As Long
			For i As Integer = 0 To shape.Length - 1
				shape(i) = shapeNode.get(i).asLong()
			Next i
			Dim dataNode As ArrayNode = CType(n.get("data"), ArrayNode)
			Dim iter As IEnumerator(Of JsonNode) = dataNode.elements()
			Dim i As Integer=0
			Dim arr As INDArray
			Select Case dt.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Dim d(dataNode.size() - 1) As Double
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: d[i++] = iter.Current.asDouble();
						d(i) = iter.Current.asDouble()
							i += 1
					Loop
					arr = Nd4j.create(d, shape, "c"c)
				Case DataType.InnerEnum.FLOAT, HALF
					Dim f(dataNode.size() - 1) As Single
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: f[i++] = iter.Current.floatValue();
						f(i) = iter.Current.floatValue()
							i += 1
					Loop
					arr = Nd4j.create(f, shape, "c"c).castTo(dt)
				Case DataType.InnerEnum.LONG
					Dim l(dataNode.size() - 1) As Long
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: l[i++] = iter.Current.longValue();
						l(i) = iter.Current.longValue()
							i += 1
					Loop
					arr = Nd4j.createFromArray(l).reshape("c"c, shape)
				Case DataType.InnerEnum.INT, [SHORT], UBYTE
					Dim a(dataNode.size() - 1) As Integer
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: a[i++] = iter.Current.intValue();
						a(i) = iter.Current.intValue()
							i += 1
					Loop
					arr = Nd4j.createFromArray(a).reshape("c"c, shape).castTo(dt)
				Case DataType.InnerEnum.BYTE, BOOL
					Dim b(dataNode.size() - 1) As SByte
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: b[i++] = (byte)iter.Current.intValue();
						b(i) = CSByte(Math.Truncate(iter.Current.intValue()))
							i += 1
					Loop
					arr = Nd4j.createFromArray(b).reshape("c"c, shape).castTo(dt)
				Case DataType.InnerEnum.UTF8
					Dim s(dataNode.size() - 1) As String
					Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: s[i++] = iter.Current.asText();
						s(i) = iter.Current.asText()
							i += 1
					Loop
					arr = Nd4j.create(s).reshape("c"c, shape)
				Case Else
					Throw New Exception("Unknown datatype: " & dt)
			End Select
			Return arr
		End Function
	End Class

End Namespace