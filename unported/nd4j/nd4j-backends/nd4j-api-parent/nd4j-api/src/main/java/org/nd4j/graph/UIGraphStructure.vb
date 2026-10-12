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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIGraphStructure extends Table
	Public NotInheritable Class UIGraphStructure
		Inherits Table

	  Public Shared Function getRootAsUIGraphStructure(ByVal _bb As ByteBuffer) As UIGraphStructure
		  Return getRootAsUIGraphStructure(_bb, New UIGraphStructure())
	  End Function
	  Public Shared Function getRootAsUIGraphStructure(ByVal _bb As ByteBuffer, ByVal obj As UIGraphStructure) As UIGraphStructure
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIGraphStructure
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function inputs(ByVal j As Integer) As String
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function inputsLength() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function inputsPair(ByVal j As Integer) As IntPair
		  Return inputsPair(New IntPair(), j)
	  End Function
	  Public Function inputsPair(ByVal obj As IntPair, ByVal j As Integer) As IntPair
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function inputsPairLength() As Integer
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function outputs(ByVal j As Integer) As String
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function outputsLength() As Integer
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function variables(ByVal j As Integer) As UIVariable
		  Return variables(New UIVariable(), j)
	  End Function
	  Public Function variables(ByVal obj As UIVariable, ByVal j As Integer) As UIVariable
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function variablesLength() As Integer
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function ops(ByVal j As Integer) As UIOp
		  Return ops(New UIOp(), j)
	  End Function
	  Public Function ops(ByVal obj As UIOp, ByVal j As Integer) As UIOp
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function opsLength() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function

	  Public Shared Function createUIGraphStructure(ByVal builder As FlatBufferBuilder, ByVal inputsOffset As Integer, ByVal inputsPairOffset As Integer, ByVal outputsOffset As Integer, ByVal variablesOffset As Integer, ByVal opsOffset As Integer) As Integer
		builder.startObject(5)
		UIGraphStructure.addOps(builder, opsOffset)
		UIGraphStructure.addVariables(builder, variablesOffset)
		UIGraphStructure.addOutputs(builder, outputsOffset)
		UIGraphStructure.addInputsPair(builder, inputsPairOffset)
		UIGraphStructure.addInputs(builder, inputsOffset)
		Return UIGraphStructure.endUIGraphStructure(builder)
	  End Function

	  Public Shared Sub startUIGraphStructure(ByVal builder As FlatBufferBuilder)
		  builder.startObject(5)
	  End Sub
	  Public Shared Sub addInputs(ByVal builder As FlatBufferBuilder, ByVal inputsOffset As Integer)
		  builder.addOffset(0, inputsOffset, 0)
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
	  Public Shared Sub addInputsPair(ByVal builder As FlatBufferBuilder, ByVal inputsPairOffset As Integer)
		  builder.addOffset(1, inputsPairOffset, 0)
	  End Sub
	  Public Shared Function createInputsPairVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startInputsPairVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addOutputs(ByVal builder As FlatBufferBuilder, ByVal outputsOffset As Integer)
		  builder.addOffset(2, outputsOffset, 0)
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
	  Public Shared Sub addVariables(ByVal builder As FlatBufferBuilder, ByVal variablesOffset As Integer)
		  builder.addOffset(3, variablesOffset, 0)
	  End Sub
	  Public Shared Function createVariablesVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startVariablesVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addOps(ByVal builder As FlatBufferBuilder, ByVal opsOffset As Integer)
		  builder.addOffset(4, opsOffset, 0)
	  End Sub
	  Public Shared Function createOpsVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startOpsVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Function endUIGraphStructure(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace