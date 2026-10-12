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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIAddName extends Table
	Public NotInheritable Class UIAddName
		Inherits Table

	  Public Shared Function getRootAsUIAddName(ByVal _bb As ByteBuffer) As UIAddName
		  Return getRootAsUIAddName(_bb, New UIAddName())
	  End Function
	  Public Shared Function getRootAsUIAddName(ByVal _bb As ByteBuffer, ByVal obj As UIAddName) As UIAddName
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIAddName
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function nameIdx() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
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

	  Public Shared Function createUIAddName(ByVal builder As FlatBufferBuilder, ByVal nameIdx As Integer, ByVal nameOffset As Integer) As Integer
		builder.startObject(2)
		UIAddName.addName(builder, nameOffset)
		UIAddName.addNameIdx(builder, nameIdx)
		Return UIAddName.endUIAddName(builder)
	  End Function

	  Public Shared Sub startUIAddName(ByVal builder As FlatBufferBuilder)
		  builder.startObject(2)
	  End Sub
	  Public Shared Sub addNameIdx(ByVal builder As FlatBufferBuilder, ByVal nameIdx As Integer)
		  builder.addInt(0, nameIdx, 0)
	  End Sub
	  Public Shared Sub addName(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer)
		  builder.addOffset(1, nameOffset, 0)
	  End Sub
	  Public Shared Function endUIAddName(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace