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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class FlatInferenceRequest extends Table
	Public NotInheritable Class FlatInferenceRequest
		Inherits Table

	  Public Shared Function getRootAsFlatInferenceRequest(ByVal _bb As ByteBuffer) As FlatInferenceRequest
		  Return getRootAsFlatInferenceRequest(_bb, New FlatInferenceRequest())
	  End Function
	  Public Shared Function getRootAsFlatInferenceRequest(ByVal _bb As ByteBuffer, ByVal obj As FlatInferenceRequest) As FlatInferenceRequest
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As FlatInferenceRequest
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function id() As Long
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function variables(ByVal j As Integer) As FlatVariable
		  Return variables(New FlatVariable(), j)
	  End Function
	  Public Function variables(ByVal obj As FlatVariable, ByVal j As Integer) As FlatVariable
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function variablesLength() As Integer
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function configuration() As FlatConfiguration
		  Return configuration(New FlatConfiguration())
	  End Function
	  Public Function configuration(ByVal obj As FlatConfiguration) As FlatConfiguration
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function

	  Public Shared Function createFlatInferenceRequest(ByVal builder As FlatBufferBuilder, ByVal id As Long, ByVal variablesOffset As Integer, ByVal configurationOffset As Integer) As Integer
		builder.startObject(3)
		FlatInferenceRequest.addId(builder, id)
		FlatInferenceRequest.addConfiguration(builder, configurationOffset)
		FlatInferenceRequest.addVariables(builder, variablesOffset)
		Return FlatInferenceRequest.endFlatInferenceRequest(builder)
	  End Function

	  Public Shared Sub startFlatInferenceRequest(ByVal builder As FlatBufferBuilder)
		  builder.startObject(3)
	  End Sub
	  Public Shared Sub addId(ByVal builder As FlatBufferBuilder, ByVal id As Long)
		  builder.addLong(0, id, 0L)
	  End Sub
	  Public Shared Sub addVariables(ByVal builder As FlatBufferBuilder, ByVal variablesOffset As Integer)
		  builder.addOffset(1, variablesOffset, 0)
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
	  Public Shared Sub addConfiguration(ByVal builder As FlatBufferBuilder, ByVal configurationOffset As Integer)
		  builder.addOffset(2, configurationOffset, 0)
	  End Sub
	  Public Shared Function endFlatInferenceRequest(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	  Public Shared Sub finishFlatInferenceRequestBuffer(ByVal builder As FlatBufferBuilder, ByVal offset As Integer)
		  builder.finish(offset)
	  End Sub
	  Public Shared Sub finishSizePrefixedFlatInferenceRequestBuffer(ByVal builder As FlatBufferBuilder, ByVal offset As Integer)
		  builder.finishSizePrefixed(offset)
	  End Sub
	End Class


End Namespace