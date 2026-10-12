Imports com.google.flatbuffers

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

Namespace org.nd4j.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIVariable extends Table
	Public NotInheritable Class UIVariable
		Inherits Table

	  Public Shared Function getRootAsUIVariable(ByVal _bb As ByteBuffer) As UIVariable
		  Return getRootAsUIVariable(_bb, New UIVariable())
	  End Function
	  Public Shared Function getRootAsUIVariable(ByVal _bb As ByteBuffer, ByVal obj As UIVariable) As UIVariable
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIVariable
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function id() As IntPair
		  Return id(New IntPair())
	  End Function
	  Public Function id(ByVal obj As IntPair) As IntPair
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function name() As String
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function nameAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(6, 1)
	  End Function
	  Public Function nameInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 6, 1)
	  End Function
	  Public Function type() As SByte
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, bb.get(o + bb_pos), 0)
	  End Function
	  Public Function datatype() As SByte
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, bb.get(o + bb_pos), 0)
	  End Function
	  Public Function shape(ByVal j As Integer) As Long
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, bb.getLong(__vector(o) + j * 8), 0)
	  End Function
	  Public Function shapeLength() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function shapeAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(12, 8)
	  End Function
	  Public Function shapeInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 12, 8)
	  End Function
	  Public Function controlDeps(ByVal j As Integer) As String
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function controlDepsLength() As Integer
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function outputOfOp() As String
		  Dim o As Integer = __offset(16)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function outputOfOpAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(16, 1)
	  End Function
	  Public Function outputOfOpInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 16, 1)
	  End Function
	  Public Function inputsForOp(ByVal j As Integer) As String
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function inputsForOpLength() As Integer
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function controlDepsForOp(ByVal j As Integer) As String
		  Dim o As Integer = __offset(20)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function controlDepsForOpLength() As Integer
		  Dim o As Integer = __offset(20)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function controlDepsForVar(ByVal j As Integer) As String
		  Dim o As Integer = __offset(22)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function controlDepsForVarLength() As Integer
		  Dim o As Integer = __offset(22)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function gradientVariable() As String
		  Dim o As Integer = __offset(24)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function gradientVariableAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(24, 1)
	  End Function
	  Public Function gradientVariableInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 24, 1)
	  End Function
	  Public Function uiLabelExtra() As String
		  Dim o As Integer = __offset(26)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function uiLabelExtraAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(26, 1)
	  End Function
	  Public Function uiLabelExtraInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 26, 1)
	  End Function
	  Public Function constantValue() As FlatArray
		  Return constantValue(New FlatArray())
	  End Function
	  Public Function constantValue(ByVal obj As FlatArray) As FlatArray
		  Dim o As Integer = __offset(28)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function

	  Public Shared Function createUIVariable(ByVal builder As FlatBufferBuilder, ByVal idOffset As Integer, ByVal nameOffset As Integer, ByVal type As SByte, ByVal datatype As SByte, ByVal shapeOffset As Integer, ByVal controlDepsOffset As Integer, ByVal outputOfOpOffset As Integer, ByVal inputsForOpOffset As Integer, ByVal controlDepsForOpOffset As Integer, ByVal controlDepsForVarOffset As Integer, ByVal gradientVariableOffset As Integer, ByVal uiLabelExtraOffset As Integer, ByVal constantValueOffset As Integer) As Integer
		builder.startObject(13)
		UIVariable.addConstantValue(builder, constantValueOffset)
		UIVariable.addUiLabelExtra(builder, uiLabelExtraOffset)
		UIVariable.addGradientVariable(builder, gradientVariableOffset)
		UIVariable.addControlDepsForVar(builder, controlDepsForVarOffset)
		UIVariable.addControlDepsForOp(builder, controlDepsForOpOffset)
		UIVariable.addInputsForOp(builder, inputsForOpOffset)
		UIVariable.addOutputOfOp(builder, outputOfOpOffset)
		UIVariable.addControlDeps(builder, controlDepsOffset)
		UIVariable.addShape(builder, shapeOffset)
		UIVariable.addName(builder, nameOffset)
		UIVariable.addId(builder, idOffset)
		UIVariable.addDatatype(builder, datatype)
		UIVariable.addType(builder, type)
		Return UIVariable.endUIVariable(builder)
	  End Function

	  Public Shared Sub startUIVariable(ByVal builder As FlatBufferBuilder)
		  builder.startObject(13)
	  End Sub
	  Public Shared Sub addId(ByVal builder As FlatBufferBuilder, ByVal idOffset As Integer)
		  builder.addOffset(0, idOffset, 0)
	  End Sub
	  Public Shared Sub addName(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer)
		  builder.addOffset(1, nameOffset, 0)
	  End Sub
	  Public Shared Sub addType(ByVal builder As FlatBufferBuilder, ByVal type As SByte)
		  builder.addByte(2, type, 0)
	  End Sub
	  Public Shared Sub addDatatype(ByVal builder As FlatBufferBuilder, ByVal datatype As SByte)
		  builder.addByte(3, datatype, 0)
	  End Sub
	  Public Shared Sub addShape(ByVal builder As FlatBufferBuilder, ByVal shapeOffset As Integer)
		  builder.addOffset(4, shapeOffset, 0)
	  End Sub
	  Public Shared Function createShapeVector(ByVal builder As FlatBufferBuilder, ByVal data() As Long) As Integer
		  builder.startVector(8, data.Length, 8)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addLong(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startShapeVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(8, numElems, 8)
	  End Sub
	  Public Shared Sub addControlDeps(ByVal builder As FlatBufferBuilder, ByVal controlDepsOffset As Integer)
		  builder.addOffset(5, controlDepsOffset, 0)
	  End Sub
	  Public Shared Function createControlDepsVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startControlDepsVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addOutputOfOp(ByVal builder As FlatBufferBuilder, ByVal outputOfOpOffset As Integer)
		  builder.addOffset(6, outputOfOpOffset, 0)
	  End Sub
	  Public Shared Sub addInputsForOp(ByVal builder As FlatBufferBuilder, ByVal inputsForOpOffset As Integer)
		  builder.addOffset(7, inputsForOpOffset, 0)
	  End Sub
	  Public Shared Function createInputsForOpVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startInputsForOpVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addControlDepsForOp(ByVal builder As FlatBufferBuilder, ByVal controlDepsForOpOffset As Integer)
		  builder.addOffset(8, controlDepsForOpOffset, 0)
	  End Sub
	  Public Shared Function createControlDepsForOpVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startControlDepsForOpVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addControlDepsForVar(ByVal builder As FlatBufferBuilder, ByVal controlDepsForVarOffset As Integer)
		  builder.addOffset(9, controlDepsForVarOffset, 0)
	  End Sub
	  Public Shared Function createControlDepsForVarVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startControlDepsForVarVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addGradientVariable(ByVal builder As FlatBufferBuilder, ByVal gradientVariableOffset As Integer)
		  builder.addOffset(10, gradientVariableOffset, 0)
	  End Sub
	  Public Shared Sub addUiLabelExtra(ByVal builder As FlatBufferBuilder, ByVal uiLabelExtraOffset As Integer)
		  builder.addOffset(11, uiLabelExtraOffset, 0)
	  End Sub
	  Public Shared Sub addConstantValue(ByVal builder As FlatBufferBuilder, ByVal constantValueOffset As Integer)
		  builder.addOffset(12, constantValueOffset, 0)
	  End Sub
	  Public Shared Function endUIVariable(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace