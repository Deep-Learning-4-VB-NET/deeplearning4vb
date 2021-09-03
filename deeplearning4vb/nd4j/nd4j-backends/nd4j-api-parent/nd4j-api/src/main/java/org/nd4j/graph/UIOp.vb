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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIOp extends Table
	Public NotInheritable Class UIOp
		Inherits Table

	  Public Shared Function getRootAsUIOp(ByVal _bb As ByteBuffer) As UIOp
		  Return getRootAsUIOp(_bb, New UIOp())
	  End Function
	  Public Shared Function getRootAsUIOp(ByVal _bb As ByteBuffer, ByVal obj As UIOp) As UIOp
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIOp
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function name() As String
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function nameAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(4, 1)
	  End Function
	  Public Function nameInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 4, 1)
	  End Function
	  Public Function opName() As String
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function opNameAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(6, 1)
	  End Function
	  Public Function opNameInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 6, 1)
	  End Function
	  Public Function inputs(ByVal j As Integer) As String
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function inputsLength() As Integer
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function outputs(ByVal j As Integer) As String
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function outputsLength() As Integer
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function controlDeps(ByVal j As Integer) As String
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function controlDepsLength() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function uiLabelExtra() As String
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function uiLabelExtraAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(14, 1)
	  End Function
	  Public Function uiLabelExtraInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 14, 1)
	  End Function

	  Public Shared Function createUIOp(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer, ByVal opNameOffset As Integer, ByVal inputsOffset As Integer, ByVal outputsOffset As Integer, ByVal controlDepsOffset As Integer, ByVal uiLabelExtraOffset As Integer) As Integer
		builder.startObject(6)
		UIOp.addUiLabelExtra(builder, uiLabelExtraOffset)
		UIOp.addControlDeps(builder, controlDepsOffset)
		UIOp.addOutputs(builder, outputsOffset)
		UIOp.addInputs(builder, inputsOffset)
		UIOp.addOpName(builder, opNameOffset)
		UIOp.addName(builder, nameOffset)
		Return UIOp.endUIOp(builder)
	  End Function

	  Public Shared Sub startUIOp(ByVal builder As FlatBufferBuilder)
		  builder.startObject(6)
	  End Sub
	  Public Shared Sub addName(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer)
		  builder.addOffset(0, nameOffset, 0)
	  End Sub
	  Public Shared Sub addOpName(ByVal builder As FlatBufferBuilder, ByVal opNameOffset As Integer)
		  builder.addOffset(1, opNameOffset, 0)
	  End Sub
	  Public Shared Sub addInputs(ByVal builder As FlatBufferBuilder, ByVal inputsOffset As Integer)
		  builder.addOffset(2, inputsOffset, 0)
	  End Sub
	  Public Shared Function createInputsVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startInputsVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addOutputs(ByVal builder As FlatBufferBuilder, ByVal outputsOffset As Integer)
		  builder.addOffset(3, outputsOffset, 0)
	  End Sub
	  Public Shared Function createOutputsVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startOutputsVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addControlDeps(ByVal builder As FlatBufferBuilder, ByVal controlDepsOffset As Integer)
		  builder.addOffset(4, controlDepsOffset, 0)
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
	  Public Shared Sub addUiLabelExtra(ByVal builder As FlatBufferBuilder, ByVal uiLabelExtraOffset As Integer)
		  builder.addOffset(5, uiLabelExtraOffset, 0)
	  End Sub
	  Public Shared Function endUIOp(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace