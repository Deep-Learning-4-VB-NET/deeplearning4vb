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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIHardwareState extends Table
	Public NotInheritable Class UIHardwareState
		Inherits Table

	  Public Shared Function getRootAsUIHardwareState(ByVal _bb As ByteBuffer) As UIHardwareState
		  Return getRootAsUIHardwareState(_bb, New UIHardwareState())
	  End Function
	  Public Shared Function getRootAsUIHardwareState(ByVal _bb As ByteBuffer, ByVal obj As UIHardwareState) As UIHardwareState
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIHardwareState
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function gpuMemory(ByVal j As Integer) As Long
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.getLong(__vector(o) + j * 8), 0)
	  End Function
	  Public Function gpuMemoryLength() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function gpuMemoryAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(4, 8)
	  End Function
	  Public Function gpuMemoryInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 4, 8)
	  End Function
	  Public Function hostMemory() As Long
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function

	  Public Shared Function createUIHardwareState(ByVal builder As FlatBufferBuilder, ByVal gpuMemoryOffset As Integer, ByVal hostMemory As Long) As Integer
		builder.startObject(2)
		UIHardwareState.addHostMemory(builder, hostMemory)
		UIHardwareState.addGpuMemory(builder, gpuMemoryOffset)
		Return UIHardwareState.endUIHardwareState(builder)
	  End Function

	  Public Shared Sub startUIHardwareState(ByVal builder As FlatBufferBuilder)
		  builder.startObject(2)
	  End Sub
	  Public Shared Sub addGpuMemory(ByVal builder As FlatBufferBuilder, ByVal gpuMemoryOffset As Integer)
		  builder.addOffset(0, gpuMemoryOffset, 0)
	  End Sub
	  Public Shared Function createGpuMemoryVector(ByVal builder As FlatBufferBuilder, ByVal data() As Long) As Integer
		  builder.startVector(8, data.Length, 8)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addLong(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startGpuMemoryVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(8, numElems, 8)
	  End Sub
	  Public Shared Sub addHostMemory(ByVal builder As FlatBufferBuilder, ByVal hostMemory As Long)
		  builder.addLong(1, hostMemory, 0L)
	  End Sub
	  Public Shared Function endUIHardwareState(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace