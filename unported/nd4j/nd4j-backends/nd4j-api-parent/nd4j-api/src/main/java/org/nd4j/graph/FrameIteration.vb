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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class FrameIteration extends Table
	Public NotInheritable Class FrameIteration
		Inherits Table

	  Public Shared Function getRootAsFrameIteration(ByVal _bb As ByteBuffer) As FrameIteration
		  Return getRootAsFrameIteration(_bb, New FrameIteration())
	  End Function
	  Public Shared Function getRootAsFrameIteration(ByVal _bb As ByteBuffer, ByVal obj As FrameIteration) As FrameIteration
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As FrameIteration
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function frame() As String
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function frameAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(4, 1)
	  End Function
	  Public Function frameInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 4, 1)
	  End Function
	  Public Function iteration() As Integer
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, bb.getShort(o + bb_pos) And &HFFFF, 0)
	  End Function

	  Public Shared Function createFrameIteration(ByVal builder As FlatBufferBuilder, ByVal frameOffset As Integer, ByVal iteration As Integer) As Integer
		builder.startObject(2)
		FrameIteration.addFrame(builder, frameOffset)
		FrameIteration.addIteration(builder, iteration)
		Return FrameIteration.endFrameIteration(builder)
	  End Function

	  Public Shared Sub startFrameIteration(ByVal builder As FlatBufferBuilder)
		  builder.startObject(2)
	  End Sub
	  Public Shared Sub addFrame(ByVal builder As FlatBufferBuilder, ByVal frameOffset As Integer)
		  builder.addOffset(0, frameOffset, 0)
	  End Sub
	  Public Shared Sub addIteration(ByVal builder As FlatBufferBuilder, ByVal iteration As Integer)
		  builder.addShort(1, CShort(iteration), CShort(0))
	  End Sub
	  Public Shared Function endFrameIteration(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace