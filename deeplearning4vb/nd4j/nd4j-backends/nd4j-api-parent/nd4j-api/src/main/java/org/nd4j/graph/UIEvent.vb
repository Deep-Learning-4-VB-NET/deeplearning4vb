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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIEvent extends Table
	Public NotInheritable Class UIEvent
		Inherits Table

	  Public Shared Function getRootAsUIEvent(ByVal _bb As ByteBuffer) As UIEvent
		  Return getRootAsUIEvent(_bb, New UIEvent())
	  End Function
	  Public Shared Function getRootAsUIEvent(ByVal _bb As ByteBuffer, ByVal obj As UIEvent) As UIEvent
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIEvent
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function eventType() As SByte
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.get(o + bb_pos), 0)
	  End Function
	  Public Function eventSubType() As SByte
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, bb.get(o + bb_pos), 0)
	  End Function
	  Public Function nameIdx() As Integer
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
	  End Function
	  Public Function timestamp() As Long
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function iteration() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
	  End Function
	  Public Function epoch() As Integer
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
	  End Function
	  Public Function variableId() As Short
		  Dim o As Integer = __offset(16)
		  Return If(o <> 0, bb.getShort(o + bb_pos), 0)
	  End Function
	  Public Function frameIter() As FrameIteration
		  Return frameIter(New FrameIteration())
	  End Function
	  Public Function frameIter(ByVal obj As FrameIteration) As FrameIteration
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function plugin() As Integer
		  Dim o As Integer = __offset(20)
		  Return If(o <> 0, bb.getShort(o + bb_pos) And &HFFFF, 0)
	  End Function

	  Public Shared Function createUIEvent(ByVal builder As FlatBufferBuilder, ByVal eventType As SByte, ByVal eventSubType As SByte, ByVal nameIdx As Integer, ByVal timestamp As Long, ByVal iteration As Integer, ByVal epoch As Integer, ByVal variableId As Short, ByVal frameIterOffset As Integer, ByVal plugin As Integer) As Integer
		builder.startObject(9)
		UIEvent.addTimestamp(builder, timestamp)
		UIEvent.addFrameIter(builder, frameIterOffset)
		UIEvent.addEpoch(builder, epoch)
		UIEvent.addIteration(builder, iteration)
		UIEvent.addNameIdx(builder, nameIdx)
		UIEvent.addPlugin(builder, plugin)
		UIEvent.addVariableId(builder, variableId)
		UIEvent.addEventSubType(builder, eventSubType)
		UIEvent.addEventType(builder, eventType)
		Return UIEvent.endUIEvent(builder)
	  End Function

	  Public Shared Sub startUIEvent(ByVal builder As FlatBufferBuilder)
		  builder.startObject(9)
	  End Sub
	  Public Shared Sub addEventType(ByVal builder As FlatBufferBuilder, ByVal eventType As SByte)
		  builder.addByte(0, eventType, 0)
	  End Sub
	  Public Shared Sub addEventSubType(ByVal builder As FlatBufferBuilder, ByVal eventSubType As SByte)
		  builder.addByte(1, eventSubType, 0)
	  End Sub
	  Public Shared Sub addNameIdx(ByVal builder As FlatBufferBuilder, ByVal nameIdx As Integer)
		  builder.addInt(2, nameIdx, 0)
	  End Sub
	  Public Shared Sub addTimestamp(ByVal builder As FlatBufferBuilder, ByVal timestamp As Long)
		  builder.addLong(3, timestamp, 0L)
	  End Sub
	  Public Shared Sub addIteration(ByVal builder As FlatBufferBuilder, ByVal iteration As Integer)
		  builder.addInt(4, iteration, 0)
	  End Sub
	  Public Shared Sub addEpoch(ByVal builder As FlatBufferBuilder, ByVal epoch As Integer)
		  builder.addInt(5, epoch, 0)
	  End Sub
	  Public Shared Sub addVariableId(ByVal builder As FlatBufferBuilder, ByVal variableId As Short)
		  builder.addShort(6, variableId, 0)
	  End Sub
	  Public Shared Sub addFrameIter(ByVal builder As FlatBufferBuilder, ByVal frameIterOffset As Integer)
		  builder.addOffset(7, frameIterOffset, 0)
	  End Sub
	  Public Shared Sub addPlugin(ByVal builder As FlatBufferBuilder, ByVal plugin As Integer)
		  builder.addShort(8, CShort(plugin), CShort(0))
	  End Sub
	  Public Shared Function endUIEvent(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace